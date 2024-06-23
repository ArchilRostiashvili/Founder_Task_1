#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace BebiLibs.PurchaseSystem
{
    [CustomEditor(typeof(PurchaseHistoryData))]
    public class ReceiptDataSOEditor : Editor
    {
        private PurchaseHistoryData _receiptDataSO;

        private void OnEnable()
        {
            _receiptDataSO = (PurchaseHistoryData)target;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save Data To Memory"))
            {
                _receiptDataSO.SaveJsonModelToMemory();
            }

            if (GUILayout.Button("Load Data From Memory"))
            {
                _receiptDataSO.LoadDataFromMemory();
            }
        }
    }
}
#endif