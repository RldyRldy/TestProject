using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class SpawnPoint : MonoBehaviour
    {
        // team color
        public Color[] teamColor = new Color[Config.teamMax];
        // tile index (x,y)
        public IntRect position;

        // actor on tile
        public int teamIndex;

        private void Awake()
        {
            var sr = gameObject.GetComponent<SpriteRenderer>();

            sr.enabled = false;
        }

        public void SetTeamIndex( int index )
        {
            teamIndex = index;

#if UNITY_EDITOR
            if (Application.isEditor)
            {
                if (index >= 0 && index < teamColor.Length)
                {
                    var sr = gameObject.GetComponent<SpriteRenderer>();

                    sr.enabled = true;
                    sr.color = teamColor[index];
                    sr.sortingOrder = 1000;
                }
            }
#endif
        }
    }
}
