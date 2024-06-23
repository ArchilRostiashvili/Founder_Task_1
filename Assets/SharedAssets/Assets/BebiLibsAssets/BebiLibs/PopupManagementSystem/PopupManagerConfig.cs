using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;

namespace BebiLibs.PopupManagementSystem
{
    [CreateAssetMenu(fileName = "PopupManagerConfig", menuName = "BebiLibs/PopupManagementSystem/PopupManagerConfig", order = 0)]
    public class PopupManagerConfig : ScriptableConfig<PopupManagerConfig>
    {
        [SerializeField] internal List<PopUpBase> _popUpPrefabList = new List<PopUpBase>();
        private readonly Dictionary<System.Type, PopUpBase> _popupPrefabDictionary = new Dictionary<System.Type, PopUpBase>();
        public List<PopUpBase> PopUpPrefabList => _popUpPrefabList;


        [SerializeField] private List<AddressablePopupAsset> _addressablePopupAssets = new List<AddressablePopupAsset>();
        public List<AddressablePopupAsset> AddressablePopupAssets => _addressablePopupAssets;

        public IEnumerator InstantiatePopup(System.Type popupType, Transform transform, System.Action<LoadStatus, PopUpBase> onPopupInstantiateSuccess)
        {
            bool isLoaded = _popupPrefabDictionary.TryGetValue(popupType, out PopUpBase popUpBasePrefab);
            if (isLoaded)
            {
                PopUpBase popUpInstance = GameObject.Instantiate(popUpBasePrefab, transform);
                onPopupInstantiateSuccess?.Invoke(LoadStatus.Successfull, popUpInstance);
            }
            else
            {
                onPopupInstantiateSuccess?.Invoke(LoadStatus.Failed, null);
            }
            yield break;
        }

        public IEnumerator InstantiateAddressablePopup(System.Type popupType, Transform transform, System.Action<LoadStatus, PopUpBase> onPopupInstantiateSuccess)
        {
            Debug.Log("Start Loading " + popupType);
            var asset = _addressablePopupAssets.Find(x => x.PopupTypeName == popupType.Name);

            if (asset == null) yield break;

            yield return asset.LoadAsset();

            if (asset.LoadStatus == LoadStatus.Successfull)
            {
                PopUpBase popUpInstance = GameObject.Instantiate(asset.RuntimePopup, transform).GetComponent<PopUpBase>();
                onPopupInstantiateSuccess?.Invoke(LoadStatus.Successfull, popUpInstance);
            }
            else
            {
                onPopupInstantiateSuccess?.Invoke(LoadStatus.Failed, null);
            }
        }

        public override void Initialize()
        {
            foreach (var popupPrefabBase in _popUpPrefabList)
            {
                if (popupPrefabBase != null)
                {
                    _popupPrefabDictionary.Add(popupPrefabBase.GetType(), popupPrefabBase);
                }
                else
                {
                    Debug.LogError($"#{nameof(PopupManagerConfig)}.Initialize: Popup inside {nameof(PopupManagerConfig)} list not set to an Instance of an Object", this);
                }
            }
        }
    }
}
