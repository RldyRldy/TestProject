using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JGame.Sound
{
    // Audio Object class
    class AudioObject : MonoBehaviour
    {
        public delegate void cbOnFinishedPlay(AudioObject audio);
        public delegate void cbOnAudioPlay(AudioObject audio);

        // is played?
        protected bool isPlayed = false;

        // playing sound item
        public SoundItem soundItem = null;

        protected AudioSource audioComp = null;
        public cbOnFinishedPlay del_OnFinishedPlay = null;
        public cbOnAudioPlay del_OnAudioPlay = null;

        private void Awake()
        {
            audioComp = GetComponent<AudioSource>();

            if (audioComp == null)
            {
                audioComp = gameObject.AddComponent<AudioSource>();
            }
        }

        // play sound
        public void PlaySound(SoundItem item)
        {
            if (isPlayed)
            {
                StopPlay();
            }

            gameObject.SetActive(true);

            soundItem = item;
            audioComp.clip = soundItem.clip;
            audioComp.Play();

            isPlayed = true;

            if (del_OnAudioPlay != null)
            {
                del_OnAudioPlay(this);
            }
        }

        // stop sound
        public void StopPlay()
        {
            if(isPlayed == false )
            {
                return;
            }

            if (audioComp != null)
            {
                audioComp.Stop();
            }

            OnFinishedPlay();

            gameObject.SetActive(false);
        }

        protected void OnFinishedPlay()
        {
            if (del_OnFinishedPlay != null)
            {
                del_OnFinishedPlay(this);
            }
            isPlayed = false;
        }

        // update
        private void Update()
        {
            if (isPlayed)
            {
                if (audioComp != null)
                {
                    if (audioComp.isPlaying == false)
                    {
                        OnFinishedPlay();
                    }
                    else
                    {
                        audioComp.volume = SoundManager.instance.GetVolume(soundItem.group,true) * soundItem.volume;
                    }
                }
            }
        }
    }
}