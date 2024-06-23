using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO!!! DYNAMIC PARAMETERS
// TODO!!! IMPLEMENT NEW AUDIO SYSTEM
// TODO!!! 0-1 RANGE FOR TIMING
// TODO!!! AnimationCurve/Ease SELECTION OPTION

public abstract class ObjectAnimatorBase : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public float AnimationTime;

    public abstract void Play(Transform owner, System.Action onComplete = null);
    public abstract void Stop(Transform owner);
}
