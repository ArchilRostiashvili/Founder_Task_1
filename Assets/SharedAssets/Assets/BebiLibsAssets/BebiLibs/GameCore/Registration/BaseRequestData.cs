using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.ServerConfigLoaderSystem.Core
{
    public abstract class BaseRequestData : ScriptableObject, IBaseRequest, IResetOnPreBuild
    {
        [SerializeField] protected SDateTime lastUpdateTime = SDateTime.UtcMin;

        [JsonIgnore()]
        [SerializeField] private string _serializationKey;
        [JsonIgnore()]
        [SerializeField] protected int _timeout = 6;

        [JsonIgnore()]
        [SerializeField] internal ResponseData _responseData;

        [SerializeField] protected bool _isDataLoadSuccessfull = false;
        [SerializeField] protected bool _isDebugModeEnabled = false;

        public int network_timeout => _timeout;
        public bool isLoaded => _isDataLoadSuccessfull;
        public bool IsDebugModeEnabled => Debug.isDebugBuild && _isDebugModeEnabled;


        public ResponseData RequestResponseData
        {
            get => _responseData;
            set
            {
                //Debug.Log($"{this.name} Set Response Data {value}");
                _responseData = value;
            }

        }

        public virtual void SaveDataToMemory()
        {
            CheckSerializationKey();
            JsonHandler.TrySaveObjectIntoPlayerPref(_serializationKey, this, out string dataToSave, Formatting.None);
            // string log = $"{name} Saved Into Memory {dataToSave}";
            // Debug.LogWarning(log);
        }

        public virtual void LoadDataFromMemory()
        {
            CheckSerializationKey();
            JsonHandler.TryPopulateObjectFromPlayerPref(_serializationKey, this, out string dataToLoad, Formatting.None);
            // string log = $"{name} Loaded Data from Memory {dataToLoad}";
            // Debug.LogWarning(log);
        }

        private void CheckSerializationKey()
        {
            if (string.IsNullOrWhiteSpace(_serializationKey))
            {
                Debug.LogError($"Serialization Key is Null or Empty for {name}");
                _serializationKey = name;
            }
        }

        public virtual bool TryDeserializeJsonFromServer(string jsonText)
        {
            if (IsDebugModeEnabled)
            {
                LoadDataFromMemory();
                return true;
            }

            ResetData();
            lastUpdateTime = SDateTime.UtcNow;
            if (!JsonHandler.TryPopulateObjectFromJson(jsonText, this))
            {
                _isDataLoadSuccessfull = false;
                return false;
            }

            _isDataLoadSuccessfull = ValidateDeserializedData();
            if (!_isDataLoadSuccessfull)
            {
                Debug.LogWarning($"Unable To Validate {name} While Loading From Server");
                return false;
            }
            return true;
        }

        public IEnumerator GetDataFromServer(string url)
        {
            yield return StartGettingDataFromServer(url);
        }

        public virtual IEnumerator StartGettingDataFromServer(string url)
        {
            yield return BaseRequestHandler.GetDataFromUrl(url, this);

            if (_responseData.RequestStatus == RequestStatus.Success)
            {
                UpdateDataFromJson(_responseData.ResponseString);
            }
            else if (_responseData.RequestStatus == RequestStatus.Failed && (_responseData.ResponseCode == 403 || _responseData.ResponseCode == 401 || _responseData.ResponseCode == 500))
            {
                UpdateDataFromJson(_responseData.ResponseString);
            }
            else
            {
                LoadDataFromMemory();
            }
        }

        private void UpdateDataFromJson(string json)
        {
            if (TryDeserializeJsonFromServer(json))
            {
                SaveDataToMemory();
            }
            else
            {
                LoadDataFromMemory();
            }
        }


        public IEnumerator PostDataToServer(string url, string data)
        {
            yield return StartPostingDataToServer(url, data);
        }

        public virtual IEnumerator StartPostingDataToServer(string url, string data)
        {
            yield return BaseRequestHandler.PostDataToUrl(url, data, this);

            if (_responseData.RequestStatus == RequestStatus.Success)
            {
                UpdateDataFromJson(_responseData.ResponseString);
            }
            else if (_responseData.RequestStatus == RequestStatus.Failed || _responseData.ResponseCode == 403 || _responseData.ResponseCode == 401)
            {
                UpdateDataFromJson(_responseData.ResponseString);
            }
            else
            {
                LoadDataFromMemory();
            }
        }

        public virtual void ModifyRequestHeader(UnityWebRequest request)
        {
            request.timeout = _timeout;
        }

        protected abstract bool ValidateDeserializedData();
        public abstract void ResetData();

        void IResetOnPreBuild.ResetOnPreBuild()
        {
            ResetOnBuild();
        }

        public virtual void ResetOnBuild()
        {
            ResetData();
            _isDataLoadSuccessfull = false;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BaseRequestData), true)]
    public class BaseRequestSerialzableDataEditor : Editor
    {
        private BaseRequestData _baseRequestData;

        private void OnEnable()
        {
            _baseRequestData = (BaseRequestData)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save To Memory"))
            {
                _baseRequestData.SaveDataToMemory();
            }

            if (GUILayout.Button("Load From Memory"))
            {
                _baseRequestData.LoadDataFromMemory();
            }
        }
    }
#endif
}
