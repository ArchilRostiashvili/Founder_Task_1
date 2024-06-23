using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class AnimTrigger : MonoBehaviour
    {
        public Action CallBack;
        public void Trigger_Animation_Finish()
        {
            this.CallBack?.Invoke();
        }
    }
}
