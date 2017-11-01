using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JGame.Data;

namespace JGame
{
    [System.Serializable]
    public class HeroInfo
    {
        public string name;
        public int[] soldiers = new int[Config.MaxSoldierNum];
    }

    public class Hero : Unit
    {
        protected int _moveRange;
        public int moveRange {  get { return _moveRange; } }

        public bool isTurnEnded;

        public IntRect position = new IntRect();

        Controller owner = null;

        // hero troops
        protected Squad squad = new Squad();

        // awake
        private void Awake()
        {
            squad.owner = this;
        }

        // begin turn
        public void BeginTurn()
        {
            isTurnEnded = false;
            _moveRange = squad.GetMoveRange();
        }

        // end turn
        public void EndTurn()
        {
            _moveRange = 0;
            isTurnEnded = true;
        }

        // set selectable
        public override void SetSelectable(bool selectable)
        {
            base.SetSelectable(selectable);
            squad.SetSelectable(selectable);
        }

        // can attack
        public bool CanAttack(Hero other)
        {
            //{{@test
            return true;
            //}}@test
            //return !team.IsSameTeam(other.team);
        }

        public bool CanMoveTo(Tile tile)
        {
            if (tile.actor != null) return false;

            if ( (tile.position - position).magnitude > _moveRange )
            {
                return false;
            }

            return true;
        }

        public bool MoveTo(Tile tile)
        {
            // cached curren position
            var destPos = position;

            // set new position
            SetPosition(tile.position);

            // reduce move range
            _moveRange -= (destPos - position).magnitude;

            #if UNITY_EDITOR
            if (_moveRange < 0)
            {
                Debug.LogError("_moveRange < 0");
            }
            #endif

            return true;
        }
        // set position
        #region setposition
        public void SetPosition(IntRect pos)
        {
            SetPosition(pos.x, pos.y);
        }
        public void SetPosition(int x, int y)
        {
            // detach from old tile
            Map.instance.LeaveFrom(position, gameObject);

            position.x = x;
            position.y = y;

            // attach to new tile
            Map.instance.MoveTo(position, gameObject);
            transform.position = Map.instance.GetTile(x, y).transform.position;

            squad.MoveTo(x, y);
        }
        #endregion

        // initialize from info
        public void InitializeFromInfo(HeroInfo info)
        {
            if (info == null) return;

            // set name
            name = info.name;

            squad.MoveTo(position);

            // add soldiers
            for ( int i = 0; i<info.soldiers.Length; ++i)
            {
                squad.SetMembers(i, DataController.CreateSoliderById(info.soldiers[i]) );
            }

            squad.UpdateSquadFormation();
            squad.UpdateMembersRotation();
            squad.UpdateMembersPosition();
        }

        public IEnumerable<Soldier> GetSoldiers(bool bFormation)
        {
            return squad.GetSoldiers(bFormation);
        }

        // set owner
        public void SetOwner( Controller newOwner)
        {
            if (owner == newOwner) return;

            owner = newOwner;

            if (owner == null) return;

            SetTeam(owner.team);
            transform.SetParent(newOwner.transform);
        }

        // change team
        public override void SetTeam(Team newTeam)
        {
            base.SetTeam(newTeam);

            if (team == newTeam) return;

            squad.SetTeam(newTeam);
        }

        // Re Use
        public override void OnPoolReuse()
        {
            base.OnPoolReuse();
            squad.OnPoolReuse();
        }

        // Release
        public override void OnPoolReleased()
        {
            base.OnPoolReleased();
            squad.OnPoolReleased();
        }

        public int GetMoveSpeed()
        {
            return 2;
        }

        public void LookAt(IntRect target)
        {
            squad.LookAt(target);
        }

        public bool CanMoveToTile(Tile tile)
        {
            return squad.CanMoveToTile(tile);
        }

        public void SetBattleMode( Vector3 loc, bool isAttacker )
        {
            loc.z = 0.0f;
            transform.position = loc;

            squad.SetBattleMode(loc, isAttacker);
        }

        public void SetFieldMode()
        {
            transform.position = Map.instance.GetTile(position.x, position.y).transform.position;
            
            squad.UpdateSquadFormation();
            squad.UpdateMembersRotation();
            squad.UpdateMembersPosition();
        }
    }
}
