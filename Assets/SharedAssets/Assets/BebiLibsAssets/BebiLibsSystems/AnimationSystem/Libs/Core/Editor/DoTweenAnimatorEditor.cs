using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiAnimations.Libs.Core
{
    /*
#if UNITY_EDITOR
    [CustomEditor(typeof(Anim_Scale), true)]
    public class DoTweenAnimatorEditor : AnimationActionEditor
    {
        private Anim_Scale _doTweenAnimator;
        private Editor _tweenAction;
        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;
            _doTweenAnimator = (Anim_Scale)target;
            _tweenAction = Editor.CreateEditor(_doTweenAnimator.TweenAction);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_doTweenAnimator.TweenAction != null)
            {
                EditorGUILayout.LabelField("Action Parameters: ");
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                if (_tweenAction != null)
                {
                    GUI.enabled = false;
                    _tweenAction.DrawDefaultInspector();
                    GUI.enabled = true;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
        }
    }
#endif
    */
}
