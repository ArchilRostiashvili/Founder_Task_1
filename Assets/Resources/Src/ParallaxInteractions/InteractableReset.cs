using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace FarmLife
{
    public class InteractableReset : MonoBehaviour
    {
        [SerializeField] private InteractableObject _interactable;
        [SerializeField] private FallingObject _fallingObject;
        [SerializeField] private SpriteRenderer _alphaRenderer;
        [SerializeField] private Transform _objectTransform;
        [SerializeField] private float _scaleResetDuration;
        [SerializeField] private bool _playScaleReset;
        [SerializeField] private bool _resetLocalPosition;
        [SerializeField] private bool _resetRendererAlpha;

        private Vector3 _startingLocalPosition;
        private Vector3 _startingLocalScale;
        private Quaternion _startingRotation;

        private void Awake()
        {
            _startingLocalScale = _objectTransform.localScale;
            _startingLocalPosition = _objectTransform.localPosition;
            _startingRotation = _objectTransform.rotation;
        }

        public void ResetObject(float delay = 0)
        {
            if (delay > 0)
            {
                StartCoroutine(ResetObjectWithDelay(delay));
                return;
            }

            if (_resetRendererAlpha)
                _alphaRenderer.SetAlpha(1f);
            if (_resetLocalPosition)
                _objectTransform.localPosition = _startingLocalPosition;

            if (_playScaleReset)
            {
                _objectTransform.localScale = Vector3.zero;
                _objectTransform.DOScale(_startingLocalScale, _scaleResetDuration);
                _objectTransform.rotation = _startingRotation;
            }

            if (_interactable != null)
            {
                _interactable.ResetInteractable();
            }

            if (_fallingObject != null)
            {
                _fallingObject.ResetFallingObject();
            }
        }

        private IEnumerator ResetObjectWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_resetRendererAlpha)
                _alphaRenderer.SetAlpha(1f);
            if (_resetLocalPosition)
                _objectTransform.localPosition = _startingLocalPosition;

            if (_playScaleReset)
            {
                _objectTransform.localScale = Vector3.zero;
                _objectTransform.DOScale(_startingLocalScale, _scaleResetDuration);
                _objectTransform.rotation = _startingRotation;
            }

            if (_interactable != null)
            {
                _interactable.ResetInteractable();
            }

            if (_fallingObject != null)
            {
                _fallingObject.ResetFallingObject();
            }
        }
    }
}