using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.ServerConfigLoaderSystem.Environment
{

    [CreateAssetMenu(fileName = "EnvironmentDataSO", menuName = "BebiLibs/RegistrationSystem/EnvironmentDataSO", order = 0)]
    public class ServerEnvironmentData : ScriptableObject
    {
        [Header("Config Url:")]
        [SerializeField] private string _configName;
        [SerializeField] private string _configUrlSchema;

        [Header("App Endpoint:")]
        [SerializeField] private string _localEndpoint;
        [SerializeField] private string _appName;

        [Header("Schemas:")]
        [SerializeField] private string _initializationUrlSchema;
        [SerializeField] private string _purchaseDataUrlSchema;
        [SerializeField] private string _tokenRefreshUrlSchema;
        [SerializeField] private string _tokenTransferUrlSchema;
        [SerializeField] private string _deleteAccountUrlSchema;
        [SerializeField] private string _receiptURLSchema;
        [SerializeField] private string _subscriptionURLSchema;
        [SerializeField] private string _emailLoginURLSchema;
        [SerializeField] private string _createEmailAccountURLSchema;
        [SerializeField] private string _resetPasswordURLSchema;

        public string OsSuffix =>
#if UNITY_ANDROID
        "os=android";
#elif UNITY_IOS
        "os=ios";
#else 
        "os=android";
#endif  

        public string GetLocalEndpoint() => _localEndpoint;
        public string GetConfigURL() => string.Format(_configUrlSchema, _configName);

        public string GetLocalInitializationURL() => string.Format(_initializationUrlSchema, _localEndpoint, _appName, OsSuffix);
        public string GetLocalPurchaseURL() => string.Format(_purchaseDataUrlSchema, _localEndpoint, _appName, OsSuffix);
        public string GetLocalTokenRefreshURL() => string.Format(_tokenRefreshUrlSchema, _localEndpoint, _appName, OsSuffix);

        public string GetInitializationURL(string newEndpoint) => string.Format(_initializationUrlSchema, newEndpoint, _appName, OsSuffix);
        public string GetPurchaseURL(string newEndpoint) => string.Format(_purchaseDataUrlSchema, newEndpoint, _appName, OsSuffix);
        public string GetTokenRefreshURL(string newEndpoint) => string.Format(_tokenRefreshUrlSchema, newEndpoint, _appName, OsSuffix);
        public string GetTokenTransferURL(string newEndpoint) => string.Format(_tokenTransferUrlSchema, newEndpoint, _appName, OsSuffix);
        public string GetDeleteUserAccountURL(string newEndpoint) => string.Format(_deleteAccountUrlSchema, newEndpoint, _appName, OsSuffix);

        public string GetEmailLoginURL(string newEndpoint) => string.Format(_emailLoginURLSchema, newEndpoint, _appName, OsSuffix);

        public string GetCreateEmailAccountURL(string newEndpoint, string languageID, string email) => string.Format(_createEmailAccountURLSchema, newEndpoint, languageID, email, OsSuffix);
        public string GetForgetPasswordURL(string newEndpoint, string languageID, string email) => string.Format(_createEmailAccountURLSchema, newEndpoint, languageID, email, OsSuffix);


        public string GetReceiptSendURL(string newEndpoint)
        {
#if UNITY_ANDROID
            return string.Format(_subscriptionURLSchema, newEndpoint, _appName, OsSuffix);
#elif UNITY_IOS
            return string.Format(_receiptURLSchema, newEndpoint, _appName, OsSuffix);
#endif
        }

    }
}
