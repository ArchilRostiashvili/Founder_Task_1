using System.Collections;
using System.Collections.Generic;
using BebiLibs.AudioSystem;
using DG.Tweening;
using FarmLife.Controllers.Drag;
using UnityEngine;

namespace FarmLife.MiniGames.SortingOfEggs
{
    public class DraggableEgg : MonoBehaviour
    {
        [SerializeField] private DragBehavior _dragBehavior;
        [SerializeField] private SpriteRenderer _eggsRenderer;

        private AudioTrackBaseSO _audioTrack = null;

        public DragBehavior DragBehavior => _dragBehavior;

        public void SetColorData(Color color, string ID, AudioTrackBaseSO audio)
        {
            _eggsRenderer.color = color;
            _dragBehavior.SetID(ID);
            _audioTrack = audio;
        }

        public void SetSizeData(Vector2 size, string ID)
        {
            transform.localScale = size;
            _dragBehavior.SetID(ID);
        }

        public void SetPosition(Transform point, int sortingOrder)
        {
            transform.parent = point;
            transform.DOLocalJump(Vector3.zero, 1f, 1, 0.25f);
            transform.DOLocalRotate(Vector3.zero, 0.25f);
            _eggsRenderer.sortingOrder = sortingOrder;
            _eggsRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }

        public void PlayColorAudio()
        {
            if (!_audioTrack)
                return;

            _audioTrack.Play();
        }

        public void SetParent(Transform parent)
        {
            transform.parent = parent;
        }
    }
}