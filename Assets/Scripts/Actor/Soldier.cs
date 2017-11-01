using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class Soldier : Unit
    {
        [Header("Image")]
        // in status image.
        public Sprite iconImg = null;

        [Header("Animation")]
        public bool hasDirAnim;
        
        [Header("Properties")]
        // can move tile type
        public List<Tile.TileType> canMoveType = new List<Tile.TileType>();

        // level of soldier
        public byte level;

        protected IntRect rotation = new IntRect();

        public bool IsAlivedAndWell()
        {
            return true;
        }

        public bool CanMoveToTile( Tile tile )
        {
            if (tile == null) return false;

            if (tile.actor != null) return false;

            return (canMoveType.FindIndex( type => type == tile.type ) >= 0) ;
        }

        #region Rotate
        public void SetRotation(IntRect r)
        {
            SetRotation(r.x, r.y);
        }

        public void SetRotation(int x, int y)
        {
            rotation.x = x;
            rotation.y = y;
            
            if ( !hasDirAnim )
            {
                Vector3 angle = new Vector3(0, 0, 15);
                Vector3 scale = new Vector3(1, 1, 1);

                if( y == 0 )
                {
                    angle *= -1.0f;
                }

                if (x < 0 || y > 0 )
                {
                    scale.x *= -1.0f;
                }

                transform.rotation = Quaternion.Euler(angle);
                transform.localScale = scale;
            }
        }
        #endregion
    }
}
