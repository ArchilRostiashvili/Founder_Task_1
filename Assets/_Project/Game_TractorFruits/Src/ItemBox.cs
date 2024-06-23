using System.Collections.Generic;
using FarmLife.Controllers.Drag;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    public class ItemBox : MonoBehaviour
    {
        [SerializeField] private DragBehavior _dragBehavior;
        [SerializeField] private Transform _boxParentTransform;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private List<SpriteRenderer> _fruitSRsList = new List<SpriteRenderer>();
        [SerializeField] private FeelAnimator _feelAnimator;
        [SerializeField] private MMF_Feedback mMF_Feedback;
        private int _correctCount;
        private Transform _finalParent;

        public DragBehavior DragBehavior => _dragBehavior;
        public Transform ParentTransform => _boxParentTransform;

        public void Correct()
        {
            if (_correctCount < _fruitSRsList.Count)
            {
                _fruitSRsList[_correctCount].gameObject.SetActive(true);
                _correctCount++;
            }

            _feelAnimator.Play("Stage1Correct");
        }

        public void SetFinalParent(Transform parent)
            => _finalParent = parent;

        public void SetData(Sprite sprite, string ID)
        {
            _spriteRenderer.sprite = sprite;
            _dragBehavior.SetID(ID);
            _correctCount = 0;

            for (int i = 0; i < _fruitSRsList.Count; i++)
            {
                _fruitSRsList[i].sprite = sprite;
            }
        }

        public bool IsCorrectBox(Sprite sprite)
        {
            if (_fruitSRsList[0].sprite == sprite)
            {
                return true;
            }

            return false;
        }

        public void SetID(string itemID)
        {
            _dragBehavior.SetID(itemID);
        }

        public void ChangeParent()
        {
            transform.parent = _finalParent;
        }
    }
}