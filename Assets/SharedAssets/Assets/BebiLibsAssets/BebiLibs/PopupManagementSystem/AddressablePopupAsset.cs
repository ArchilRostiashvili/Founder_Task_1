using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BebiLibs.PopupManagementSystem
{
    [System.Serializable]
    public class AddressablePopupAsset
    {
        [ReadonlyField]
        public string PopupTypeName;
        public AssetReferenceGameObject PopupAssetReference;
        [System.NonSerialized] private GameObject _runtimePopup;

        private AsyncOperationHandle<GameObject> _audioLoadHandler;
        [System.NonSerialized] private LoadStatus _addressableLoadStatus = LoadStatus.None;

        public AddressablePopupAsset(string name, AssetReferenceGameObject asset)
        {
            PopupTypeName = name;
            PopupAssetReference = asset;
        }

        public GameObject RuntimePopup => _runtimePopup;
        public LoadStatus LoadStatus => _addressableLoadStatus;

        public IEnumerator LoadAsset()
        {
            if (_addressableLoadStatus == LoadStatus.Started)
            {
                Debug.LogWarning("Localized Track Data Loading Already Started");
                yield break;
            }

            if (_addressableLoadStatus == LoadStatus.Successfull)
            {
                Debug.LogWarning("Localized Track Data Loading Already Loaded Successfully");
                yield break;
            }

            _addressableLoadStatus = LoadStatus.Started;
            _audioLoadHandler = PopupAssetReference.LoadAssetAsync();

            yield return _audioLoadHandler;

            _addressableLoadStatus = _audioLoadHandler.Status == AsyncOperationStatus.Succeeded ? LoadStatus.Successfull : LoadStatus.Failed;

            Debug.Log(_addressableLoadStatus);
            if (_addressableLoadStatus == LoadStatus.Successfull)
                _runtimePopup = _audioLoadHandler.Result;
        }

        public void ReleaseAsset()
        {
            if (_addressableLoadStatus == LoadStatus.Started)
            {
                Debug.LogWarning("Loading already started");
                return;
            }

            if (_addressableLoadStatus == LoadStatus.Successfull && _audioLoadHandler.IsValid())
            {
                Addressables.Release(_audioLoadHandler);
            }

            if (_runtimePopup != null)
            {
                Resources.UnloadAsset(_runtimePopup);
            }

            _addressableLoadStatus = LoadStatus.None;
            _audioLoadHandler = default;
            _runtimePopup = null;
        }
    }

}
