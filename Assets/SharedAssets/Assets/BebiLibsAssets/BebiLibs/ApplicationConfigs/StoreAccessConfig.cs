using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.GameApplicationConfig
{
    [CreateAssetMenu(fileName = "StoreAccessConfig", menuName = "BebiLibs/ApplicationConfigs/StoreAccessConfig", order = 0)]
    public class StoreAccessConfig : ScriptableObject
    {
        [SerializeField] private List<StoreConfigData> _storeConfigDataList;

        public string GetStoreURL(RuntimePlatform platform, string storeSpecificID)
        {
            StoreConfigData storeConfigData = _storeConfigDataList.Find(x => x.Platform == platform);
            if (storeConfigData == null)
            {
                Debug.LogError($"StoreConfigData not found for platform {platform}");
                return string.Empty;
            }

            return string.Format(storeConfigData.StoreUrlSchema, storeSpecificID);
        }


        [System.Serializable]
        public class StoreConfigData
        {
            public RuntimePlatform Platform;
            public string StoreUrlSchema;
        }
    }
}
