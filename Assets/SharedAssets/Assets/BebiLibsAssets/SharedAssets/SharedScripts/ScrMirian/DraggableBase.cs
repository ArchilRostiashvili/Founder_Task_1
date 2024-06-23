using UnityEngine;

namespace Libs_Mirian
{
    public class DraggableBase : InteractableBase
    {
        protected void Drag()
        {
            this.transform.position = base.touchPosition - base.touchOffset;
        }

        protected virtual bool DragCondition()
        {
            return true;
        }

        protected virtual void OnDragConditionMet()
        {
            this.Drag();
        }

        protected virtual void OnDragConditionNotMet()
        {
        }

        public override void OnInteractMove(Vector2 touchPosition)
        {
            base.OnInteractMove(touchPosition);

            if (this.DragCondition())
            {
                this.OnDragConditionMet();
            }
            else
            {
                this.OnDragConditionNotMet();
            }
        }     
    }
}