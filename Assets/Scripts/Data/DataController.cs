using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JGame.Pool;

namespace JGame.Data
{
    public class DataController : MonoBehaviour
    {
        // default prefabs
        protected static GameObject defaultController;
        protected static GameObject defaultHero;
        protected static Dictionary<int, GameObject> defaultSoldiers = new Dictionary<int, GameObject>();

        // data version
        const float version = 1.0f;

        // instance
        private static DataController _instance = null;
        public static DataController instance
        {
            get
            {
                if (_instance == null) CreateInstance();

                return _instance;
            }
        }

        public bool isReady = false;

        // create instance
        protected static void CreateInstance()
        {
            if (_instance != null) return;

            GameObject obj = new GameObject();

            if (obj != null)
            {
                obj.name = "Data Controller";
                _instance = obj.AddComponent<DataController>();
                _instance.Initialize();
                DontDestroyOnLoad(obj);
            }
        }

        // check data version
        private bool IsOldData()
        {
            return (GetFloatData(Config.key_DataVersion) < version);
        }

        // update old data
        private void UpdateOldData()
        {
            SetFloatData(Config.key_DataVersion, version);
        }

        private bool IsFirstExcute()
        {
            return !PlayerPrefs.HasKey(Config.key_DataVersion);
        }

        private void DoFirstExcute()
        {
            SetFloatData(Config.key_DataVersion, version);
        }

        // initialize
        protected void Initialize()
        {
            //@Test
            //PlayerPrefs.DeleteAll();

            if (IsFirstExcute())
            {
                DoFirstExcute();
            }

            if (IsOldData())
            {
                UpdateOldData();
            }

            // get default controller
            if( defaultController == null )
            {
                defaultController = CreateDataObject<Controller>();
            }

            // get default hero
            if( defaultHero == null )
            {
                defaultHero = CreateDataObject<Hero>();
            }

            // {{ @Test
            defaultSoldiers.Add(0,Resources.Load<GameObject>("Prefab/Units/SwordMan"));
            defaultSoldiers.Add(1, Resources.Load<GameObject>("Prefab/Units/Archer"));
            // }} @Test

            isReady = true;
        }

        // load Json data
        public static T LoadJson<T>(string path) where T : class
        {
            TextAsset targetFile = Resources.Load<TextAsset>(path);

            T data = null;

            if (targetFile != null)
            {
                string dataAsJson = targetFile.text;

                data = JsonUtility.FromJson<T>(dataAsJson);
            }
            else
            {
                Debug.LogError("Cannot find data file!");
            }

            return data;
        }

        // get local country
        public string GetCountry()
        {
            if (!PlayerPrefs.HasKey(Config.key_Country))
            {
                string Country;
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Korean:
                        Country = "kor";
                        break;
                    default:
                        Country = "eng";
                        break;
                }
                SetStrData(Config.key_Country, Country);
            }
            return GetStrData(Config.key_Country, "kor");
        }

        #region Default Info
        // create default game object
        public static GameObject CreateDataObject<T>() where T : MonoBehaviour
        {
            var obj = new GameObject();

            obj.AddComponent<T>();

            DontDestroyOnLoad(obj);

            obj.transform.SetParent(instance.transform);
            obj.name = typeof(T).Name;

            obj.SetActive(false);

            return obj;
        }

        public GameObject GetDefaultController()
        {
            return defaultController;
        }

        public GameObject GetDefaultHero()
        {
            return defaultHero;
        }
        #endregion

        #region HeroInfo
        public void DeleteHeroInfo(int index)
        {
            string HeroTag = "Hero_" + index;

            SetIntData(HeroTag, 0);
        }

        public void SetHeroInfo(HeroInfo info, int index)
        {
            string HeroTag = "Hero_" + index;

            SetIntData(HeroTag, 1);
            SetStrData(HeroTag + "_name", info.name);

            for (int i = 0; i < info.soldiers.Length; ++i)
            {
                SetIntData(HeroTag + "_Soldier_" + i, info.soldiers[i]);
            }
        }

        public HeroInfo GetHeroInfo(int index)
        {
            string HeroTag = "Hero_" + index;

            // has valid data?
            if (GetIntData(HeroTag, 0) == 0) return null;

            HeroInfo info = new HeroInfo();

            info.name = GetStrData(HeroTag + "_name");

            for (int i = 0; i < info.soldiers.Length; ++i)
            {
                info.soldiers[i] = GetIntData(HeroTag + "_Soldier_" + i);
            }

            return info;
        }
        #endregion // heroInfo

        #region Loading Info
        public void SetLoadingInfo( int scene , int stage )
        {
            SetIntData("Loading_Scene", scene);
            SetIntData("Loading_Stage", stage);
        }

        public void GetLoadingInfo( out int scene, out int stage )
        {
            scene = GetIntData("Loading_Scene", 0);
            stage = GetIntData("Loading_Stage", -1);
        }
        #endregion

        #region Soldiers
        public static Soldier CreateSoliderById( int id )
        {
            // if has id
            if (!defaultSoldiers.ContainsKey(id)) return null;

            // get prob
            GameObject defaultProb = defaultSoldiers[id];

            if( defaultProb != null )
            {
                // create new
                var obj = ObjectPoolManager.instance.Pop(defaultProb);

                if( obj != null )
                {
                    return obj.GetComponent<Soldier>();
                }
            }

            return null;
        }
        #endregion // soldiers

        #region get/set int,float,string data
        public string GetStrData(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public int GetIntData(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public float GetFloatData(string key, float defaultValue = 0.0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public void SetStrData(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void SetIntData(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public void SetFloatData(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
        #endregion
    }
}
