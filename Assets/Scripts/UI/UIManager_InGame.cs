using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class UIManager_InGame : UIManager
    {
        [Header("UI Objects")]
        public GameObject NormalHUD;
        public GameObject BattleHUD;
        public GameObject MessageUI;
        public GameObject OptionWnd;

        [Header("UI Scripts")]
        public UIPanel_Ingame_Command commandMenu;
        public UIPanel_Ingame_SelectStatus selectedObjStat;
        public UIPanel_Ingame_SelectStatus curHeroStat;
        public UIPanel_Ingame_SelectStatus attackerStat;
        public UIPanel_Ingame_SelectStatus defenderStat;

        // Use this for initialization
        void Start()
        {
            CloseHUD(UIHUD.InGame_Battle);
            CloseHUD(UIHUD.InGame_Normal);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void ShowWindow(UIWindow wnd, int param1)
        {
            switch(wnd)
            {
                case UIWindow.InGame_Command:
                    if( commandMenu != null)
                    {
                        commandMenu.ShowWindow(param1);
                    }
                    break;
                default:
                    base.ShowWindow(wnd, param1);
                    break;
            }
        }

        public override void ShowHUD(UIHUD hud)
        {
            switch(hud)
            {
                case UIHUD.InGame_Normal:
                    NormalHUD.SetActive(true);
                    break;
                case UIHUD.InGame_Battle:
                    BattleHUD.SetActive(true);
                    break;
            }
        }

        public override void CloseHUD(UIHUD hud)
        {
            switch (hud)
            {
                case UIHUD.InGame_Normal:
                    NormalHUD.SetActive(false);
                    break;
                case UIHUD.InGame_Battle:
                    BattleHUD.SetActive(false);
                    break;
            }
        }

        public override void CloseWindow(UIWindow wnd)
        {
            switch (wnd)
            {
                case UIWindow.InGame_Command:
                    if (commandMenu != null)
                    {
                        commandMenu.SetActive(false);
                    }
                    break;
                default:
                    base.CloseWindow(wnd);
                    break;
            }
        }

        public override void SetSelection(UIWindow wnd, GameObject selection)
        {
            switch (wnd)
            {
                case UIWindow.InGame_Status_Hero:
                    if (curHeroStat != null)
                    {
                        curHeroStat.SetSelection(selection);
                    }
                    break;
                case UIWindow.InGame_Status_Selected:
                    if (selectedObjStat != null)
                    {
                        selectedObjStat.SetSelection(selection);
                    }
                    break;
                case UIWindow.InGame_Status_Attacker:
                    if( attackerStat != null )
                    {
                        attackerStat.SetSelection(selection);
                    }
                    break;
                case UIWindow.InGame_Status_Defender:
                    if( defenderStat != null )
                    {
                        defenderStat.SetSelection(selection);
                    }
                    break;
                default:
                    base.SetSelection(wnd, selection);
                    break;
            }
        }

        public void SetShowOptionWnd(bool bShow)
        {
            if(OptionWnd != null)
            {
                OptionWnd.SetActive(bShow);
            }
        }

        protected IEnumerator MessageBox_Surrender()
        {
            if( MessageUI == null )
            {
                yield break;
            }
            
            var wnd = MessageUI.GetComponent<UIPanel_MessageBox_YN>();

            if (wnd != null)
            {
                wnd.ShowMsgBox("L_Ask_Surrender");

                while ( wnd.IsWorking() )
                {
                    yield return new WaitForSeconds(0.1f);
                }

                if (wnd.IsYes())
                {
                    GameManager.instance.Surrender();
                }
            }
        }

        #region UIFunc
        public void OnClick_PassTurn()
        {
            GameManager.instance.localController.PassCurrentHeroTurn();
        }

        public void OnClick_EndTurn()
        {
            GameManager.instance.localController.EndCurrentHeroTurn();
        }

        public void OnClick_Option()
        {
            SetShowOptionWnd(true);
        }

        public void OnClick_Surrender()
        {
            StartCoroutine(MessageBox_Surrender());
            SetShowOptionWnd(false);
        }
        #endregion
    }
}
