using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BebiInteractions.Libs
{
    public class TapController : InteractionControllerBase
    {
        public List<InteractableItemBase> TapItemsList = new List<InteractableItemBase>();

        public override void Reset()
        {
            base.Reset();
            foreach (var item in TapItemsList)
            {
                item.Reset();
            }
        }

        public override void Enable(bool isEnabled)
        {
            _isEnabled = isEnabled;
            for (int i = 0; i < TapItemsList.Count; i++)
            {
                TapItemsList[i].Enable(isEnabled);
            }
        }

        protected virtual void OnTap(InteractableItemBase tapItem)
        {
            if (!_isEnabled)
            {
                return;
            }
     
            if (tapItem == null)
            {
                return;
            }
      
            if (!tapItem.IsEnabled)
            {
                return;
            }
    
            Enable(false);
            if (tapItem.ItemID == "True")
            {
                ActionEvent?.Invoke(InteractionMessage.CORRECT, tapItem, null);
            }
            else
            {
                ActionEvent?.Invoke(InteractionMessage.WRONG, tapItem, null);
            }
        }

        public override void Correct(InteractableItemBase tapItem, InteractableItemBase extraNull = null)
        {
            tapItem.Done();
            AddCorrect();
            if (!IsFinished())
            {
                Enable(true);
            }
            else
            {
                ActionEvent?.Invoke(InteractionMessage.FINISH, tapItem, extraNull);
            }
        }

        public override void Wrong(InteractableItemBase tapItem, InteractableItemBase extraNull = null)
        {
            Enable(true);
        }

        public override void GetHelp()
        {
            for (int i = 0; i < TapItemsList.Count; i++)
            {
                if (!TapItemsList[i].IsDone)
                {
                    ActionEvent?.Invoke(InteractionMessage.HELP, TapItemsList[i], null);
                    return;
                }
            }
            ActionEvent?.Invoke(InteractionMessage.HELP, null, null);
        }

        private void Update()
        {
            if (!_isEnabled) return;

            InteractableItemBase tapItem = null;
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 _wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                tapItem = HitPoint(_wp);
                OnTap(tapItem);
            }
#else
            Touch touch;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                Vector3 _wp = Camera.main.ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began)
                {
                    tapItem = HitPoint(_wp);
                    OnTap(tapItem);
                }
            }
#endif
        }

        private InteractableItemBase HitPoint(Vector2 p)
        {
            Collider2D[] array = Physics2D.OverlapPointAll(p);
            for (int i = 0; i < array.Length; i++)
            {
                InteractableItemBase tapItem = array[i].GetComponent<InteractableItemBase>();
                if (tapItem != null)
                {
                    return tapItem;
                }
                else
                {
                    tapItem = array[i].transform.parent.GetComponent<InteractableItemBase>();
                    if (tapItem != null)
                    {
                        return tapItem;
                    }
                }
            }
            return null;
        }
    }
}
