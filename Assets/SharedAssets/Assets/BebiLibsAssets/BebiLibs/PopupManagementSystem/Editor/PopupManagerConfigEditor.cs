#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
#endif


namespace BebiLibs.PopupManagementSystem
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PopupManagerConfig))]
    public class PopupManagerConfigEditor : Editor
    {
        private PopupManagerConfig _popupManagerConfig;

        private void OnEnable()
        {
            _popupManagerConfig = (PopupManagerConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Add Popup Prefab"))
            {
                AssetSearchWindow.ShowWindow($"Select Popup prefab to add {nameof(PopupManagementSystem)}", "prefab", typeof(PopUpBase), OnItemSelect, OnWindowClose);
            }
        }

        private void OnItemSelect(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                Debug.LogError("obj is null");
                return;
            }

            GameObject gameObject = (GameObject)obj;

            PopUpBase pop = gameObject.GetComponent<PopUpBase>();
            if (pop == null)
            {
                Debug.LogError("gameObject.GetComponent<PopUpBase>() == null");
                return;
            }

            List<AddressablePopupAsset> addressablePopupAssets = _popupManagerConfig.AddressablePopupAssets;
            var addressablePopup = addressablePopupAssets.Find(x => x.PopupTypeName == pop.GetType().Name);
            if (addressablePopup != null)
            {
                Debug.LogError("Popup prefab " + obj.GetType().Name + " is already added");
                return;
            }

            AssetReferenceT<GameObject> asset = AddressableExtensions.SetAddressableGroup<GameObject>(gameObject, "PopupAssets");
            addressablePopupAssets.Add(new AddressablePopupAsset(pop.GetType().Name, new AssetReferenceGameObject(asset.AssetGUID)));
        }


        private void OnWindowClose()
        {

        }
    }


#endif
}
