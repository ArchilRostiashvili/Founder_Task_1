using System.Collections;
using System.Collections.Generic;
using Bebi.FarmLife;
using BebiLibs.AudioSystem;
using FarmLife.Controllers.Drag;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FarmLife
{
    public class AnimalTarget : MonoBehaviour
    {
        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private DragTargetBehavior _dragTargetBehavior;
        [SerializeField] private ItemLabel _itemLabel;
        [SerializeField] private Transform _animalTransform;
        [SerializeField] private Transform _boxTransform;

        private float _scaleUpValue;
        private AudioTrackBaseSO _correctAudioTrack;
        private FarmAnimalAnimator _farmAnimalAnimator;
        private Animator _animalAnimator;

        public Transform BoxTransform => _boxTransform;
        public DragTargetBehavior DragTarget => _dragTargetBehavior;

        public void SpawnMecanim()
        {
            _farmAnimalAnimator = Instantiate(_farmAnimalAnimator, _animalTransform);
            _farmAnimalAnimator.transform.localScale = _farmAnimalAnimator.transform.localScale * _scaleUpValue;

            Collider2D animalCollider = _farmAnimalAnimator.GetComponent<Collider2D>();

            if (animalCollider != null)
                animalCollider.enabled = false;

            CheckForInteractableObject();

            _feelAnimator.SetTransform(_farmAnimalAnimator.transform, AnimationNamesData.ANIM_SHOW);
            _feelAnimator.Play(AnimationNamesData.ANIM_SHOW);
        }

        private void CheckForInteractableObject()
        {
            InteractableObject interactableObject = _farmAnimalAnimator.GetComponent<InteractableObject>();

            if (interactableObject == null)
                return;

            interactableObject.OnInteract.RemoveAllListeners();
            interactableObject.OnInteract.AddListener(_farmAnimalAnimator.SetExcited);
        }

        public AudioTrackBaseSO CorrectAudioTrack => _correctAudioTrack;
        public void Show()
        {
            _itemLabel.Show();
        }

        public void SetData(Sprite correctSprite, string itemID = null, FarmAnimalAnimator farmAnimalAnimator = null, float scaleUpValue = 1)
        {
            _scaleUpValue = scaleUpValue;
            _itemLabel.SetData(correctSprite);
            _dragTargetBehavior.SetID(itemID);
            _farmAnimalAnimator = farmAnimalAnimator;
        }

        public void PlayAnimation(string animationID)
        {
            if (animationID == null)
            {
                _farmAnimalAnimator.SetExcited();
            }
            else
            {
                _farmAnimalAnimator.FarmAnimator.SetTrigger(animationID);
            }
        }
    }
}