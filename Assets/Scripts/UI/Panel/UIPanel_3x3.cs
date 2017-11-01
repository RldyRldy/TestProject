using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JGame
{
    // has 3x3 cells.
    // cells hierarchy : cell bg -> icon -> level bg -> level txt
    public class UIPanel_3x3 : UIPanel
    {
        protected Vector2 cellSizeRect;

        protected override void InitFromAwake()
        {
            base.InitFromAwake();

            var gridGroup = gameObject.GetComponent<GridLayoutGroup>();

            if( gridGroup != null )
            {
                cellSizeRect = gridGroup.cellSize;
            }
        }

        protected void SetActiveCell( int idx, bool bActive )
        {
            var child = transform.GetChild(idx);
            child.GetChild(0).gameObject.SetActive(bActive);
        }

        protected void SetCellIcon(int idx, Sprite img)
        {
            var child = transform.GetChild(idx);
            var obj = child.GetChild(0).gameObject;

            var Image = obj.GetComponent<Image>();

            Image.sprite = img;
        }

        protected void SetCellLevel(int idx, int lvl)
        {
            var child = transform.GetChild(idx);
            var obj = child.GetChild(0).GetChild(0).GetChild(0).gameObject;

            var txt = obj.GetComponent<Text>();

            txt.text = lvl.ToString();
        }

        protected void SetCellHealth(int idx, float pct )
        {
            var child = transform.GetChild(idx);
            var obj = child.GetChild(0).GetChild(1).GetChild(0).gameObject;

            var rect = obj.transform as RectTransform;
            rect.anchorMax = new Vector2(pct, 1.0f);
        }

        public void SetHeroInfo( Hero hero )
        {
            var soldiers = hero.GetSoldiers( false );

#if UNITY_EDITOR
            if (transform.childCount != 9 )
            {
                Debug.LogError(" transform.childCount != 9 ");
            }
#endif
            int i = 0;
            foreach (var soldier in soldiers)
            {
                if( soldier != null && soldier.IsAlivedAndWell() )
                {
                    SetActiveCell(i,true);
                    SetCellIcon(i, soldier.iconImg);
                    SetCellLevel(i, soldier.level);
                    SetCellHealth(i, soldier.health / (float)soldier.healthMax);
                }
                else
                {
                    SetActiveCell(i, false);
                }
                ++i;
            }
        }

        public override void Resizing()
        {
            base.Resizing();

            if (resizeType != EUIResizeType.None)
            {
                var gridGroup = gameObject.GetComponent<GridLayoutGroup>();

                if (gridGroup != null)
                {
                    var newSize = cellSizeRect;

                    newSize = GetResizingSize( ref newSize, resizeType );

                    gridGroup.cellSize = newSize;
                }
            }
        }
    }
}
