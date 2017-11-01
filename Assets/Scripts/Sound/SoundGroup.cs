using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Sound
{
    [System.Serializable]
    public enum ESoundGroup
    {
        Master,
        Effect,
        BGM,
    }

    // sound group class
    class SoundGroup
    {
        // group enum
        public ESoundGroup group = ESoundGroup.Master;

        // volume
        protected float volume = 1.0f;

        // parent group
        public SoundGroup _parent = null;

        // parent
        public SoundGroup parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (value != this)
                {
                    _parent = value;
                }
            }
        }

        // get volume
        public float GetVolume(bool inheritance = false)
        {
            if (parent == null || inheritance == false )
            {
                return volume;
            }

            return volume * parent.GetVolume(inheritance);
        }

        // set volume
        public void SetVolume(float newVolume)
        {
            volume = newVolume;

            PlayerPrefs.SetFloat(GetVolumeDataKey(), volume);
        }

        // initialize
        public bool Initialize(ESoundGroup sndGroup)
        {
            group = sndGroup;

            volume = PlayerPrefs.GetFloat(GetVolumeDataKey(), 1.0f);

            return true;
        }

        // get volume data key
        public string GetVolumeDataKey()
        {
            return "Volume_" + group.ToString();
        }
    }
}