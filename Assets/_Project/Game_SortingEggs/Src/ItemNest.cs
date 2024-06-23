using System;
using System.Collections.Generic;
using FarmLife.Controllers.Drag;
using FarmLife.MiniGames.SortingOfEggs;
using UnityEngine;

namespace FarmLife
{
    public class ItemNest : MonoBehaviour
    {
        [SerializeField] private List<Transform> _eggPointsList = new List<Transform>();
        [SerializeField] private ProgressBarBase progressBar;
        [SerializeField] private SpriteRenderer _nestRenderer;
        [SerializeField] private DragTargetBehavior _dragTargetBehavior;

        public DragTargetBehavior DragTargetBehavior => _dragTargetBehavior;
        private int _eggCounter;

        private int _sortingOrder;

        public void Init()
        {
            _dragTargetBehavior.CorrectTakeEvent += OnCorrectlyTake;
        }

        public void Reset()
        {
            _sortingOrder = _eggPointsList.Count;
        }

        public void SetEgg(DraggableEgg egg)
        {
            egg.SetPosition(_eggPointsList[_eggCounter], _sortingOrder);
            _eggCounter++;
            _sortingOrder--;

            progressBar.ProgressUp();
        }

        public void SetColorData(Color color)
        {
            _nestRenderer.color = color;
        }

        public void SetProgressBarCounter(float counter)
        {
            progressBar.SetData(counter);
        }

        public void SetColoredProgressBarData(Color color, float counter)
        {
            progressBar.SetData(counter, color);
        }

        public void SetID(string ID)
        {
            _dragTargetBehavior.SetID(ID);
        }

        private void OnCorrectlyTake(DragBehavior behavior)
        {
            DraggableEgg egg = behavior.GetComponent<DraggableEgg>();

            if (egg == null)
            {
                Debug.LogError(behavior.ID + " has no DraggableEgg component");
                return;
            }

            SetEgg(egg);
            egg.PlayColorAudio();
        }
    }
}