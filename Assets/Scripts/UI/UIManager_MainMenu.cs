using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame
{
    public class UIManager_MainMenu : UIManager
    {
        #region UIFunc
        public void OnClick_Play()
        {
            JUtil.LoadScene(Config.Scene.Ingame, 1);
        }

        public void OnClick_Quit()
        {
            JUtil.Quit();
        }
        #endregion
    }
}
