using System;
using BebiLibs.AudioSystem;
using UnityEngine;
using UnityEngine.Events;

namespace FarmLife
{
    public class InteractableObject : MonoBehaviour
    {
        public int Priority => _priority;

        [Header("General")][SerializeField] private bool _isSingleUse;
        [SerializeField] private bool _playSoundOnSpawn;
        [SerializeField] private int _priority;

        [Header("Particles")][SerializeField] private GameObject _particleToPlay;
        [SerializeField] private Transform _particleSpawnLocation;
        [SerializeField] private bool _parentParticleOnSpawn;

        [Header("Audio")][SerializeField] private AudioTrackBaseSO _audioToPlay;
        [SerializeField] private bool _audioHasDelay;

        [Header("Animator")][SerializeField] private Animator _animator;
        [SerializeField] private bool _useAnimator;
        [SerializeField] private float _interactedAnimationSpeed;
        [SerializeField] private float _interactionTimer;

        [Header("Interaction Events")]
        [SerializeField]
        private float _eventInteractionDelay;

        public UnityEvent OnInteract;

        private bool _interactionEventsDisabled;
        private bool _alreadyInteracted;
        private float _currentInteractionTimer;
        private float _lastInteractionTime;
        private float _cachedInteractionTimer;

        private void Awake()
        {
            _lastInteractionTime = -_eventInteractionDelay - 1;
            _cachedInteractionTimer = _interactedAnimationSpeed;
        }

        private void OnEnable()
        {
            if (_playSoundOnSpawn)
            {
                PlaySoundOnSpawn();
            }
        }

        public void Interact(Vector3 interactionPosition)
        {
            if (_isSingleUse && _alreadyInteracted)
                return;

            if (_particleToPlay != null)
            {
                interactionPosition = new Vector3(interactionPosition.x, interactionPosition.y, 0);

                GameObject spawnedParticle = null;
                if (_particleSpawnLocation)
                    spawnedParticle = Instantiate(_particleToPlay, _particleSpawnLocation.position,
                        _particleSpawnLocation.rotation);
                else
                    spawnedParticle = Instantiate(_particleToPlay, interactionPosition, Quaternion.identity);

                if (_parentParticleOnSpawn)
                    spawnedParticle.transform.SetParent(transform);
            }

            if (_audioToPlay != null)
            {
                if (_audioHasDelay)
                {
                    if (Time.time - _lastInteractionTime > _eventInteractionDelay)
                    {
                        _audioToPlay.Play();
                    }
                }
                else
                {
                    _audioToPlay.Play();
                }
            }

            if (Time.time - _lastInteractionTime > _eventInteractionDelay)
            {
                if (_interactionEventsDisabled)
                    return;
                OnInteract?.Invoke();
                _lastInteractionTime = Time.time;
            }

            if (_useAnimator)
            {
                _animator.SetFloat("Speed", _interactedAnimationSpeed);
                _currentInteractionTimer = _interactionTimer;
            }

            if (_isSingleUse)
                _alreadyInteracted = true;
        }

        private void Update()
        {
            if (!_useAnimator)
                return;

            if (_alreadyInteracted && _interactedAnimationSpeed > 1)
            {
                _interactedAnimationSpeed -= 2 * Time.deltaTime;
                _animator.SetFloat("Speed", _interactedAnimationSpeed);

                _currentInteractionTimer -= Time.deltaTime;

                if (_interactedAnimationSpeed <= 1)
                {
                    _animator.SetFloat("Speed", 1f);
                    _alreadyInteracted = false;
                    _interactedAnimationSpeed = _cachedInteractionTimer;
                }
            }

        }

        public void ResetInteractable()
        {
            _alreadyInteracted = false;
        }

        private void PlaySoundOnSpawn()
        {
            if (_audioToPlay != null)
                _audioToPlay.Play();
        }

        public void DisableInteractionEvents() => _interactionEventsDisabled = true;
        public void EnableInteractionEvents() => _interactionEventsDisabled = false;
    }
}