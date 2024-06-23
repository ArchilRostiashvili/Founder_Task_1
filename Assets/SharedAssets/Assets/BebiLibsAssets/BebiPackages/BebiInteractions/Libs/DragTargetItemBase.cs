using BebiLibs;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace BebiInteractions.Libs
{
    public enum ColliderType {COLLIDER, DISTANCE };
    public class DragTargetItemBase : InteractableItemBase
    {
        public bool IsHighlighted { get; protected set; }

        public Transform TargetPointTR;

        private float _collisionDistance;
        private ColliderType _colliderType;


        public virtual bool CheckCollision(DragItemBase dragItem)
        {
            if (!_isEnabled) return false;

            if (_colliderType == ColliderType.DISTANCE)
            {
                if (Vector3.Distance(TargetPointTR.position, dragItem.ContentTR.position) <= _collisionDistance)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public virtual bool CheckMatch(DragItemBase dragItem)
        {
            return dragItem.ItemID == ItemID;
        }

        public virtual void Highlight(bool value)
        {
            IsHighlighted = value;
        }

        public void SetCollisionDistance(float distance)
        {
            _collisionDistance = distance;
            SetCollisionType(ColliderType.DISTANCE);
        }

        public void SetCollisionType(ColliderType type)
        {
            _colliderType = type;
        }

        public Vector3 TargetPoint
        {
            get
            {
                return TargetPointTR.position;
            }
        }

        public Vector3 TargetPointLocal
        {
            get
            {
                return TargetPointTR.localPosition;
            }
        }
    }
}