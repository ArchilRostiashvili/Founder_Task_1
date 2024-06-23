using BebiInteractions.Libs;

namespace Showcase
{
    public class TapGameStage_Showcase : GameStageBase_Showcase
    {
        public override void Init()
        {
            _interactionController.Reset();
            _interactionController.ActionEvent += (InteractionMessage message, InteractableItemBase dragItem, InteractableItemBase targetItem) =>
            {
                if (message == InteractionMessage.WRONG)
                {
                    PlayAudio(false);
                    PlayAdditionalAudio(false);

                    dragItem.GetComponent<VisualObject_Showcase>().Wrong(() =>
                    {
                        _interactionController.Wrong(dragItem, targetItem);
                    });
                }
                else
                if (message == InteractionMessage.CORRECT)
                {
                    PlayAudio(false);
                    PlayAdditionalAudio(false);

                    VisualObject_Showcase visualObjectDrag = dragItem.GetComponent<VisualObject_Showcase>();
                    visualObjectDrag.Correct(() =>
                    {
                        _interactionController.Correct(dragItem, null);
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
                if (message == InteractionMessage.HELP)
                {
                    if (dragItem != null)
                    {

                    }
                }
            };
        }

        public override void SetData(string[] dataArray, int correctCount)
        {
            _interactionController.Reset();
            base.SetData(dataArray, correctCount);
            _interactionController.SetData(correctCount);

            TapController tapController = (TapController)_interactionController;
            for (int i = 0; i < tapController.TapItemsList.Count; i++)
            {
                VisualObject_Showcase visualObject = tapController.TapItemsList[i].gameObject.GetComponent<VisualObject_Showcase>();
                visualObject.SetData(dataArray[i]);
            }
        }
    }
}
