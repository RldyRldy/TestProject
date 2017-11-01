using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using JGame.Pool;
using JGame.Data;
using JGame.Sound;

namespace JGame
{
    [System.Serializable]
    public class GameInfo
    {
        // max team num. 0 ~ teamNum
        public int teamNum;

        // map path
        public string mapPath;
    }

    public enum GameState
    {
        None,
        Init,
        Start,
        Play,
        Over,
    }
    
    public class GameManager : MonoBehaviour
    {
#region Camera
        public Camera FieldCamera;
        public Camera BattleCamera;
        public CameraInput cameraInput; // field camera input
        #endregion

        public SoundItem BGM = null;

        protected Hero cachedAttacker;
        protected Hero cachedDefender;

        GameInfo gameInfo = null;
        GameState _state;
        public Map map;

        #region controller
        // all controllers
        protected List<Controller> allController = new List<Controller>();
        // current turn controller 
        Controller controller = null;
        // local user controller
        Controller _localController = null;
        public Controller localController { get { return _localController; } }
        #endregion

        // current turn controller index
        public int curTurn = 0;

        // all teams
        List<Team> teams = new List<Team>();

        // is Initialized?
        public bool isInitialized = false;

        IEnumerator coroutine_Update = null;

        #region instance
        // instance
        private static GameManager _instance = null;
        public static GameManager instance
        {
            get
            {
                // if null create instance
                if (_instance == null) CreateInstance();

                return _instance;
            }
        }
        #endregion

        // Awake
        private void Awake()
        {
            if( _instance == null )
            {
                _instance = this;
            }

            SetState(GameState.Init);
        }

        // Set State
        public void SetState(GameState state)
        {
            if (state == _state) return;

            if (_state == GameState.Init && isInitialized == false ) return;

            _state = state;

            switch (_state)
            {
                case GameState.Init:
                    coroutine_Update = InitializeGame();
                    break;
                case GameState.Start:
                    coroutine_Update = StartGame();
                    break;
                case GameState.Play:
                    coroutine_Update = PlayState();
                    break;
                case GameState.Over:
                    coroutine_Update = EndGame();
                    break;
            }        
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            
            coroutine_Update = null;
        }

        private void Start()
        {
            if(cameraInput == null )
            {
                if (FieldCamera != null)
                {
                    cameraInput = FieldCamera.GetComponent<CameraInput>();
                }
                else
                {
                    cameraInput = JUtil.FindComponent<CameraInput>("Camera");
                }
            }

            StartCoroutine(UpdateGame());
        }

        private void Update()
        {
            // if not selected ui
            if(EventSystem.current.currentSelectedGameObject == null && EventSystem.current.IsPointerOverGameObject() == false )
            {
                ProcessInput();
            }
        }

        // Create Instance
        protected static void CreateInstance()
        {
            if (_instance != null) return;

            GameObject obj = new GameObject();

            if (obj != null)
            {
                obj.name = "Game Manager";
                _instance = obj.AddComponent<GameManager>();
            }
        }

#region State
        // Initialize Game
        IEnumerator InitializeGame()
        {
            yield return null;

            // wait data controller initialize
            while (!DataController.instance.isReady)
            {
                yield return null;
            }

            // {{ @ Test
            if (gameInfo == null )
            {
                gameInfo = new GameInfo();
                gameInfo.mapPath = "Map/Default";
                gameInfo.teamNum = 2;
            }
            // }} @Test

            // load map
            LoadMap();
            yield return null;

            // Create Teams
            for (int i = 0;i< gameInfo.teamNum ; ++i)
            {
                CreateTeam(i);
            }
            yield return null;
            
            // one team = one controller
            for ( int i =0; i< gameInfo.teamNum ; ++i)
            {
                CreateController(i);
            }

            // my controller
            _localController = allController[0];
            _localController.CreateArlamObj();
            yield return null;

            // Create Hero
            // {{ @test            
            for ( int j = 0; j<2;++j)
            {
                for (int i = 0; i < 4; ++i)
                {
                    CreateHero(allController[j],i);

                    yield return null;
                }
            }
            // }} @test

            isInitialized = true;
            yield return null;
            SetState(GameState.Start);
        }

        IEnumerator StartGame()
        {            
            yield return null;

            // Play BGM
            PlaySound(BGM);

            yield return new WaitForSeconds(1.0f);
            yield return new WaitForSeconds(1.0f);
            yield return new WaitForSeconds(1.0f);
            SetState(GameState.Play);
            
            UIManager.instance.ShowHUD(UIHUD.InGame_Normal);
        }

        IEnumerator PlayState()
        {
            yield return null;

            curTurn = -1;

            while (true)
            {
                // get next turn controller
                controller = GetNextTurnController();

                // set controller auto
                controller.isAuto = IsAuto(controller);

                // begin turn
                controller.BeginTurn();
                yield return null;

                // update turn
                IEnumerator e = controller.CalcTurn();
                while (e.MoveNext())
                {
                    yield return e.Current;
                }

                // end turn
                controller.EndTurn();
                yield return new WaitForSeconds(0.5f);

                // game over?
                if (CheckGameOver())
                {
                    SetState(GameState.Over);
                    yield break;
                }
                yield return null;
            }
        }

