using BebiAnimations.Libs.Core;
using UnityEngine.Events;

namespace BebiAnimations.Libs.Actions
{
    public class Action_FunctionCaller : AnimationAction
    {
        public UnityEvent Event;

        protected override void ActionPlay()
        {
            Event?.Invoke();
        }
    }
}
