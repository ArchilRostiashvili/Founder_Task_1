using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmLife.MiniGames.SortingOfEggs
{
    public class DraggableEggHolder : MonoBehaviour
    {
        [SerializeField] private DraggableEgg _draggableEgg;
        [SerializeField] private Transform _movableTransform;
        [SerializeField] private Transform _staticTransform;

        public DraggableEgg DraggableEgg => _draggableEgg;

        public Transform MovableTransform => _movableTransform;
        public Transform StaticTransform => _staticTransform;
    }
}