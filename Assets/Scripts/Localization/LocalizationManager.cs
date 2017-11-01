using System.Collections.Generic;
using UnityEngine;
using JGame.Data;

namespace JGame.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        // instance
        private static LocalizationManager _instance = null;
        public static LocalizationManager instance
        {
            get
            {
                if (_instance == null) CreateInstance();

                return _instance;
            }
        }

        // localization data
        private Dictionary<string, string> localizedText;

        private bool isReady = false;
        
        protected static void CreateInstance()
        {
            if (_instance != null) return;

            GameObject obj = new GameObject();

            if (obj != null)
            {
                obj.name = "Localization Manager";
                _instance = obj.AddComponent<LocalizationManager>();
                _instance.Initialize();
                DontDestroyOnLoad(obj);
            }
        }

        // Initialize
        public void Initialize()
        {
            LoadLocalizedText("Localize/"+GetCountry());
        }

        // load localization data file
        public void LoadLocalizedText(string fileName)
        {
            LocalizationData loadedData = DataController.LoadJson<LocalizationData>(fileName);

            if( loadedData != null )
            {
                localizedText = new Dictionary<string, string>();

                for (int i = 0; i < loadedData.items.Length; i++)
                {
                    localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
                }
            }

            isReady = true;
        }

        // get localized text
        public string GetLocalizedValue(string key)
        {
            string result = key;
            if (localizedText.ContainsKey(key))
            {
                result = localizedText[key];
            }

            return result;

        }

        // data file loaded?
        public bool GetIsReady()
        {
            return isReady;
        }

        // get country for localization
        public string GetCountry()
        {
            // from DataController
            return DataController.instance.GetCountry();
        }
    }
}
