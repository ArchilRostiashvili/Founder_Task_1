using System.Collections.Generic;
using UnityEngine;


namespace BebiInteractions.Libs
{
    public class DragController : InteractionControllerBase
    {
        public List<DragItemBase> DragItemList = new List<DragItemBase>();
        public List<DragTargetItemBase> DragTargetItemList = new List<DragTargetItemBase>();

        [Header("Configurables")]
        [SerializeField] protected bool _isRestrictedToSingleDrag = true;
        [SerializeField] protected bool _scaleDownOnCorrectMatch = true;
        [SerializeField] protected float _dragDistance;
        [SerializeField] public System.Action<DragItemBase> DragCorrectEvent;


        public ColliderType colliderType;

        public override void Reset()
        {
            base.Reset();
            foreach (var item in DragItemList)
            {
                item.Reset();
            }

            foreach (var item in DragTargetItemList)
            {
                item.Reset();
                if (colliderType == ColliderType.DISTANCE)
                {
                    item.SetCollisionDistance(_dragDistance);
                }
            }
        }

        public override void Enable(bool isEnabled)
        {
            EnableEvents(isEnabled);

            foreach (var item in DragItemList)
            {
                item.Enable(isEnabled);
            }

            foreach (var item in DragTargetItemList)
            {
                item.Enable(isEnabled);
            }
        }

        private void EnableEvents(bool enable)
        {
            for (int i = 0; i < DragItemList.Count; i++)
            {
                DragItemList[i].BeginDragEvent = null;
                DragItemList[i].DragEvent = null;
                DragItemList[i].DragEndEvent = null;

                if (enable)
                {
                    DragItemList[i].BeginDragEvent += OnDragBegin;
                    DragItemList[i].DragEvent += OnDrag;
                    DragItemList[i].DragEndEvent += OnDragEnd;
                }
            }
        }

        protected virtual void OnDragBegin(DragItemBase dragItem)
        {
            ActiveAllDraggables(false, dragItem);
            ActionEvent?.Invoke(InteractionMessage.DRAG_BEGIN, dragItem, null);
        }

        protected virtual void OnDrag(DragItemBase dragItem)
        {
            TryToHighlightTarget(dragItem);
        }

        protected virtual void OnDragEnd(DragItemBase dragItem)
        {
            HighlightTarget(null, null);

            DragTargetItemBase targetItem = GetCollidedTarget(dragItem);
            if (targetItem == null || !targetItem.CheckMatch(dragItem))
            {
                ActionEvent?.Invoke(InteractionMessage.WRONG, dragItem, targetItem);
            }
            else
            {
                DragCorrectEvent?.Invoke(dragItem);
                ActionEvent?.Invoke(InteractionMessage.CORRECT, dragItem, targetItem);
            }
        }

        public override void Correct(InteractableItemBase dragItem, InteractableItemBase targetItem)
        {
            targetItem.Done();
            dragItem.Done();
            AddCorrect();
            if (!IsFinished())
            {
                Enable(true);
            }
            else
            {
                ActionEvent?.Invoke(InteractionMessage.FINISH, dragItem, targetItem);
            }
        }

        public override void Wrong(InteractableItemBase dragItem, InteractableItemBase targetItem)
        {
            Enable(true);
        }

        public override void GetHelp()
        {
            DragItemBase dragItem = null;
            for (int i = 0; i < DragItemList.Count; i++)
            {
                if (!DragItemList[i].IsDone)
                {
                    dragItem = DragItemList[i];
                    break;
                }
            }

            if (dragItem != null)
            {
                for (int i = 0; i < DragTargetItemList.Count; i++)
                {
                    if (DragTargetItemList[i].ItemID == dragItem.ItemID)
                    {
                        ActionEvent?.Invoke(InteractionMessage.HELP, dragItem, DragTargetItemList[i]);
                        return;
                    }
                }
            }

            ActionEvent?.Invoke(InteractionMessage.HELP, null, null);
        }

        protected DragTargetItemBase GetCollidedTarget(DragItemBase dragItem)
        {
            DragTargetItemBase targetItem = null;
            for (int i = 0; i < DragTargetItemList.Count; i++)
            {
                if (DragTargetItemList[i].CheckCollision(dragItem))
                {
                    targetItem = DragTargetItemList[i];
                    break;
                }
            }
            return targetItem;
        }

        private void TryToHighlightTarget(DragItemBase dragItem)
        {
            DragTargetItemBase targetItem = GetCollidedTarget(dragItem);
            HighlightTarget(targetItem, dragItem);
        }

        private void HighlightTarget(DragTargetItemBase targetItem, DragItemBase dragItem)
        {
            for (int i = 0; i < DragTargetItemList.Count; i++)
            {
                if (DragTargetItemList[i] == targetItem)
                {
                    if (!DragTargetItemList[i].IsHighlighted)
                    {
                        DragTargetItemList[i].Highlight(true);
                        ActionEvent?.Invoke(InteractionMessage.HIGHLIGHT, dragItem, DragTargetItemList[i]);
                    }
                }
                else
                {
                    if (DragTargetItemList[i].IsHighlighted)
                    {
                        DragTargetItemList[i].Highlight(false);
                        ActionEvent?.Invoke(InteractionMessage.HIGHLIGHT_OFF, dragItem, DragTargetItemList[i]);
                    }
                }
            }
        }

        protected void ActiveAllDraggables(bool isActive, InteractableItemBase exception = null)
        {
            foreach (InteractableItemBase dragItem in DragItemList)
            {
                if (exception != dragItem)
                {
                    dragItem.Enable(isActive);
                }
            }
        }
    }
}
