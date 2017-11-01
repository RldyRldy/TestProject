using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JGame.Pool;

namespace JGame
{
    public class Squad
    {
        protected IntRect position = new IntRect();
        protected IntRect rotation = new IntRect();

        public Hero owner;
        public Team team = null;
        protected Soldier[] members = new Soldier[Config.MaxSoldierNum];
        protected Soldier[] formation = new Soldier[Config.MaxSoldierNum];

        // Set member
        public void SetMembers( int idx , Soldier newMember )
        {
            if (idx < 0 || idx >= members.Length) return;
            
            members[idx] = newMember;

            if(members[idx] != null)
            {
                members[idx].SetTeam(team);
                members[idx].transform.SetParent(owner.transform);
            }
        }

        public void SetSelectable(bool selectable)
        {
            foreach (var member in members)
            {
                if( member != null )
                {
                    member.SetSelectable(selectable);
                }
            }
        }

        // set team
        public void SetTeam( Team newTeam )
        {
            if (team == newTeam) return;

            team = newTeam;

            // change members team
            for( int i =0; i < members.Length;++i)
            {
                var unit = members[i].GetComponent<Unit>();

                if (unit != null)
                {
                    unit.SetTeam(team);
                }
            }
        }

        public void MoveTo( IntRect newPos )
        {
            MoveTo(newPos.x, newPos.y);
        }

        public void UpdateMembersPosition()
        {
            for( int i = 0; i < formation.Length;++i )
            {
                if(formation[i] != null)
                {
                    formation[i].transform.position = GetMemberPosition(i);
                }
            }
        }

        public void UpdateSquadFormation()
        {
            var dir = new Vector3(rotation.x,rotation.y, 0);
            var front = new Vector3(0, -1, 0);
            var angle = Vector3.Angle(front, dir);

            // reflection.
            if ( rotation.x < 0 )
            {
                angle = -angle;
            }

            for ( int y = -1;y<2;++y) // -1 ~ 1
            {
                for(int x=-1;x<2;++x) // -1 ~ 1
                {
                    var v = new Vector3(x, y, 0);

                    // rotate
                    v = Quaternion.Euler(0, 0, angle) * v;

                    IntRect ir;
                    ir.x = (int)Mathf.Round(v.x) + 1;
                    ir.y = (int)Mathf.Round(v.y) + 1;

                    // set members
                    formation[ir.y * 3 + ir.x] = members[((y+1)*3)+ (x+1)];
                }
            }
        }

        public void UpdateMembersRotation()
        {
            foreach (var mem in members)
            {
                if( mem != null )
                {
                    mem.SetRotation(rotation);
                }
            }
        }

        public Vector3 GetMemberPosition( int i )
        {
            return GetMemberPosition(i% 3, i / 3);
        }

        public Vector3 GetMemberPosition(int x, int y)
        {
            var pos = owner.transform.position;
            pos += Map.instance.tileSpacingX * 0.3f * (x - 1);
            pos += Map.instance.tileSpacingY * 0.3f * (y - 1);

            return pos;
        }

        public void MoveTo(int x, int y)
        {
            var DestPos = position;

            position.x = x;
            position.y = y;

            SetRotation(position - DestPos);

            UpdateMembersPosition();
        }
        #region rotate
        public void SetRotation( IntRect r)
        {
            SetRotation(r.x, r.y);
        }
        public void SetRotation(int x, int y)
        {
            rotation.x = x;
            rotation.y = y;

            rotation.Normalize();

            UpdateSquadFormation();
            UpdateMembersRotation();
            UpdateMembersPosition();
        }
        #endregion

        // reuse
        public void OnPoolReuse()
        {

        }

        // released
        public void OnPoolReleased()
        {
            for (int i = 0; i < members.Length; ++i)
            {
                if (members[i] != null)
                {
                    ObjectPoolManager.instance.Push(members[i].gameObject);
                }
                members[i] = null;
            }
        }

        public int GetMoveRange()
        {
            return 2;
        }

        public void LookAt(IntRect target)
        {
            SetRotation(target - position);
        }

        public IEnumerable<Soldier> GetSoldiers(bool bFormation )
        {
            if(bFormation )
            {
                return formation;
            }

            return members;
        }

        public bool CanMoveToTile(Tile tile)
        {
            foreach( var mem in members )
            {
                if( mem != null 
                    && mem.IsAlivedAndWell() 
                    && ( mem.CanMoveToTile(tile) == false )
                    )
                {
                    return false;
                }
            }

            return true;
        }

        public void SetBattleMode(Vector3 loc, bool isAttacker)
        {
            var scale = new Vector3(1, 1, 1);
            var offset = new Vector3(0, 0, 0);

            if ( isAttacker == false )
            {
                scale.x = -1.0f;
            }
            else
            {
                for(int x=0;x<3;++x)
                {
                    for(int y=0;y<3;++y)
                    {
                        formation[x+ (y*3)] = members[((2-x)*3)+ y];
                    }
                }
            }

            for (int i = 0; i < formation.Length; ++i)
            {
                if (formation[i] != null)
                {
                    offset.x = ((i % 3) - 1) * 0.5f;
                    offset.y = ((i / 3) - 1) * -0.5f;
                    offset.z = offset.y * 0.1f;
                    formation[i].transform.position = loc + offset;
                    formation[i].transform.localScale = scale;
                    formation[i].transform.rotation = Quaternion.identity;
                }
            }
        }
    }
}
