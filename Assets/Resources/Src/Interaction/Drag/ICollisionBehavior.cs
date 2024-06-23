using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
namespace FarmLife.Controllers.Drag
{
    public interface ICollisionBehavior
    {
        void CollisionEnter(DragBehavior dragBehavior, List<DragTargetBehavior> dragTargetBehaviors);
        void CollisionExit(DragBehavior dragBehavior, List<DragTargetBehavior> dragTargetBehaviors);
        void CollisionFinish(DragBehavior dragBehavior, List<DragTargetBehavior> dragTargetBehaviors,
        Action<DragBehavior> IncorrectCheckEvent = null, Action<DragBehavior, DragTargetBehavior> AfterCheckCorrectEvent = null);
    }
}
