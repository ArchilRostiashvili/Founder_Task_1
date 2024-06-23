using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs.Analytics;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;


namespace BebiLibs
{
    public class PopUp_Report : PopUpBase
    {
        public static PopUp_Report instance;

        [Header("Report Url")]
        public string urlToSend = "https://bebi-family.fndr.dev/api/v1/big-adventure/app/feedback";

        [Header("Report Button Texts")]
        public TMP_InputField reportInputField;
        public TMP_Text textLimitCounter;

        [Header("Report Button")]
        public ButtonScale ReportButton;
        public Image Image_ReportButtonCover;

        [Header("Report Character Values")]
        [Range(250, 3000)]
        public int maxCharacterCount = 1024;
        private int _currentCharacterCount = 0;
        private string reportText;

        public override void Init()
        {
            base.Init();
            instance = this;
            reportInputField.characterLimit = maxCharacterCount;
        }

        public static void Activate()
        {
            instance.Show(false);
        }


        private void GenerateAndSendReport()
        {
            JObject body = new JObject();
            JProperty langId = new JProperty("langId", 0);
            JProperty text = new JProperty("text", reportText);
            body.Add(langId);
            body.Add(text);
            DoPostRequest(urlToSend, body.ToString(), false);
            Hide(false);
        }

        public void DoPostRequest(string url, string data, bool debugLog = false)
        {
            StartCoroutine(PostRequest());
            IEnumerator PostRequest()
            {
                var jsonBinary = System.Text.Encoding.UTF8.GetBytes(data);

                DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
                UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(jsonBinary);
                uploadHandlerRaw.contentType = "application/json";

                using UnityWebRequest www = new UnityWebRequest(url, "POST", downloadHandlerBuffer, uploadHandlerRaw);

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError(string.Format("{0}: {1}", www.url, www.error));
                else
                    Debug.Log(string.Format("Response: {0}", www.downloadHandler.text));
                if (debugLog)
                {
                    DoPostRequest("http://192.168.88.234:5555/", www.downloadHandler.text, false);
                }
            }
        }

        public void Trigger_ButtonClick_SendReport()
        {
            if (reportText.Length == 0)
            {
                ManagerSounds.PlayEffect("fx_wrong7");
            }
            else
            {
                GenerateAndSendReport();
            }
        }

        public void UpdateReportButtonState()
        {
            if (reportText.Length > 0)
            {
                SetReportButtonActive(true);
            }
            else
            {
                SetReportButtonActive(false);
            }
        }

        private void SetReportButtonActive(bool value)
        {
            ReportButton.buttonEnabled = value;
            Image_ReportButtonCover.gameObject.SetActive(!value);
        }


        public override void Show(bool anim)
        {
            base.Show(anim);
            ManagerSounds.PlayEffect("fx_page15");
            _currentCharacterCount = 0;
            UpdateCharacterCounter();
        }

        private void UpdateCharacterCounter()
        {
            textLimitCounter.text = $"Character limit: {_currentCharacterCount}/{maxCharacterCount}";
        }

        public void OnInputFieldChange(string input)
        {
            _currentCharacterCount = input.Length;
            reportText = input;
            if (_currentCharacterCount >= maxCharacterCount)
            {
                ManagerSounds.PlayEffect("fx_wrong7");
            }

            UpdateReportButtonState();
            UpdateCharacterCounter();
        }


        public override void Trigger_ButtonClick_Close()
        {
            ManagerSounds.PlayEffect("fx_page17");
            Hide(false);
        }

        public override void Hide(bool anim)
        {
            base.Hide(false);
        }



    }
}
