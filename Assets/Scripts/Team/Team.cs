using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class Team
    {
        //protected List<Team> alianceList;

        // index of team
        public int teamIndex = Config.teamNone;

        // is same team ?
        public bool IsSameTeam(Team other)
        {
            if (teamIndex == Config.teamNone) return false;

            return (this == other || teamIndex == other.teamIndex);
        }
    }
}
