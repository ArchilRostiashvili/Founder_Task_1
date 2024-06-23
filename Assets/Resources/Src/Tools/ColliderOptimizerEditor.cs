#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColliderOptimizer))]
public class ColliderOptimizerEditor : Editor
{
    private ColliderOptimizer _colliderOptimizer;

    private void OnEnable()
    {
        _colliderOptimizer = target as ColliderOptimizer;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("optimizeCollider"))
        {
            _colliderOptimizer.OptimizeCollider();
        }
    }
}
#endif