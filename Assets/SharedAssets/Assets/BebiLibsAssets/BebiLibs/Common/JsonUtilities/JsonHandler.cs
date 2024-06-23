using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class JsonHandler
    {
        private static IContractResolver _ContractResolver = new FixedUnityTypeContractResolver();

        public static bool TryPopulateObjectFromPlayerPref<T>(string prefKey, T defaultObject, out string json, Formatting formatting = Formatting.None)
        {
            try
            {
                string defaultModel = JsonConvert.SerializeObject(defaultObject, formatting, new JsonSerializerSettings() { ContractResolver = _ContractResolver });
                json = PlayerPrefs.GetString(prefKey, defaultModel);
                JsonConvert.PopulateObject(json, defaultObject, new JsonSerializerSettings() { ContractResolver = _ContractResolver });
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Unable To Load {defaultObject} from Memory, Error: " + e);
                json = string.Empty;
                return false;
            }
        }

        public static bool TrySaveObjectIntoPlayerPref<T>(string prefKey, T defaultObject, out string json, Formatting formatting = Formatting.None)
        {
            try
            {
                json = JsonConvert.SerializeObject(defaultObject, formatting, new JsonSerializerSettings() { ContractResolver = _ContractResolver });
                PlayerPrefs.SetString(prefKey, json);
                PlayerPrefs.Save();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Unable To Save {defaultObject} to Memory, Error: " + e);
                json = string.Empty;
                return false;
            }
        }

        public static bool TryPopulateObjectFromJson<T>(string jsonData, T defaultObject, bool logError = true)
        {
            try
            {
                JsonConvert.PopulateObject(jsonData, defaultObject, new JsonSerializerSettings() { ContractResolver = _ContractResolver });
                return true;
            }
            catch (System.Exception e)
            {
                if (logError)
                {
                    Debug.LogWarning($"Unable To Deserialize Data {defaultObject} Error: {e}");
                }
                return false;
            }
        }

        public static bool TrySerializeObjectToJson<T>(T defaultObject, out string jsonData)
        {
            return TrySerializeObjectToJson(defaultObject, Formatting.None, out jsonData);
        }

        public static bool TrySerializeObjectToJson<T>(T defaultObject, Formatting formatting, out string jsonData)
        {
            try
            {
                jsonData = JsonConvert.SerializeObject(defaultObject, formatting, new JsonSerializerSettings() { ContractResolver = _ContractResolver });
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Unable To Serialize {defaultObject} to Json, Error: {e}");
                jsonData = string.Empty;
                return false;
            }

        }
    }
}
