using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Sound
{
    class SoundManager : MonoBehaviour
    {
        #region instance
        private static SoundManager _instance = null;
        public static SoundManager instance
        {
            get
            {
                if (_instance == null) CreateInstance();

                return _instance;
            }
        }
        #endregion

        // max audio components.
        private static readonly int MAX_Audio = 10;

        // sound groups
        protected Dictionary<ESoundGroup, SoundGroup> soundGroup = new Dictionary<ESoundGroup, SoundGroup>();

        // playing sound item count
        protected Dictionary<int, List<AudioObject>> playingList = new Dictionary<int, List<AudioObject>>();

        // all audio objects
        protected List<AudioObject> cachedAuidoList = new List<AudioObject>();

        // audio objects what can use
        protected Stack<AudioObject> audioPool = new Stack<AudioObject>();

        // create instance
        protected static void CreateInstance()
        {
            if (_instance != null) return;

            _instance = CustomUtility.CreateDontDestroyObject<SoundManager>();

            if (_instance != null)
            {
                _instance.Initialize();
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        // initialize
        protected void Initialize()
        {
            // create sound groups and initialize
            foreach (ESoundGroup group in System.Enum.GetValues(typeof(ESoundGroup)))
            {
                soundGroup.Add(group, new SoundGroup());
                if (soundGroup[group] != null)
                {
                    soundGroup[group].Initialize(group);
                    //{{@임시
                    soundGroup[group].parent = soundGroup[ESoundGroup.Master];
                    //}}@임시
                }
            }

            // create audio 
            for (int i = 0; i < MAX_Audio; ++i)
            {
                CreateAudioObject();
            }

            DontDestroyOnLoad(gameObject);
        }

        public void StopAll()
        {
            foreach (var ao in cachedAuidoList)
            {
                if (ao != null)
                {
                    ao.StopPlay();
                }
            }

            playingList.Clear();
        }

        // play sound
        public bool PlaySound(SoundItem snd)
        {
            if (snd == null)
            {
                return false;
            }

            // get audio
            AudioObject ao = GetAudioObject(snd);

            if (ao == null)
            {
                return false;
            }

            // play
            ao.PlaySound(snd);

            return true;
        }

        // get/set volume
        public float GetVolume(ESoundGroup group, bool inheritance = false )
        {
            return soundGroup[group].GetVolume(inheritance);
        }

        public void SetVolume(ESoundGroup group, float volume)
        {
            soundGroup[group].SetVolume(volume);
        }
        
        // callback on finished audio play
        public void cbAudioFinished(AudioObject ao)
        {
            audioPool.Push(ao);

            int key = ao.soundItem.GetInstanceID();

            if (!playingList.ContainsKey(key))
            {
                playingList.Add(key, new List<AudioObject>());
            }
            
            playingList[key].Remove(ao);
        }

        // callback on audio play
        public void cbAudioPlay(AudioObject ao)
        {
            int key = ao.soundItem.GetInstanceID();

            if (!playingList.ContainsKey(key))
            {
                playingList.Add(key, new List<AudioObject>());
            }
            playingList[key].Add(ao);
        }

        // get audio object
        protected AudioObject GetAudioObject(SoundItem snd)
        {
            int key = snd.GetInstanceID();

            if (!playingList.ContainsKey(key))
            {
                playingList.Add(key, new List<AudioObject>());
            }

            if (playingList[key].Count > 0 && playingList[key].Count >= snd.maxPlay)
            {
                return playingList[key][0];
            }

            // create
            if (cachedAuidoList.Count < MAX_Audio)
            {
                CreateAudioObject();
            }

            // pop
            if (audioPool.Count > 0)
            {
                return audioPool.Pop();
            }

            return null;
        }

        // create audio object
        private void CreateAudioObject()
        {
            var AO = CustomUtility.CreateDontDestroyObject<AudioObject>();

            AO.del_OnFinishedPlay = cbAudioFinished;
            AO.del_OnAudioPlay = cbAudioPlay;
            AO.transform.parent = transform;
            AO.gameObject.SetActive(false);
            cachedAuidoList.Add(AO);
            audioPool.Push(AO);
        }
    }
}