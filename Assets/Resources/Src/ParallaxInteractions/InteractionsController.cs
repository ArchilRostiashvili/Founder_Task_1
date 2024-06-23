using Bebi.Helpers;
using UnityEngine;

namespace FarmLife
{
    public class InteractionsController : MonoBehaviour
    {
        [SerializeField] private WorldspaceScroll _worldspaceScroll;

        private bool _isEnabled;

        InteractableObject _tapItem1 = null;
        InteractableObject _tapItem2 = null;
        Vector3 _tapPosition = Vector3.zero;

        public void Activate() => _isEnabled = true;

        public void Stop() => _isEnabled = false;

        private void TapDetected(InteractableObject tapTarget, Vector3 tapPosition)
        {
            if (tapTarget)
                tapTarget.Interact(tapPosition);
        }

        private void Update()
        {
            if (!_isEnabled || _worldspaceScroll.IsSwipping)
                return;


            if (Input.GetMouseButtonDown(0))
            {
                Vector3 _wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _tapItem1 = HitPoint(_wp);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3 _wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _tapItem2 = HitPoint(_wp);
                _tapPosition = _wp;
            }

            if (_tapItem2 != null && _tapItem1 != null && _tapItem1.GetInstanceID() == _tapItem2.GetInstanceID())
            {
                TapDetected(_tapItem1, _tapPosition);
                _tapItem1 = null;
                _tapItem2 = null;
            }


            Touch touch1;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch1 = Input.GetTouch(i);
                Vector3 _wp = Camera.main.ScreenToWorldPoint(touch1.position);
                if (touch1.phase == TouchPhase.Began)
                {
                    _tapItem1 = HitPoint(_wp);
                }
            }

            Touch touch2;
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch2 = Input.GetTouch(i);
                Vector3 _wp = Camera.main.ScreenToWorldPoint(touch2.position);
                if (touch2.phase == TouchPhase.Ended)
                {
                    _tapItem2 = HitPoint(_wp);
                    _tapPosition = _wp;
                }
            }

            if (_tapItem2 != null && _tapItem1 != null && _tapItem1.GetInstanceID() == _tapItem2.GetInstanceID())
            {
                TapDetected(_tapItem1, _tapPosition);
                _tapItem1 = null;
                _tapItem2 = null;
            }

        }

        private InteractableObject HitPoint(Vector2 p)
        {
            Collider2D[] array = Physics2D.OverlapPointAll(p);

            InteractableObject priorityObject = null;

            int lastObjectPriority = -1;

            for (int i = 0; i < array.Length; i++)
            {
                InteractableObject tapItem = array[i].GetComponent<InteractableObject>();

                if (tapItem != null)
                {
                    if (tapItem.Priority > lastObjectPriority)
                    {
                        lastObjectPriority = tapItem.Priority;
                        priorityObject = tapItem;
                    }
                }
                else
                {
                    tapItem = array[i].transform.parent.GetComponent<InteractableObject>();

                    if (tapItem != null)
                    {
                        if (tapItem.Priority > lastObjectPriority)
                        {
                            lastObjectPriority = tapItem.Priority;
                            priorityObject = tapItem;
                        }
                    }
                }
            }

            return priorityObject;
        }
    }
}