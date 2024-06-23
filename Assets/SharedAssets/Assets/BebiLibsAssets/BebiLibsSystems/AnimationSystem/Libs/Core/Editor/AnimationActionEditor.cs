using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiAnimations.Libs.Core
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AnimationAction), true)]
    [CanEditMultipleObjects]
    public class AnimationActionEditor : Editor
    {
        private AnimationAction _doAction;

        protected virtual void OnEnable()
        {
            if (target == null) return;
            _doAction = (AnimationAction)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying || _doAction == null) return;

            if (GUILayout.Button("Play"))
            {
                _doAction.Play();
            }

            if (GUILayout.Button("Stop"))
            {
                _doAction.Stop();
            }
        }
    }

#endif
#if UNITY_EDITOR
#endif

}


