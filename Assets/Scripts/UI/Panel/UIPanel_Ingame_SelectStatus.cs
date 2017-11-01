using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JGame
{
    public class UIPanel_Ingame_SelectStatus : UIPanel
    {
        public bool isHiddenStart = true;
        public Text txtName;
        public UIPanel_3x3 cell3x3 = null;
        GameObject selection;

        public void SetSelection( GameObject obj )
        {
            if (obj != null)
            {
                Tile t = obj.GetComponent<Tile>();

                if (t != null)
                {
                    obj = t.actor;
                }
            }

            if (selection == obj) return;

            selection = obj;

            Hero hero = null;

            if (selection != null)
            {
                hero = selection.GetComponent<Hero>();
            }
            
            if(selection != null && hero != null )
            {
                if(txtName != null )
                {
                    txtName.text = hero.name;
                }

                SetActive(true);
                if (cell3x3 != null)
                {
                    cell3x3.SetHeroInfo(hero);
                }
            }
            else
            {
                SetActive(false);
            }
        }

        protected override void InitFromStart()
        {
            base.InitFromStart();

            if (isHiddenStart)
            {
                SetActive(false, true);
            }
        }
    }
}