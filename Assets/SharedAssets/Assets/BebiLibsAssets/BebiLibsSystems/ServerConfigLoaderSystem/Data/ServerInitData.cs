using BebiLibs.RegistrationSystem.Core;
using BebiLibs.ServerConfigLoaderSystem.Core;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using System;


namespace BebiLibs.ServerConfigLoaderSystem
{
    [CreateAssetMenu(fileName = "RegistrationInitDataSO", menuName = "Registration/RegistrationInitDataSO", order = 0)]
    public class ServerInitData : AbstractInitData, IResetOnExitPlay
    {
        [Header("Initialization Data: ")]

        [JsonIgnore()]
        public AbstractUserData _userData;

        public List<Login> logins;
        public string ads;

        public string deviceId;

        public Surveys surveys;


        [JsonProperty("receipt")]
        public string appleReceipt;
        [JsonProperty("subscription")]
        public string googleSubscription;

        [JsonProperty("registration")]
        public string RegistrationURL;

        [JsonProperty("reset")]
        public string ResetUrl;

        public bool update;
        public bool error;
        public string lastVersion;

        public SDateTime time = new SDateTime(DateTime.UtcNow);
        public string country;
        public string config;

        [JsonIgnore()]
        public string receiptURL =>
#if UNITY_ANDROID
                googleSubscription;
#elif UNITY_IOS
                appleReceipt;
#else       
                googleSubscription;
#endif

        public string bindURL => receiptURL + "&attach=true";


        public override void ResetData()
        {
            lastVersion = string.Empty;
            update = false;
            error = false;
            logins.Clear();
            _isDataLoadSuccessfull = false;
        }

        public override void ResetOnBuild()
        {
            base.ResetOnBuild();
            deviceId = string.Empty;
            country = string.Empty;
        }

        public void ResetOnExitPlay()
        {
            base.ResetOnBuild();
            deviceId = string.Empty;
            country = string.Empty;
        }

        public override void LoadDataFromMemory()
        {
            logins.Clear();
            base.LoadDataFromMemory();
        }

        protected override bool ValidateDeserializedData()
        {
            return !error && logins.Count > 0;
        }

        public override void ModifyRequestHeader(UnityWebRequest request)
        {
            base.ModifyRequestHeader(request);

            BaseRequestHandler.SetDeviceKey(request);
            BaseRequestHandler.SetBearerToken(request, _userData);

            if (!string.IsNullOrEmpty(deviceId))
            {
                BaseRequestHandler.SetDeviceID(deviceId, request);
            }
        }

        public override bool GetDeviceID(out string outDeviceID)
        {
            if (!string.IsNullOrEmpty(deviceId))
            {
                outDeviceID = deviceId;
                return true;
            }

            string localDeviceID = SystemInfo.deviceUniqueIdentifier;
            outDeviceID = Uri.EscapeDataString(localDeviceID);
            return false;
        }
    }

    [System.Serializable]
    public class Surveys
    {
        public string questions;
        public string answers;
    }

    [System.Serializable]
    public class Login
    {
        public string provider;
        public string url;
    }
}
