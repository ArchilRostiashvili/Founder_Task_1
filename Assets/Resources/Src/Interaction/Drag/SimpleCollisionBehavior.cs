using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FarmLife.Controllers.Drag
{
    public class SimpleCollisionBehavior : MonoBehaviour, ICollisionBehavior
    {
        private DragTargetBehavior _correctTarget;

        public void CollisionFinish(DragBehavior dragBehavior, List<DragTargetBehavior> dragTargetBehaviors,
         Action<DragBehavior> AfterCheckEvent = null, Action<DragBehavior, DragTargetBehavior> AfterCheckCorrectEvent = null)
        {
            bool foundCorrect = false;

            DragTargetBehavior dragTargetBehavior = null;

            for (int i = 0; i < dragBehavior.TouchingColliderList.Count; i++)
            {
                dragTargetBehavior = dragBehavior.TouchingColliderList[i].GetComponent<DragTargetBehavior>();

                if (dragTargetBehavior == null)
                    continue;

                if (dragTargetBehavior.ID != dragBehavior.ID)
                    continue;

                foundCorrect = true;
                break;
            }

            if (foundCorrect)
                dragTargetBehavior.Take(dragBehavior, AfterCheckCorrectEvent);
            else
            {
                if (dragTargetBehavior != null)
                    dragTargetBehavior.PlayWrong();

                OnIncorrectlyDragged(dragBehavior, () => AfterCheckEvent?.Invoke(dragBehavior));
            }
        }

        private void OnIncorrectlyDragged(DragBehavior dragBehavior, Action afterAnimationEvent = null)
        {
            dragBehavior.Wrong(() =>
            {
                afterAnimationEvent?.Invoke();
            });
        }

        public void CollisionEnter(DragBehavior dragBehavior, List<DragTargetBehavior> dragTargetBehaviors)
        {
            dragBehavior.CheckDraggableColliderOverlap();
            GetClosestPlace(dragBehavior, dragTargetBehaviors);
            for (int i = 0; i < dragTargetBehaviors.Count; i++)
            {
                if (dragBehavior.TouchingColliderList.Count > 0)
                {
                    if (!dragBehavior.TouchingColliderList.Contains(dragTargetBehaviors[i].Collider))
                    {
                        dragTargetBehaviors[i].HighlightOff();
                    }
                }
                else
                {
                    dragTargetBehaviors[i].HighlightOff();
                }
            }
        }

        public void CollisionExit(DragBehavior dragBehavior, List<DragTargetBehavior> dragTargetBehaviors)
        {
            for (int i = 0; i < dragTargetBehaviors.Count; i++)
            {
                dragTargetBehaviors[i].HighlightOff();
            }
        }

        private void GetClosestPlace(DragBehavior dragBehavior, List<DragTargetBehavior> targetList)
        {
            if (dragBehavior.TouchingColliderList.Count < 1)
            {
                return;
            }

            DragTargetBehavior correctPlace;

            CheckForCorrectOverlap(dragBehavior);

            if (_correctTarget != null)
            {
                if (dragBehavior.TouchingColliderList.Contains(_correctTarget.Collider))
                {
                    correctPlace = _correctTarget;
                }
                else
                {
                    correctPlace = CheckForClosest(dragBehavior);
                }
            }
            else
            {
                correctPlace = CheckForClosest(dragBehavior);
            }

            if (correctPlace == null)
            {
                return;
            }

            correctPlace.Highlight();

            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] != correctPlace)
                {
                    targetList[i].HighlightOff();
                }
            }
        }

        public DragTargetBehavior CheckForClosest(DragBehavior dragBehavior)
        {
            Collider2D correctPlace = dragBehavior.TouchingColliderList[0];
            float minDistance = float.MaxValue;
            for (int i = 0; i < dragBehavior.TouchingColliderList.Count; i++)
            {
                float comparableDistance = Vector3.Distance(dragBehavior.transform.position, dragBehavior.TouchingColliderList[i].transform.position);

                if (comparableDistance < minDistance)
                {
                    minDistance = comparableDistance;
                    correctPlace = dragBehavior.TouchingColliderList[i];
                }
            }
            return correctPlace.GetComponent<DragTargetBehavior>();
        }

        public bool CheckForCorrectOverlap(DragBehavior dragBehavior)
        {
            for (int i = 0; i < dragBehavior.TouchingColliderList.Count; i++)
            {
                DragTargetBehavior target = dragBehavior.TouchingColliderList[i].GetComponent<DragTargetBehavior>();

                if (target == null)
                {
                    continue;
                }

                if (target.ID == dragBehavior.ID)
                {
                    _correctTarget = target;
                    return true;
                }
            }

            _correctTarget = null;
            return false;
        }
    }
}