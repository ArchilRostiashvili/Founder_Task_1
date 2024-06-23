using System.Collections;
using System.Collections.Generic;
using BebiLibs;
using UnityEngine;

namespace FarmLife.Controllers.Drag
{
    public class SimpleDraggable : MonoBehaviour
    {
        [SerializeField] private ItemSpriteSizer _spriteSizer;
        [SerializeField] private DragBehavior _dragBehavior;
        [SerializeField] private Transform _helperTransform;
        private Vector2 _defaultPosition;

        public DragBehavior DragBehavior => _dragBehavior;
        public Transform HelperTransform => _helperTransform == null ? transform : _helperTransform;

        public void Init()
            => _defaultPosition = transform.position;

        public void SetData(Sprite sprite, string clothesID)
        {
            _dragBehavior.SetID(clothesID);
            _spriteSizer.SetSprite(sprite);
        }

        public void Reset()
        => transform.position = _defaultPosition;
    }
}