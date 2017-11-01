using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JGame
{
    public enum UIWnd_Cmd
    {
        Attack,
        Move,
    }

    public enum UICmd_Attack
    {
        Cancel,
        Attack,
        Throw,
    }

    public enum UICmd_Move
    {
        Cancel,
        Move,
        Rotate,
    }

    public class UIPanel_Ingame_Command : UIPanel
    {
        protected int selectedBtIndex;

        public Button attCmd_Att;
        public Button attCmd_Throw;
        public Button attCmd_Cancel;
        public Button moveCmd_Move;
        public Button moveCmd_Rotate;
        public Button moveCmd_Cancel;

        public GameObject attackCommand;
        public GameObject moveCommand;
        
        protected override void InitFromStart()
        {
            UIManager_InGame mgr = (UIManager_InGame)UIManager.instance;

            if (mgr != null)
            {
                mgr.commandMenu = this;
            }

            base.InitFromStart();

            SetActive(false , true);
            SetShowAllWindows(false);

            // attack command - attack
            if (attCmd_Att != null)
            {
                attCmd_Att.onClick.AddListener(() => { GameManager.instance.Command_Attack(); });
            }

            // attack command - throw
            if (attCmd_Throw != null)
            {
                attCmd_Throw.onClick.AddListener(() => { GameManager.instance.Command_Throw(); });
            }

            // move command - move
            if (moveCmd_Move != null)
            {
                moveCmd_Move.onClick.AddListener(() => { GameManager.instance.Command_Move(); });
            }

            // move command - rotate
            if (moveCmd_Rotate != null)
            {
                moveCmd_Rotate.onClick.AddListener(() => { GameManager.instance.Command_Rotate(); });
            }
        }

        public void ShowWindow( int param1 )
        {
            selectedBtIndex = -1;

            SetActive(true);
            SetShowAllWindows(false);

            ShowWindow((UIWnd_Cmd)param1);
        }

        protected void SetShowAllWindows(bool bShow)
        {
            if (attackCommand != null) attackCommand.SetActive(bShow);
            if (moveCommand != null) moveCommand.SetActive(bShow);
        }

        protected void ShowWindow( UIWnd_Cmd type )
        {
            switch(type)
            {
                case UIWnd_Cmd.Attack:
                    if (attackCommand != null)
                    {
                        UpdateAttackCmdInteractable();
                        attackCommand.SetActive(true);
                    }
                    break;
                case UIWnd_Cmd.Move:
                    if (moveCommand != null)
                    {
                        UpdateMoveCmdInteractable();
                        moveCommand.SetActive(true);
                    }
                    break;
            }
        }

        protected void UpdateAttackCmdInteractable()
        {
            // attack command - attack
            if (attCmd_Att != null)
            {
                attCmd_Att.interactable = GameManager.instance.localController.CanAttackSelection();
            }

            // attack command - throw
            if (attCmd_Throw != null)
            {
                attCmd_Throw.interactable = GameManager.instance.localController.CanThrowSelection();
            }
        }

        protected void UpdateMoveCmdInteractable()
        {
            // move command - move
            if (moveCmd_Move != null)
            {
                moveCmd_Move.interactable = GameManager.instance.localController.CanMoveSelection();
            }
        }
    }
}