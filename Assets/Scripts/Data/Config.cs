using UnityEngine;
using UnityEngine.SceneManagement;
using JGame.Data;

namespace JGame
{
    public class Config
    {
        [System.Serializable]
        public enum Scene
        {
            MainMenu,
            Loading,
            Ingame,
        }

        public const int MaxSoldierNum = 9; // 3x3

        // Team Index
        public const int teamNone = 255;
        public const int team1 = 0;
        public const int team2 = 1;

        public const int teamMax = 2;

        // system option
        public const float ResX = 1920.0f;
        public const float ResY = 1080.0f;
        
        public const float defaultResXY = ResX / ResY;
        public const float defaultResYX = ResY / ResX;

        // for data key
        public const string key_Country = "country";
        public const string key_DataVersion = "dataVersion";
    }

    [System.Serializable]
    public struct IntRect
    {
        public int x;
        public int y;

        public int magnitude { get { return Mathf.Abs(x) + Mathf.Abs(y); } }

        public void Normalize()
        {
            x = Mathf.Clamp(x, -1, 1);
            y = Mathf.Clamp(y, -1, 1);
        }

        public static IntRect operator -(IntRect left, IntRect right)
        {
            IntRect retval = new IntRect();
            retval.x = left.x - right.x;
            retval.y = left.y - right.y;
            return retval;
        }

        public static IntRect operator +(IntRect left, IntRect right)
        {
            IntRect retval = new IntRect();
            retval.x = left.x + right.x;
            retval.y = left.y + right.y;
            return retval;
        }
    }

    public class JUtil
    {
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void LoadScene( Config.Scene scene, int stage = -1 )
        {
            DataController.instance.SetLoadingInfo((int)scene, stage);

            SceneManager.LoadScene( (int)Config.Scene.Loading );
        }

        public static T FindComponent<T>() where T : MonoBehaviour
        {
            string objectName = typeof(T).Name;

            return FindComponent<T>(objectName);
        }

        public static T FindComponent<T>(string objectName) where T : MonoBehaviour
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                return null;
            }

            T comp = obj.GetComponent<T>();

            return comp;
        }

        public static float GetMaxResRatio()
        {
            return Mathf.Max( GetResRatioX(), GetResRatioY() );
        }

        public static float GetMinResRatio()
        {
            return Mathf.Min(GetResRatioX(), GetResRatioY());
        }

        public static float GetResRatioX()
        {
            return (Screen.width / Config.ResX);
        }

        public static float GetResRatioY()
        {
            return (Screen.height / Config.ResY);
        }

        public static float GetResRatioXY()
        {
            return ((float)Screen.width / Screen.height);
        }

        public static float GetResRatioYX()
        {
            return ((float)Screen.height / Screen.width);
        }
    }
}