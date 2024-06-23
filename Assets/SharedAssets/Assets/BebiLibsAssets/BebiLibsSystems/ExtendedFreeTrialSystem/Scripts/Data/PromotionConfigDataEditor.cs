using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace BebiLibs.ExtendedFreeTrialSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PromotionConfigData))]
    public class PromotionConfigDataEditor : Editor
    {
        private PromotionConfigData _promotionConfigData;

        private void OnEnable()
        {
            _promotionConfigData = (PromotionConfigData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save Json To Memory"))
            {
                _promotionConfigData.SaveIntoMemory();
            }

            if (GUILayout.Button("Load Json From Memory"))
            {
                _promotionConfigData.SaveIntoMemory();
            }

            if (GUILayout.Button("Print Remote Value"))
            {
                Debug.LogWarning(_promotionConfigData.PrintPromotionDataJson());
            }

            if (GUILayout.Button("Print Default Value"))
            {
                Debug.LogWarning(_promotionConfigData.GetDefaultDataAsJsonString());
            }
        }
    }
#endif
}
