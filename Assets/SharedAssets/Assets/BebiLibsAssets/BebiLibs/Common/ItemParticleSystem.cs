using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class ItemParticleSystem : MonoBehaviour
    {
        public ParticleSystem[] arrayParticles;

        public bool isActive;
        public bool isOn;
        public bool isPlaying;

        public float time;

        public bool persistOnParentDisable = false;
        public bool autoStopWithAnimation = false;

        public void Play()
        {
            isOn = true;
            isPlaying = false;
            gameObject.SetActive(true);

            PlayEffect();
        }

        public void Hide()
        {
            StopAllCoroutines();
            isOn = false;
            isPlaying = false;
            gameObject.SetActive(false);
        }

        private void PlayEffect()
        {
            if (isOn && isActive && !isPlaying)
            {
                isPlaying = true;
                StopAllCoroutines();
                if (0.0f < time)
                {
                    StartCoroutine(DelayHide());
                }

                for (int i = 0; i < arrayParticles.Length; i++)
                {
                    //arrayParticles[i].enableEmission = false;
                    arrayParticles[i].Stop();
                    arrayParticles[i].Clear(true);
                    arrayParticles[i].Play();
                }
            }
        }

        public void SetColor(Color color)
        {
            for (int i = 0; i < arrayParticles.Length; i++)
            {
                ParticleSystem.MainModule m = arrayParticles[i].main;
                m.startColor = color;
            }
        }

        private void OnEnable()
        {
            isActive = true;
            PlayEffect();
        }

        private void OnDisable()
        {
            if (!persistOnParentDisable)
            {
                isActive = false;
                Stop(false);
            }
        }

        private IEnumerator DelayHide()
        {
            yield return new WaitForSeconds(time);
            Stop(autoStopWithAnimation);
        }

        public void Stop(bool anim = false)
        {
            isOn = false;
            StopAllCoroutines();
            if (anim)
            {
                for (int i = 0; i < arrayParticles.Length; i++)
                {
                    arrayParticles[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }
}
