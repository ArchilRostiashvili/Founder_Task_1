using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorEvents : MonoBehaviour
{
    public UnityEvent CallBackAnimation;
    public void Trigger_Animation_CallBack()
    {
        this.CallBackAnimation?.Invoke();
    }
}
