using System.Collections.Generic;
using DG.Tweening;
using FarmLife;
using UnityEngine;

namespace FarmLife.MiniGames.TractorFruit
{
    public class ItemTractor : MonoBehaviour
    {
        [SerializeField] private List<ItemBox> _itemBoxesList = new List<ItemBox>();

        [SerializeField] private List<Transform> _boxPointsList = new List<Transform>();

        [SerializeField] private Animator _animator;

        private int _boxCounter;
        private int _pointCounter = 1;

        private ItemBox _currentItemBox;

        private static readonly int Action = Animator.StringToHash("Action");

        public void SetData(Sprite sprite, int index, string ID)
            => _itemBoxesList[index].SetData(sprite, ID);


        public void Correct()
        {
            _currentItemBox.Correct();
        }

        public void BoxFinishAnim()
        {
            DOTween.Sequence()
                .Append(_currentItemBox.ParentTransform.DOLocalJump(_currentItemBox.ParentTransform.localPosition, 2f, 0, 0.2f).SetEase(Ease.OutSine))
                .Append(_currentItemBox.transform.DOScale(new Vector3(2.5f, 2.3f, 1f), 0.1f))
                .Append(_currentItemBox.transform.DOScale(new Vector3(2.35f, 2.45f, 1f), 0.1f))
                .Append(_currentItemBox.transform.DOScale(new Vector3(2.4f, 2.4f, 1f), 0.1f))
                .Play();
        }

        public void SetCorrectBox()
            => _currentItemBox = _itemBoxesList[_boxCounter];

        public void ChangeBox()
        {
            _currentItemBox.ParentTransform.DOJump(_boxPointsList[_pointCounter].position, 0.5f, 0, 0.5f);

            _boxCounter++;
            _pointCounter++;

            if (_boxCounter < _itemBoxesList.Count)
            {
                SetCorrectBox();
                _itemBoxesList[_boxCounter].ParentTransform.DOJump(_boxPointsList[0].position, 0.5f, 0, 0.5f);
            }

            Move();
        }

        public ItemBox CorrectItemBox(Sprite sprite)
        {
            return _itemBoxesList.Find(x => x.IsCorrectBox(sprite));
        }

        public void Move()
        {
            transform.DOLocalMoveX(transform.position.x + 3.5f, 1f).SetDelay(0.5f);
        }
    }
}