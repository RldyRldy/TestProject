using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JGame.Pool;
using System.Reflection;
using System.Security.Permissions;

namespace JGame
{
    public class Controller : Actor
    {
        public enum State
        {
            BeginTurn,
            WaitCmd,
            Move,
            Rotate,
            Attack,
            Throw,
            Battle,
            EndTurn,
        }

        // is auto?
        public bool isAuto = false;
        
        // cached object last selected
        GameObject selectedObj = null;
        // selected tile arlram
        GameObject arlramObj = null;
        
        Hero curHero = null;
        int curHeroIndex;
        protected bool _isTurnEnded;

        // heros
        protected List<GameObject> teamHeros = new List<GameObject>();

        protected IEnumerator coroutine_Update = null;

        protected Controller.State _state = State.EndTurn;
        public Controller.State state { get { return _state; } }

        protected Dictionary<State, MethodInfo> beginStateList = new Dictionary<State, MethodInfo>();
        protected Dictionary<State, MethodInfo> updateStateList = new Dictionary<State, MethodInfo>();
        protected Dictionary<State, MethodInfo> endStateList = new Dictionary<State, MethodInfo>();

        private void Awake()
        {
            // cach state method infos
            foreach (State e in System.Enum.GetValues( typeof(State) ) )
            {
                string stateName = "State_" + e.ToString();
                System.Type class1Type = GetType();
                MethodInfo method = class1Type.GetMethod(stateName+"_Begin", BindingFlags.Instance | BindingFlags.NonPublic );
                if (method != null)
                {
                    beginStateList.Add(e, method);
                }

                method = class1Type.GetMethod(stateName, BindingFlags.Instance | BindingFlags.NonPublic );
                if (method != null)
                {
                    updateStateList.Add(e, method);
                }

                method = class1Type.GetMethod(stateName + "_End", BindingFlags.Instance | BindingFlags.NonPublic );
                if (method != null)
                {
                    endStateList.Add(e, method);
                }
            }
        }

        // change team
        public override void SetTeam(Team newTeam)
        {
            base.SetTeam(newTeam);

            if (team == newTeam) return;

            for (int i = 0; i < teamHeros.Count; ++i)
            {
                var hero = teamHeros[i].GetComponent<Hero>();

                if (hero != null)
                {
                    hero.SetTeam(team);
                }
            }
        }
        
