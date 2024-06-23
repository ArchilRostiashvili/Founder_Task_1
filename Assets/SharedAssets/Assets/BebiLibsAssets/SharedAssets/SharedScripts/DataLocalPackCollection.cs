using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BalloonAdventure
{
    using UnityEngine;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [CreateAssetMenu(fileName = "DataLocalPackCollection", menuName = "Modules/BalloonAdventure/DataLocalPackCollection", order = 0)]
    public class DataLocalPackCollection : ScriptableObject
    {
        [System.Serializable]
        internal class DataLocal
        {
            public AssetReference dataLocalPack;
            public string localID;
        }

        [SerializeField] private List<DataLocal> _arrayLocalRefrancePack = new List<DataLocal>();
        AsyncOperationHandle<DataLocalPack> _localHandle;

        private DataLocalPack _dataLocalPack;
        public DataLocalPack dataLocalPack => _dataLocalPack;

        public IEnumerator LoadDataLocal(string localID)
        {
            DataLocal dataLocal = _arrayLocalRefrancePack.Find(x => x.localID == localID);
            if (dataLocal == null)
            {
                Debug.LogError($"DatLocal with local: \"{localID}\" Is Null");
                yield break;
            }

            if (_localHandle.IsValid() && _localHandle.Status == AsyncOperationStatus.Succeeded)
            {
                yield break;
            }

            _localHandle = dataLocal.dataLocalPack.LoadAssetAsync<DataLocalPack>();
            yield return _localHandle;

            if (_localHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _dataLocalPack = _localHandle.Result;
            }
            else
            {
                Debug.LogError($"Error Loading DataLocalPack for lang {localID}");

            }
        }

        public void Release()
        {
            if (_localHandle.IsValid())
            {
                Addressables.Release(_localHandle);
            }
        }

    }
}
