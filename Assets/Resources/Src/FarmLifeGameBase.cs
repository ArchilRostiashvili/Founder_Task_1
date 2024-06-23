using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bebi.FarmLife
{
    public class FarmLifeGameBase : MonoBehaviour
    {
        public System.Action OnGameExitRequestedEvent;

        public void OnExitSliderSwipe()
        {
            OnGameExitRequestedEvent?.Invoke();
        }
    }
}