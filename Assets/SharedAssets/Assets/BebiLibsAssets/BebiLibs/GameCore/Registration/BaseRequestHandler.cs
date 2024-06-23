using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using BebiLibs.Localization;
using BebiLibs.RegistrationSystem.Core;
using BebiLibs.ServerConfigLoaderSystem.Core;

namespace BebiLibs.ServerConfigLoaderSystem
{
    public class BaseRequestHandler : MonoBehaviour
    {
        public const int REQUEST_TIME_OUT = 30;
        public const string REMOTE_DEVICE_ID = "X-Device-Id";
        public const string USER_AGENT = "X-User-Agent";
        public const string APPLICATON_VERSION = "X-Client-Version";
        public const string APPLICATION_LANGUAGE = "Accept-Language";
        public const string LOCAL_DEVICE_ID = "X-Device-Key";

        public const string AUTHORIZATION = "Authorization";

        private static AbstractInitData _AbstractInitData;
        private static AbstractUserData _AbstractUserData;

        public static void SetInitData(AbstractInitData abstractInitData) => _AbstractInitData = abstractInitData;
        public static void SetUserData(AbstractUserData abstractUserData) => _AbstractUserData = abstractUserData;

        public static IEnumerator GetDataFromUrl(string url, IBaseRequest responseReceiver)
        {
            System.Uri myUri = new Uri(url);
            using UnityWebRequest webRequest = UnityWebRequest.Get(myUri);
            UpdateRequestHeaders(responseReceiver, webRequest);
            HeaderLog(webRequest);
            yield return webRequest.SendWebRequest();

            string response = webRequest.downloadHandler.text;
            RequestStatus requestStatus = webRequest.result == UnityWebRequest.Result.Success ? RequestStatus.Success : RequestStatus.Failed;
            ResponseData responseData = new ResponseData(response, requestStatus, webRequest.responseCode);
            responseReceiver.RequestResponseData = responseData;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Error Loading Data From URL {url}, Error Code {webRequest.responseCode}, {response} Make sure that url is right and connected to internet, Error {webRequest.error}");
            }
        }

        public static IEnumerator PostDataToUrl(string url, string data, IBaseRequest baseRequest)
        {
            var jsonBinary = System.Text.Encoding.UTF8.GetBytes(data);
            using DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
            using UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
            uploadHandlerRaw.contentType = "application/json";

            using UnityWebRequest webRequest = new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

            UpdateRequestHeaders(baseRequest, webRequest);
            HeaderLog(webRequest);
            yield return webRequest.SendWebRequest();

            string response = webRequest.downloadHandler.text;
            RequestStatus requestStatus = webRequest.result == UnityWebRequest.Result.Success ? RequestStatus.Success : RequestStatus.Failed;
            ResponseData responseData = new ResponseData(response, requestStatus, webRequest.responseCode);

            baseRequest.RequestResponseData = responseData;

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"Error Loading Data From URL {url}, Error Code {webRequest.responseCode}, {response} Make sure that url is right and connected to internet");
            }
        }


        private static void UpdateRequestHeaders(IRequestModifier requestModifier, UnityWebRequest webRequest)
        {
            webRequest.timeout = REQUEST_TIME_OUT;
            webRequest.SetRequestHeader(APPLICATION_LANGUAGE, LocalizationManager.ActiveLanguageUniversalID);
            webRequest.SetRequestHeader(USER_AGENT, GetDeviceAgent());
            webRequest.SetRequestHeader(APPLICATON_VERSION, Application.version);

            SetDeviceID(webRequest);
            SetBearerToken(webRequest);

            if (requestModifier != null)
                requestModifier.ModifyRequestHeader(webRequest);
        }

        private static void SetDeviceID(UnityWebRequest webRequest)
        {
            if (_AbstractInitData == null)
            {
                Debug.LogError($"{nameof(BaseRequestHandler)}'s {nameof(AbstractInitData)} is null, Set it Before Config Initialization Using {nameof(SetDeviceID)} Method");
                return;
            }
            else if (_AbstractInitData.GetDeviceID(out string deviceID))
            {
                webRequest.SetRequestHeader(REMOTE_DEVICE_ID, deviceID);
            }
        }

        private static void SetBearerToken(UnityWebRequest webRequest)
        {
            if (_AbstractUserData == null)
            {
                Debug.LogError($"{nameof(BaseRequestHandler)}'s {nameof(AbstractUserData)} is null, Set it Before Config Initialization Using {nameof(SetBearerToken)} Method");
                return;
            }
            else if (_AbstractUserData.GetRawUserToken(out string rawToken))
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + rawToken);
            }
        }

        public static string GetDeviceAgent()
        {
            string deviceHeader = "Model: " + SystemInfo.deviceModel + ", Name: " + SystemInfo.deviceName + ", Type: " + SystemInfo.deviceType + ", OS: " + SystemInfo.operatingSystem;
            return EncodeString(deviceHeader);
        }

        public static void SetBearerToken(UnityWebRequest webRequest, AbstractUserData abstractUserData)
        {
            if (webRequest == null)
            {
                Debug.LogWarning("webRequest is null");
                return;
            }

            if (abstractUserData == null)
            {
                Debug.LogWarning("abstractUserData is null");
                return;
            }

            if (abstractUserData.GetRawUserToken(out string rawToken))
            {
                webRequest.SetRequestHeader(AUTHORIZATION, "Bearer " + rawToken);
            }
        }

        public static void SetDeviceKey(UnityWebRequest webRequest)
        {
            string deviceID = SystemInfo.deviceUniqueIdentifier;
            deviceID = System.Uri.EscapeDataString(deviceID);
            webRequest.SetRequestHeader(LOCAL_DEVICE_ID, deviceID);
        }

        public static void SetDeviceID(string deviceID, UnityWebRequest webRequest)
        {
            webRequest.SetRequestHeader(REMOTE_DEVICE_ID, deviceID);
        }

        public static string EncodeString(string headerString)
        {
            return Uri.EscapeDataString(headerString);
        }

        private static void HeaderLog(UnityWebRequest webRequest)
        {
            // string header = $"Request Header -> {webRequest.url}\n";
            // header += "Authorization: " + webRequest.GetRequestHeader("Authorization") + "\n";
            // header += "X-Device-Id: " + webRequest.GetRequestHeader("X-Device-Id") + "\n";
            // header += "X-Device-Key: " + webRequest.GetRequestHeader("X-Device-Key") + "\n";
            // header += "X-User-Agent: " + webRequest.GetRequestHeader("X-User-Agent") + "\n";
            // Debug.LogWarning(header);
        }


    }
}
