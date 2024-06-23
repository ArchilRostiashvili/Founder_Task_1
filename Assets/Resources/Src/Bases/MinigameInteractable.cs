using System;
using System.Collections;
using UnityEngine;

namespace FarmLife
{
    public class MinigameInteractable : MinigameInteractableBase
    {
        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private float _delay;

        [SerializeField] private float _delayBetweenInteractions;

        [SerializeField] private bool _playAudioSource;

        private float _previousInteractTime;

        private bool _isReadyToInteract;

        public void SetData(AudioClip data)
        {
            if (_audioSource != null && data != null)
            {
                _audioSource.clip = data;
            }

            if (_interactOnInitialization)
            {
                StartCoroutine(PlayInteraction());
            }

            SetClickEnabled(true);
        }

        public void Interact()
        {
            StartCoroutine(PlayInteraction());
        }

        protected override void Update()
        {
            base.Update();

            if (!_isReadyToInteract)
            {
                if (Time.time - _previousInteractTime > _delayBetweenInteractions)
                {
                    _isReadyToInteract = true;
                }
            }
        }

        protected override void TapDetected(MinigameInteractable minigameInteractable)
        {
            if (minigameInteractable != null)
            {
                if (_isReadyToInteract)
                {
                    OnInteract?.Invoke();

                    if (_playAudioSource)
                    {
                        if (!_audioSource.isPlaying)
                        {
                            _audioSource.Play();
                        }
                    }

                    _previousInteractTime = Time.time;
                    _isReadyToInteract = false;
                }
            }
        }

        private IEnumerator PlayInteraction()
        {
            yield return new WaitForSeconds(_delay);

            if (!_isReadyToInteract) yield break;

            _previousInteractTime = Time.time;
            _isReadyToInteract = false;

            OnInteract?.Invoke();
        }

        public void Show(Action callback = null)
        {
            if (_feelAnimator != null)
            {
                _feelAnimator.Play(AnimationNamesData.ANIM_SHOW, callback);
            }
        }
    }
}