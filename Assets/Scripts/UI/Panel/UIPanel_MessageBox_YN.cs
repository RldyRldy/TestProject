using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JGame.Localization;

namespace JGame
{
    public class UIPanel_MessageBox_YN : UIPanel
    {
        public Text text;

        protected int result;

        public void ShowMsgBox(string title)
        {
            if( text != null )
            {
                text.text = LocalizationManager.instance.GetLocalizedValue(title);
            }
            gameObject.SetActive(true);
            result = -1;
        }

        protected override void InitFromStart()
        {
            base.InitFromStart();
        }

        public bool IsWorking()
        {
            return ( gameObject.activeInHierarchy && result < 0 );
        }

        public bool IsYes()
        {
            return (result > 0);
        }

        public void OnClick_Yes()
        {
            result = 1;
            gameObject.SetActive(false);
        }

        public void OnClick_No()
        {
            result = 0;
            gameObject.SetActive(false);
        }
    }
}
