using BebiLibs.Localization;
using BebiLibs.Localization.Core;
using BebiLibs.Analytics.GameEventLogger;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs
{
    [ExecuteInEditMode]
    public class ErrorMessagePanel : MonoBehaviour
    {
        [SerializeField] internal string panel_name;
        [SerializeField] private MessageInfoType _errorMessageType;
        public MessageInfoType messageInfoType => _errorMessageType;

        [SerializeField] protected string _contentText;

        [Header("Panel Components")]
        [SerializeField] private TMP_Text Text_Info;
        [SerializeField] private TMP_Text Text_Header;
        [SerializeField] private TMP_Text Text_ButtonSubmit;
        [SerializeField] private TMP_Text Text_ErrorText;

        private PopUp_MessageInfo _popUp_Information;

        private void OnEnable()
        {
            Localization.LocalizationManager.OnLanguageChangeEvent -= OnLanguageChange;
            Localization.LocalizationManager.OnLanguageChangeEvent += OnLanguageChange;
            UpdateState();
        }

        private void OnDisable()
        {
            Localization.LocalizationManager.OnLanguageChangeEvent -= OnLanguageChange;
        }

        public void Init(PopUp_MessageInfo popUp_Error)
        {
            _popUp_Information = popUp_Error;
        }

        public void OnLanguageChange(LanguageIdentifier languageIdentifier)
        {
            UpdateState();
        }

        public void UpdateState()
        {
            string translatedTex = ProcessContentText();
            if (Text_Info != null)
            {
                Text_Info.text = translatedTex;
            }
        }

        public virtual string ProcessContentText()
        {
            if (!I2.Loc.LocalizationManager.TryGetTranslation(_contentText, out string message))
            {
                message = _contentText;
            }
            return message;
        }


        internal virtual void Show(string error)
        {
            if (Text_ErrorText != null)
                Text_ErrorText.text = !string.IsNullOrEmpty(error) ? "Error: " + error : "";

            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Trigger_ButtonClick_Exit()
        {
            ManagerSounds.PlayEffect("fx_page17");
            _popUp_Information.Hide(true);
        }

        public virtual void Trigger_ButtonClick_ButtonSubmit()
        {
            ManagerSounds.PlayEffect("fx_page15");
            _popUp_Information.TriggerEndAction();
            _popUp_Information.Hide(true);
        }

        internal void SetInfoText(string infoText)
        {
            _contentText = infoText;
        }
    }

    public enum MessageInfoType
    {
        Default, UpdateApp, SomethingWentWrong, NoInternet, BindSuccessful
    }
}