        bool IsTurnEnded
        {
            get
            {
                if (_isTurnEnded || teamHeros.Count <= 0)
                    return true;

                foreach (var hero in teamHeros)
                {
                    if (hero != null && hero.GetComponent<Hero>() != null)
                    {
                        if (hero.GetComponent<Hero>().isTurnEnded == false)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        // begin turn
        public void BeginTurn()
        {
            SetSelectedObject(null);

            foreach( var hero in teamHeros )
            {
                if( hero != null && hero.GetComponent<Hero>() != null )
                {
                    hero.GetComponent<Hero>().BeginTurn();
                }
            }

            _isTurnEnded = false;
            curHeroIndex = GetBestHeroIndex(0);

            SetState(State.BeginTurn, true);
        }

        // end turn
        public void EndTurn()
        {
            foreach (var hero in teamHeros)
            {
                if (hero != null && hero.GetComponent<Hero>() != null)
                {
                    hero.GetComponent<Hero>().EndTurn();
                }
            }
            
            curHero = null;

            SetState(State.EndTurn, true);
        }

        #region State
        public void SetState(Controller.State newState , bool bForce = false )
        {
            if( _state == newState && !bForce )
            {
                return;
            }

            var oldState = _state;
            DoEndState(newState);
            _state = newState;
            DoBeginState(oldState);
            coroutine_Update = GetStateFunc();
        }

        protected void DoBeginState(Controller.State PrevState)
        {
#if UNITY_EDITOR
            Debug.Log("Begin state - " + name + " '" + PrevState.ToString() + "' -> '"+ _state.ToString() + "'");
#endif
            if( beginStateList == null || beginStateList.ContainsKey(_state) == false )
            {
                return;
            }
            
            MethodInfo method = beginStateList[_state];
            
            if( method != null )
            {
                method.Invoke(this, new object[] { PrevState });
            }
        }

        protected IEnumerator GetStateFunc()
        {
            if (updateStateList != null && updateStateList.ContainsKey(_state) != false)
            {
                MethodInfo method = updateStateList[_state];

                if (method != null)
                {
                    yield return method.Invoke(this, null);
                }
            }

            yield return null;
        }

        protected void DoEndState(Controller.State NextState)
        {
            if (endStateList == null || endStateList.ContainsKey(_state) == false)
            {
                return;
            }

            MethodInfo method = endStateList[_state];

            if (method != null)
            {
                method.Invoke(this, new object[] { NextState });
            }
        }
        #endregion

        #region State func
        #region State_BeginTurn
        protected void State_BeginTurn_Begin(Controller.State PrevState)
        {
        }
        protected IEnumerator State_BeginTurn()
        {
            yield return new WaitForSeconds(0.5f);

            SetState(State.WaitCmd);
        }
        protected void State_BeginTurn_End(Controller.State NextState)
        {

        }
        #endregion

        #region State_WaitCmd
        protected void State_WaitCmd_Begin(Controller.State PrevState)
        {
            // release selected object
            SetSelectedObject(null);

            curHero = teamHeros[curHeroIndex].GetComponent<Hero>();

            // move camera to current hero position
            GameManager.instance.cameraInput.SetPosition(curHero.transform.position);

            // draw hero point
            Map.instance.SetCurrentHeroGuide(true, curHero.transform.position);

            // update hero status ui
            UIManager.instance.SetSelection(UIWindow.InGame_Status_Hero, curHero.gameObject);

            // enable moveable tile
            Map.instance.DisableAllTiles();
            Map.instance.EnableCanMoveRangeTiles(curHero.position, curHero.moveRange, curHero );
        }
        protected IEnumerator State_WaitCmd()
        {
            while ( state == State.WaitCmd )
            {
                // DO Auto
                if (isAuto)
                {
                    //{{@Test
                    yield return new WaitForSeconds(0.5f);
                    EndCurrentHeroTurn();
                    break;
                    //}}@Test
                }
                else
                {
                    // process cmd
                    if (selectedObj != null)
                    {
                        var tile = selectedObj.GetComponent<Tile>();

                        // if selected tile
                        if (tile != null )
                        {
                            Hero other = null;

                            if (tile.actor != null)
                            {
                                other = tile.actor.GetComponent<Hero>();
                            }

                            // set command type.
                            if (other != null && other != curHero)
                            {
                                // attack command
                                UIManager.instance.ShowWindow(UIWindow.InGame_Command, (int)UIWnd_Cmd.Attack);
                            }
                            else if( tile.isSelecteable )
                            {
                                // move command
                                UIManager.instance.ShowWindow(UIWindow.InGame_Command, (int)UIWnd_Cmd.Move);
                            }
                            else
                            {
                                UIManager.instance.CloseWindow(UIWindow.InGame_Command);
                            }
                        }
                    }
                }

                yield return null;
            }
        }
        protected void State_WaitCmd_End(Controller.State NextState)
        {
            // update hero status ui
            UIManager.instance.SetSelection(UIWindow.InGame_Status_Hero, null);

            // enable all tile
            Map.instance.EnableAllTiles();

            UIManager.instance.CloseWindow(UIWindow.InGame_Command);
        }
        #endregion

        #region State_Move
        protected void State_Move_Begin(Controller.State PrevState)
        {
            curHero = teamHeros[curHeroIndex].GetComponent<Hero>();
        }
        protected IEnumerator State_Move()
        {
            yield return null;

            if (curHero != null && selectedObj != null)
            {
                var tile = selectedObj.GetComponent<Tile>();

                if (curHero.CanMoveTo(tile))
                {
                    curHero.MoveTo(tile);
                }
            }

            yield return new WaitForSeconds( 0.1f );
        }
        protected void State_Move_End(Controller.State NextState)
        {
            SetSelectedObject(null);
        }
        #endregion

        #region State_Rotate
        protected void State_Rotate_Begin(Controller.State PrevState)
        {
            // release selected object
            SetSelectedObject(null);

            // enable only 1x1 tiles
            Map.instance.DisableAllTiles();
            Map.instance.EnableRangeTiles(curHero.position, 1);

            // show direction guide ui
            Map.instance.ShowDirGuide(curHero.position);
        }
        protected IEnumerator State_Rotate()
        {
            yield return null;

            while ( state == Controller.State.Rotate )
            {
                // if choose dir tile
                if (selectedObj != null)
                {
                    var tile = selectedObj.GetComponent<Tile>();

                    if (tile != null && tile.isSelecteable)
                    {
                        curHero.LookAt(tile.position);
                        break;
                    }
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.1f);
        }
        protected void State_Rotate_End(Controller.State NextState)
        {
            // enable all tile
            Map.instance.EnableAllTiles();
            Map.instance.HideDirGuide();

            SetSelectedObject(null);
        }
        #endregion

        #region State_Attack
        protected void State_Attack_Begin(Controller.State PrevState)
        {
            Hero attacker, defender;

            attacker = curHero;
            defender = selectedObj.GetComponent<Hero>();

            if( defender == null )
            {
                var tile = selectedObj.GetComponent<Tile>();

                if (tile != null && tile.actor != null)
                {
                    defender = tile.actor.GetComponent<Hero>();
                }
            }

            GameManager.instance.BeginBattle(attacker, defender);

            // set ui
            UIManager.instance.ShowHUD(UIHUD.InGame_Battle);
            UIManager.instance.CloseHUD(UIHUD.InGame_Normal);

            UIManager.instance.SetSelection(UIWindow.InGame_Status_Attacker, attacker.gameObject );
            UIManager.instance.SetSelection(UIWindow.InGame_Status_Defender, defender.gameObject );
            
        }

        protected IEnumerator State_Attack()
        {
            yield return null;

            yield return new WaitForSeconds(5.0f);
        }

        protected void State_Attack_End(Controller.State NextState)
        {
            GameManager.instance.EndBattle();
            UIManager.instance.CloseHUD(UIHUD.InGame_Battle);
            UIManager.instance.ShowHUD(UIHUD.InGame_Normal);
        }
        #endregion

        #region State_EndTurn
        protected void State_EndTurn_Begin(Controller.State PrevState)
        {
            _isTurnEnded = true;
        }
        protected IEnumerator State_EndTurn()
        {
            yield return null;
        }
        protected void State_EndTurn_End(Controller.State NextState)
        {

        }
        #endregion

        #endregion

        // update turn
        public IEnumerator CalcTurn()
        {
            yield return null;

            while (!IsTurnEnded)
            {
                if (coroutine_Update == null)
                {
                    SetState(State.WaitCmd);
                }

                if (coroutine_Update != null)
                {
                    IEnumerator e = coroutine_Update;
                    if (e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        coroutine_Update = null;
                        yield return null;
                    }
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError(" can't find update func.");
#endif
                    yield return null;
                }
            }

            yield return null;
        }

        // Re Use
        public override void OnPoolReuse()
        {
            GameManager.instance.AddController(this);

            base.OnPoolReuse();
        }

        // Release
        public override void OnPoolReleased()
        {
            GameManager.instance.RemoveController(this);

            base.OnPoolReleased();
        }

        // process input
        public bool ProcessInput()
        {
            // if up
            if( JInputManager.ButtonUp(0) )
            {
                Vector2 wp = Camera.main.ScreenToWorldPoint(JInputManager.GetScreenPosition(0));
                Ray2D ray = new Ray2D(wp, Vector2.zero);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                
                if (hit.collider != null )
                {
                    SetSelectedObject(hit.collider.gameObject);
                }

                return true;
            }

            return false;
        }

        // add hero.
        public void AddHero(GameObject hero)
        {
            teamHeros.Add(hero);
        }

        // create arlam object.
        public void CreateArlamObj()
        {
            // create selected tile arlram 
            if (arlramObj == null)
            {
                arlramObj = ObjectPoolManager.instance.Pop(Resources.Load<GameObject>("Prefab/Map/Arrow"));

                if (arlramObj != null)
                {
                    arlramObj.SetActive(false);
                    arlramObj.transform.SetParent(transform);
                }
            }
        }

        // set selected object. 
        public void SetSelectedObject( GameObject obj )
        {
            if ( selectedObj == obj ) return;

            selectedObj = obj;

            UIManager.instance.SetSelection(UIWindow.InGame_Status_Selected, selectedObj);

            if ( selectedObj != null && arlramObj != null)
            {
                arlramObj.SetActive(true);

                arlramObj.transform.position = selectedObj.transform.position;
            }
            else if( arlramObj != null )
            {
                arlramObj.SetActive(false);
            }
        }

        // can attack selected hero?
        public bool CanAttackSelection()
        {
            if (curHero == null || selectedObj == null) return false;

            var tile = selectedObj.GetComponent<Tile>();

            if (tile != null )
            {
                Hero other = null;

                if (tile.actor != null)
                {
                    other = tile.actor.GetComponent<Hero>();
                }

                if (other != null)
                {
                    return curHero.CanAttack(other);
                }
            }

            return false;
        }

        // can throw attack selected hero?
        public bool CanThrowSelection()
        {
            if (curHero == null || selectedObj == null) return false;

            return false;
        }

        // can move to selected tile?
        public bool CanMoveSelection()
        {
            if (curHero == null || selectedObj == null) return false;

            var tile = selectedObj.GetComponent<Tile>();

            if (tile != null && tile.isSelecteable)
            {
                // check can move to
                if ( curHero.CanMoveTo( tile ) )
                {
                    return true;
                }
            }

            return false;
        }

        // pass turn
        public void PassCurrentHeroTurn()
        {
            if (curHero != null)
            {
                curHeroIndex = GetBestHeroIndex(curHeroIndex+1);
                
                if (curHeroIndex == -1)
                {
                    SetState(State.EndTurn);
                }
                else
                {
                    SetState(State.WaitCmd, true);
                }
            }
        }

        // end turn
        public void EndCurrentHeroTurn()
        {
            if (curHero != null)
            {
                curHero.EndTurn();
                curHeroIndex = GetBestHeroIndex( curHeroIndex+1);

                if( curHeroIndex == -1 )
                {
                    SetState(State.EndTurn);
                }
                
                if( !IsTurnEnded )
                {
                    SetState(State.WaitCmd, true);
                }
            }
        }

        // get best hero index
        public int GetBestHeroIndex(int idx)
        {
            if (idx < 0 || idx >= teamHeros.Count) idx = 0;

            for(int i = idx; i < teamHeros.Count; ++i )
            {
                if(teamHeros[i] != null && teamHeros[i].GetComponent<Hero>() != null )
                {
                    if (!teamHeros[i].GetComponent<Hero>().isTurnEnded && !teamHeros[i].GetComponent<Hero>().isDead)
                        return i;
                }
            }

            for( int i = 0; i <= idx;++i)
            {
                if (teamHeros[i] != null && teamHeros[i].GetComponent<Hero>() != null)
                {
                    if (!teamHeros[i].GetComponent<Hero>().isTurnEnded && !teamHeros[i].GetComponent<Hero>().isDead)
                        return i;
                }
            }

            return -1;
        }
    }
}
