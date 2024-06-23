using BebiInteractions.Libs;

namespace Showcase
{
    public class DragGameStage_Showcase : GameStageBase_Showcase
    {
        public override void Init()
        {
            _interactionController.Reset();
            _interactionController.ActionEvent += (InteractionMessage message, InteractableItemBase dragItem, InteractableItemBase targetItem) =>
            {
                if (message == InteractionMessage.DRAG_BEGIN)
                {
                    PlayAudio(false);
                    PlayAdditionalAudio(false);
                    dragItem.GetComponent<VisualObject_Showcase>().DragBegin();
                }
                else
                if (message == InteractionMessage.WRONG)
                {
                    dragItem.GetComponent<VisualObject_Showcase>().Wrong(()=>
                    {
                        _interactionController.Wrong(dragItem, targetItem);
                    });
                }
                else
                if (message == InteractionMessage.CORRECT)
                {
                    VisualObject_Showcase visualObjectDrag = dragItem.GetComponent<VisualObject_Showcase>();
                    VisualObject_Showcase visualObjectTarget = targetItem.GetComponent<VisualObject_Showcase>();

                    BebiInteractions.Libs.DragTargetItemBase targetItemNew = (BebiInteractions.Libs.DragTargetItemBase)targetItem;
                    visualObjectDrag.SetExtraData(targetItemNew.TargetPoint, true);

                    visualObjectTarget.Correct();
                    visualObjectDrag.Correct(() =>
                    {
                        _interactionController.Correct(dragItem, targetItem);
                    });
                }
                else
                if (message == InteractionMessage.FINISH)
                {
                    Finish(() =>
                    {
                        OnCompleteEvent?.Invoke();
                    });
                }
                else
                if (message == InteractionMessage.HIGHLIGHT)
                {
                    if (dragItem != null)
                    {
                        dragItem.GetComponent<VisualObject_Showcase>().Highlight();
                    }

                    if (targetItem != null)
                    {
                        targetItem.GetComponent<VisualObject_Showcase>().Highlight();
                    }
                }
                else
                if (message == InteractionMessage.HIGHLIGHT_OFF)
                {
                    if (dragItem != null)
                    {
                        dragItem.GetComponent<VisualObject_Showcase>().Highlight_Off();
                    }

                    if (targetItem != null)
                    {
                        targetItem.GetComponent<VisualObject_Showcase>().Highlight_Off();
                    }
                }
                else
                if (message == InteractionMessage.HELP)
                {
                    if (dragItem != null && targetItem != null)
                    {
                        
                    }
                }
            };
        }

        public override void SetData(string[] dataArray, int correctCount)
        {
            _interactionController.Reset();
            _interactionController.SetData(correctCount);
            base.SetData(dataArray, correctCount);
            DragController dragController = (DragController)_interactionController;

            for (int i = 0; i < dragController.DragItemList.Count; i++)
            {
                VisualObject_Showcase visualObject = dragController.DragItemList[i].gameObject.GetComponent<VisualObject_Showcase>();
                visualObject.SetData(dataArray[i]);
            }

            for (int i = 0; i < dragController.DragTargetItemList.Count; i++)
            {
                VisualObject_Showcase visualObject = dragController.DragTargetItemList[i].gameObject.GetComponent<VisualObject_Showcase>();
                visualObject.SetData(dataArray[i]);
            }
        }
    }
}
