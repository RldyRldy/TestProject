using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using JGame;
using JGame.Data;
using JGame.Localization;

namespace JGame
{
    public class LoadingManager : MonoBehaviour
    {
        [Header("Time")]
        // force minimum loading time
        public float minLoadTime = 2.0f;

        // wiat time after load scene finished
        public float waitTimeAfterFinish = 0.1f;

        [Header("UI")]
        // loading title text
        public Text txt_Tile = null;

        // stage text
        public Text txt_Stage = null;

        // loading percentage slider
        public Slider sld_Percentage = null;


        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadScene());
        }

        // set loading pct
        protected void SetLoadingPct( float pct )
        {
            if (sld_Percentage != null)
            {
                sld_Percentage.value = pct;
            }
        }

        // Load scene
        protected IEnumerator LoadScene()
        {
            int scene, stage;
            var duration = 0.0f;
            float pct = 0.0f;

            Sound.SoundManager.instance.StopAll();

            minLoadTime = Mathf.Max(0.001f, minLoadTime);

            DataController.instance.GetLoadingInfo( out scene, out stage );

            if( txt_Stage != null )
            {
                if (stage >= 0)
                {
                    txt_Stage.text = string.Format("{0} {1}", LocalizationManager.instance.GetLocalizedValue("L_Stage"), stage);
                }
                else
                {
                    txt_Stage.text = "";
                }
            }

            AsyncOperation async = SceneManager.LoadSceneAsync(scene);
            async.allowSceneActivation = false;

            yield return null;

            while( pct < 1.0f )
            {
                duration += Time.deltaTime;

                pct = 0.1f + Mathf.Min(duration / minLoadTime, async.progress);

                SetLoadingPct(pct);

                yield return null;
            }

            SetLoadingPct(1.0f);

            yield return new WaitForSeconds( Mathf.Max(0.001f,waitTimeAfterFinish) );

            async.allowSceneActivation = true;
        }
    }
}