using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace MathGames
{
    public class ItemInteractableSortingBehaviour : MonoBehaviour
    {
        [SerializeField] int _draggingSortOrder;
        [SerializeField] int _defaultSortOrder;
        [SerializeField] int _restingSortOrder;
        [SerializeField] int _returningSortOrder;
        [SerializeField] MeshRenderer MR_MeshToSort;
        [SerializeField] List<SpriteRenderer> arrayRenderersToSort = new List<SpriteRenderer>();
        [SerializeField] List<SpriteRenderer> arrayRendererBackgroundsToSort = new List<SpriteRenderer>();
        [SerializeField] bool _useSortingGroup;
        [SerializeField] SortingGroup _sortingGroup;

        private ItemInteractable _itemInteractable;
        public int DraggingSort { get => _draggingSortOrder; set => _draggingSortOrder = value; }
        public int DefaultSort => _defaultSortOrder;
        public int ReturningSort => _returningSortOrder;
        public int RestingSort { get => _restingSortOrder; set => _restingSortOrder = value; }

        private void Awake()
        {
            _itemInteractable = this.GetComponent<ItemInteractable>();
        }

        private void OnEnable()
        {
            _itemInteractable.onDragStart.AddListener(this.OnDragStart);
        }

        private void OnDisable()
        {
            _itemInteractable.onDragStart.RemoveListener(this.OnDragStart);
        }

        public void SetStrokeActive(bool val)
        {
            foreach (SpriteRenderer renderer in this.arrayRendererBackgroundsToSort) renderer.enabled = val;
        }

        public void SetRestingSort()
        {
            if (!_useSortingGroup)
            {
                foreach (SpriteRenderer renderer in this.arrayRenderersToSort) renderer.sortingOrder = _restingSortOrder;
                foreach (SpriteRenderer renderer in this.arrayRendererBackgroundsToSort) renderer.sortingOrder = _restingSortOrder - 1;

                if (this.MR_MeshToSort != null)
                    this.MR_MeshToSort.sortingOrder = _restingSortOrder;
            }
            else
            {
                _sortingGroup.sortingOrder = _restingSortOrder;
            }
        }

        public void SetDraggingSort()
        {
            if (!_useSortingGroup)
            {
                foreach (SpriteRenderer renderer in this.arrayRenderersToSort) renderer.sortingOrder = _draggingSortOrder;
                foreach (SpriteRenderer renderer in this.arrayRendererBackgroundsToSort) renderer.sortingOrder = _draggingSortOrder - 1;

                if (this.MR_MeshToSort != null)
                    this.MR_MeshToSort.sortingOrder = _draggingSortOrder;
            }
            else
            {
                _sortingGroup.sortingOrder = _draggingSortOrder;
            }
        }

        public void SetReturningSort()
        {
            if (!_useSortingGroup)
            {
                foreach (SpriteRenderer renderer in this.arrayRenderersToSort) renderer.sortingOrder = _returningSortOrder;
                foreach (SpriteRenderer renderer in this.arrayRendererBackgroundsToSort) renderer.sortingOrder = _returningSortOrder - 1;

                if (this.MR_MeshToSort != null)
                    this.MR_MeshToSort.sortingOrder = _returningSortOrder;
            }
            else
            {
                _sortingGroup.sortingOrder = _returningSortOrder;
            }
        }

        public void SetDefaultSort()
        {
            if (!_useSortingGroup)
            {
                foreach (SpriteRenderer renderer in this.arrayRenderersToSort) renderer.sortingOrder = _defaultSortOrder;
                foreach (SpriteRenderer renderer in this.arrayRendererBackgroundsToSort) renderer.sortingOrder = _defaultSortOrder - 1;

                if (this.MR_MeshToSort != null)
                    this.MR_MeshToSort.sortingOrder = _defaultSortOrder;
            }
            else
            {
                _sortingGroup.sortingOrder = _defaultSortOrder;
            }
        }

        private void OnDragStart(ItemInteractable item)
        {
            this.SetDraggingSort();
        }
    }
}
