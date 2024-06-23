using I2.Loc.SimpleJSON;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using BebiLibs.RegistrationSystem;
using System;
using BebiLibs;
using BebiLibs.ServerConfigLoaderSystem;

namespace Survey
{
    public class SurveyUrlParser : MonoBehaviour
    {
        [SerializeField] private string _configurationUrl;
        [SerializeField] private string _resultSubmitUrl;

        public string UrlToParse => _configurationUrl;
        public string UrlToSend => _resultSubmitUrl;

        private JArray _jsonparser = new JArray();
        public Action<bool, string> JsonRetrieveEvent;

        private string _response = "";

        public void Init(string configurationUrl, string resultSubmitUrl)
        {
            CheckUrlForNullOrEmpty(configurationUrl, nameof(configurationUrl));
            CheckUrlForNullOrEmpty(resultSubmitUrl, nameof(resultSubmitUrl));

            _configurationUrl = configurationUrl;
            _resultSubmitUrl = resultSubmitUrl;
        }

        private void CheckUrlForNullOrEmpty(string url, string propertyName)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError($"{propertyName} is null or empty");
            }
        }


        public void TryLoadJson()
        {
            StartCoroutine(TryLoadJsonCoroutine());
        }

        private IEnumerator TryLoadJsonCoroutine()
        {
            System.Uri myUri = new Uri(UrlToParse);
            using UnityWebRequest webRequest = UnityWebRequest.Get(myUri);
            webRequest.certificateHandler = new CertPublicKey();
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                _response = webRequest.downloadHandler.text;

                JsonRetrieveEvent?.Invoke(true, _response);
            }
            else
            {
                JsonRetrieveEvent?.Invoke(false, _response);

            }
        }

        public string SerializeAnswersIntoJsonString<T>(List<T> questionList)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(questionList);
        }

        public void SendSerialized()
        {

        }

        public List<SurveyQuestion> TryParseJson(string jsonResponse)
        {

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<SurveyQuestion>>(jsonResponse);
        }
    }
    class CertPublicKey : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