        IEnumerator EndGame()
        {
            yield return null;
        }     
        #endregion

        // create new controller
        protected Controller CreateController( int teamIndex)
        {
            GameObject Obj = ObjectPoolManager.instance.Pop( DataController.instance.GetDefaultController() );

            Obj.name = "Controller_" + teamIndex;

            var control = Obj.GetComponent<Controller>();

            control.SetTeam(GetTeam(teamIndex));
            control.transform.SetParent(null);

            return control;
        }
        
        // create new hero
        protected void CreateHero( Controller owner , int index )
        {
            SpawnPoint sp = GetBestSpawnPoint(owner.team.teamIndex);

            if (sp == null) return;

            GameObject Obj = ObjectPoolManager.instance.Pop( DataController.instance.GetDefaultHero() , sp.transform.position);

            Obj.name = "hero_" + index;

            var hero = Obj.GetComponent<Hero>();

            owner.AddHero(Obj);
            hero.SetOwner(owner);
            hero.SetPosition(sp.position);

            // {{ @Test
            HeroInfo info = new HeroInfo();
            info.name = "name" + Obj.name;
            for(int i = 0; i<9;++i)
            {
                info.soldiers[i] = Random.Range(0, 2);
            }
            // }} @Test
            hero.InitializeFromInfo(info);
        }

        // get spawn point
        protected SpawnPoint GetBestSpawnPoint(int teamIndex)
        {
            if (map == null)
            {
                return null;
            }

            for( int i = 0; i<map.spawnPoints.Count; ++i )
            {
                if( map.spawnPoints[i].teamIndex == teamIndex )
                {
                    if( map.GetTile(map.spawnPoints[i].position).actor == null )
                    {
                        return map.spawnPoints[i];
                    }
                }
            }

            return null;
        }

        // create new team
        protected Team CreateTeam(int index)
        {
            var team = new Team();

            team.teamIndex = index;
            teams.Add(team);

            return team;
        }

        protected Team GetTeam( int index )
        {
            int i = teams.FindIndex(item => item.teamIndex == index);
            if ( i == -1 )
            {
                return CreateTeam(index);
            }

            return teams[i];
        }

        // load map
        protected void LoadMap()
        {
            if( map == null )
            {
                map = JUtil.FindComponent<Map>();
            }

            if (map == null) return;

            if (gameInfo != null)
            {
                map.Initialize(gameInfo.mapPath);
            }
        }

        // Game Update
        IEnumerator UpdateGame()
        {
            while( true )
            {
                if( coroutine_Update != null )
                {
                    while( coroutine_Update.MoveNext() )
                    {
                        yield return coroutine_Update.Current;
                    }
                }

                yield return null;
            }
        }

        protected bool IsAuto( Controller ctr )
        {
            return (ctr != localController);
        }

        protected Controller GetNextTurnController()
        {
            curTurn += 1;

            // loop index
            if (curTurn < 0 || curTurn >= allController.Count )
            {
                curTurn = 0;
            }

            return allController[curTurn];
        }

        // game is over?
        protected bool CheckGameOver()
        {
            return false;
        }

        // process input
        protected bool ProcessInput()
        {
            if (cameraInput.ProcessInput())
            {
                return true;
            }

            if( localController != null && localController.ProcessInput() )
            {
                return true;
            }

            return false;
        }

        public void AddController(Controller ctr)
        {
            if (allController.Find(item => item == ctr) != null)
            {
                return;
            }

            allController.Add(ctr);
        }

        public void RemoveController(Controller ctr)
        {
            allController.Remove(ctr);
        }

        public void Surrender()
        {
            JUtil.LoadScene(Config.Scene.MainMenu);
        }

        public void BeginBattle( Hero attacker , Hero defender )
        {
            if (cameraInput != null)
            {
                cameraInput.SetLockCameraInput(true);
            }

            cachedAttacker = attacker;
            cachedDefender = defender;

            BattleCamera.gameObject.SetActive(true);
            FieldCamera.gameObject.SetActive(false);

            cachedAttacker.SetBattleMode(BattleCamera.transform.position + new Vector3(-1.0f, 0, 0), true);
            cachedDefender.SetBattleMode(BattleCamera.transform.position + new Vector3(1.0f,0,0), false);
        }

        public void EndBattle()
        {
            FieldCamera.gameObject.SetActive(true);
            BattleCamera.gameObject.SetActive(false);
            
            cachedAttacker.SetFieldMode();
            cachedDefender.SetFieldMode();

            if (cameraInput != null)
            {
                cameraInput.SetLockCameraInput(false);
            }
        }

        public void PlaySound( SoundItem item )
        {
            SoundManager.instance.PlaySound(item);
        }

        #region UI->GameManager
        #region Command
        public void Command_Attack()
        {
            if (controller != null && controller.CanAttackSelection() )
            {
                controller.SetState(Controller.State.Attack);
            }
        }

        public void Command_Throw()
        {
            if (controller != null)
            {
                controller.SetState(Controller.State.Throw);
            }
        }

        public void Command_Move()
        {
            if(controller != null)
            {
                controller.SetState(Controller.State.Move);
            }
        }

        public void Command_Rotate()
        {
            if (controller != null)
            {
                controller.SetState(Controller.State.Rotate);
            }
        }
        #endregion
        #endregion
    }
}
