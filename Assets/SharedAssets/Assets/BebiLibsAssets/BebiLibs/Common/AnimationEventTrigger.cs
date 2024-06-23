using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs
{
    public class AnimationEventTrigger : MonoBehaviour
    {
        [SerializeField] private List<UnityEvent> _eventsList = new List<UnityEvent>();

        public void InvokeEvent(int eventIndex)
        {
            if (eventIndex >= 0 && eventIndex < _eventsList.Count)
            {
                _eventsList[eventIndex].Invoke();
            }
            else
            {
                Debug.LogError("Event index out of range");
            }
        }
    }
}
