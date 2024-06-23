using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(BalloonSystem))]
public class BalloonSystemEditor : Editor
{
    private BalloonSystem _balloonSystem;

    private void OnEnable()
    {
        _balloonSystem = (BalloonSystem)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Activate"))
        {
            _balloonSystem.Activate();
        }
    }
}
#endif