using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PopupManagementSystem
{
    public class PopupManager : GenericSingletonClass<PopupManager>
    {
        private static List<InstantiatedPopupData> _InstantiatedPopupList = new List<InstantiatedPopupData>();
        private static PopupManagerConfig _PopupManagerConfig;

        public static PopupManagerConfig PopupManagerConfig => GetDefaultConfig();

        protected override void OnInstanceAwake()
        {
            _dontDestroyOnLoad = true;
        }

        private static PopupManagerConfig GetDefaultConfig()
        {
            if (_PopupManagerConfig == null)
            {
                _PopupManagerConfig = PopupManagerConfig.DefaultInstance();
            }

            return _PopupManagerConfig;
        }


        public static void GetPopup<T>(Action<T> onPopupReady, Action onPopupCreationFailed = null) where T : PopUpBase
        {
            if (TryGetActivePopup(typeof(T), out InstantiatedPopupData popupData))
            {
                onPopupReady?.Invoke((T)popupData.PopupInstance);
            }
            else
            {
                Instance.StartCoroutine(Instance.InstantiatePopup<T>(onPopupReady, onPopupCreationFailed));
            }
        }

        private IEnumerator InstantiatePopup<T>(System.Action<T> onPopupReady, Action onPopupCreationFailed) where T : PopUpBase
        {
            if (PopupManagerConfig == null)
            {
                //onPopupReady?.Invoke(null);
                onPopupCreationFailed?.Invoke();
                yield break;
            }

            yield return PopupManagerConfig.InstantiatePopup(typeof(T), transform, (loadStatus, popupInstance) =>
            {
                if (loadStatus == LoadStatus.Successfull)
                {
                    popupInstance.Init();
                    var popupData = new InstantiatedPopupData(typeof(T), popupInstance);
                    _InstantiatedPopupList.Add(popupData);
                    onPopupReady?.Invoke((T)popupInstance);
                }
                else
                {
                    onPopupCreationFailed?.Invoke();
                    //Debug.LogError($"PopupManager.InstantiatePopup: Failed to instantiate popup of type {typeof(T)}");
                }
            });

            // yield return PopupManagerConfig.InstantiateAddressablePopup(typeof(T), transform, (loadStatus, popupInstance) =>
            // {
            //     if (loadStatus == LoadStatus.Successfull)
            //     {
            //         popupInstance.Init();
            //         var popupData = new InstantiatedPopupData(typeof(T), popupInstance);
            //         _InstantiatedPopupList.Add(popupData);
            //         onPopupReady?.Invoke((T)popupInstance);
            //     }
            //     else
            //     {
            //         Debug.LogError($"PopupManager.InstantiatePopup: Failed to instantiate popup of type {typeof(T)}");
            //     }
            // });

            // if (PopupManagerConfig.TryGetPopupPrefab(popupType, out PopUpBase popupPrefab))
            // {
            //     PopUpBase popUpInstance = GameObject.Instantiate(popupPrefab, Instance.transform);
            //     return true;
            // }
            // else
            // {
            //     Debug.LogWarning($"#{nameof(PopupManager)}.{nameof(GetPopup)}: Unable To Instantiate {typeof(T).Name} Popup");
            //     Debug.LogError($"#{nameof(PopupManagementSystem.PopupManager)}.{nameof(InstantiatePopup)}: Unable To Find {popupType} inside popup {nameof(PopupManagementSystem.PopupManagerConfig)}");
            //     ;
            // }
        }

        private static bool TryGetActivePopup(System.Type popupType, out InstantiatedPopupData popupData)
        {
            popupData = _InstantiatedPopupList.Find(x => x.PopupType == popupType);
            return popupData != null;
        }


    }


    internal class InstantiatedPopupData
    {
        public System.Type PopupType;
        public PopUpBase PopupInstance;

        public InstantiatedPopupData(Type popupType, PopUpBase popupInstance)
        {
            PopupType = popupType;
            PopupInstance = popupInstance;
        }
    }

}
