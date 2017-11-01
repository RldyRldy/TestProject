using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Sound
{
    // sound item.
    [System.Serializable]
    public class SoundItem : ScriptableObject
    {
        // resource 
        [SerializeField]
        public AudioClip clip = null;

        // Maximum number of simultaneous playbacks
        [SerializeField]
        public int maxPlay = 1;

        // sound group
        [SerializeField]
        public ESoundGroup group = ESoundGroup.Master;

        [SerializeField]
        public float volume = 1.0f;
    }
}