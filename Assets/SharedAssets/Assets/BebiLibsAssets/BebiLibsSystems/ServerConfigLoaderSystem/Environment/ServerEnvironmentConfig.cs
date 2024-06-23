using System.Collections;
using UnityEngine;

namespace BebiLibs.ServerConfigLoaderSystem.Environment
{
    using System;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    using UnityEngine;

    [CreateAssetMenu(fileName = "EnvironmentManager", menuName = "BebiLibs/RegistrationSystem/EnvironmentManager", order = 0)]
    public class ServerEnvironmentConfig : ScriptableConfig<ServerEnvironmentConfig>
    {
        [ObjectInspector(true)]
        [SerializeField] internal ServerEnvironmentData _production;
        [ObjectInspector(true)]
        [SerializeField] internal ServerEnvironmentData _development;

        [System.NonSerialized] private ServerEnvironmentData _activeEnv;

        public string LocalEndpoint => _activeEnv.GetLocalEndpoint();
        public string ConfigURL => _activeEnv.GetConfigURL();
        public string LocalInitializationURL => _activeEnv.GetLocalInitializationURL();
        public string LocalPurchaseURL => _activeEnv.GetLocalPurchaseURL();
        public string LocalTokenRefreshURL => _activeEnv.GetLocalTokenRefreshURL();

        public string GetInitializationURL(string newEndpoint) => _activeEnv.GetInitializationURL(newEndpoint);
        public string GetInitializationURL(ServerEnvironmentData environmentData, string newEndpoint) => environmentData.GetInitializationURL(newEndpoint);

        public string GetPurchaseURL(string newEndpoint) => _activeEnv.GetPurchaseURL(newEndpoint);
        public string GetPurchaseURL(ServerEnvironmentData environmentData, string newEndpoint) => environmentData.GetPurchaseURL(newEndpoint);

        public string GetTokenTransferURL(string newEndpoint) => _activeEnv.GetTokenTransferURL(newEndpoint);
        public string GetTokenTransferURL(ServerEnvironmentData environmentData, string newEndpoint) => environmentData.GetTokenTransferURL(newEndpoint);

        public string GetTokenRefreshURL(string newEndpoint) => _activeEnv.GetTokenRefreshURL(newEndpoint);
        public string GetTokenRefreshURL(ServerEnvironmentData environmentData, string newEndpoint) => environmentData.GetTokenRefreshURL(newEndpoint);

        public string GetDeleteUserAccountURL(string newEndpoint) => _activeEnv.GetDeleteUserAccountURL(newEndpoint);
        public string GetDeleteUserAccountURL(ServerEnvironmentData environmentData, string newEndpoint) => environmentData.GetDeleteUserAccountURL(newEndpoint);

        public string GetReceiptSendURl(string newEndpoint) => _activeEnv.GetReceiptSendURL(newEndpoint);
        public string GetReceiptSendURl(ServerEnvironmentData environmentData, string newEndpoint) => environmentData.GetReceiptSendURL(newEndpoint);

        public string GetEmailLoginURL(string newEndpoint) => _activeEnv.GetEmailLoginURL(newEndpoint);
        public string GetCreateEmailAccountURL(string newEndpoint, string languageID, string email) => _activeEnv.GetCreateEmailAccountURL(newEndpoint, languageID, email);
        public string GetForgetPasswordURL(string newEndpoint, string languageID, string email) => _activeEnv.GetForgetPasswordURL(newEndpoint, languageID, email);


        public override void Initialize()
        {

        }

        public void Initialize(bool forceDevelopmentEnvironment)
        {
            if (forceDevelopmentEnvironment)
            {
                _activeEnv = _development;
                _isInitialized = true;
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, this, "Enable environment: " + _activeEnv.name);
                return;
            }

            _activeEnv = Application.isEditor ? _development : _production;
            _isInitialized = true;

            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, this, "Enable environment: " + _activeEnv.name);
        }

        public void SetServerConfigEnvironment(ServerEnvironmentData dev, ServerEnvironmentData prod)
        {
            _development = dev;
            _production = prod;
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(ServerEnvironmentConfig))]
    public class EnvironmentSOEditor : Editor
    {
        ServerEnvironmentConfig _config;

        private void OnEnable()
        {
            _config = (ServerEnvironmentConfig)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            _config.Initialize(false);
            TestButton("ConfigURL:", "Test Config URL", _config.ConfigURL);
            TestButton("InitializationURL:", "Test Initialization URL", _config.LocalInitializationURL);
            TestButton("PurchaseURL:", "Test Purchase URL", _config.LocalPurchaseURL);
            TestButton("Receipt:", "Receipt Refresh URL", _config.GetReceiptSendURl(_config.LocalEndpoint));
        }

        public void TestButton(string label, string testName, string testURL)
        {
            EditorGUILayout.TextField(label, testURL);
            if (GUILayout.Button(testName))
            {
                Application.OpenURL(testURL);
            }
        }
    }
#endif
}
