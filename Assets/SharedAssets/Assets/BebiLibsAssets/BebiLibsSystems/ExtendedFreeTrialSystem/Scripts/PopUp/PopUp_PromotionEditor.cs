
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BebiLibs.ExtendedFreeTrialSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PopUp_Promotion))]
    public class PopUp_PromotionEditor : Editor
    {
        private PopUp_Promotion _popUp_Promotion;

        private void OnEnable()
        {
            _popUp_Promotion = (PopUp_Promotion)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Activate"))
            {
                PopUp_Promotion.Activate(true);
            }
        }
    }
#endif
}
