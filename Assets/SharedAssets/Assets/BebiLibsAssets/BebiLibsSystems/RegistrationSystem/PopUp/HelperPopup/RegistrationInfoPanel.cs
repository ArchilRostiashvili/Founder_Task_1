using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.RegistrationSystem;
using BebiLibs.RegistrationSystem.Core;

namespace BebiLibs.RegistrationSystem.PopUp
{
    class RegistrationInfoPanel : MonoBehaviour
    {
        [SerializeField] private RegistrationInfoType _registrationInfoType;
        [SerializeField] private string _panelName;

        [Header("Buttons")]
        [SerializeField] private ButtonScale _okButton;
        [SerializeField] private ButtonScale _cancelButton;
        [SerializeField] private ButtonScale _closeButton;

        [SerializeField] private TextLinkClickDetector _textLinkClickDetector;
        [SerializeField] private DeleteAccountDataSO _deleteAccountDataSO;

        internal RegistrationInfoType PanelType => _registrationInfoType;
        internal string PanelName => _panelName;

        private RegistrationInfoPopup _registrationInfoPopup;

        private void OnEnable()
        {
            _okButton?.onClick.AddListener(OnOkButtonClick);
            _cancelButton?.onClick.AddListener(OnCancelButtonClick);
            _closeButton?.onClick.AddListener(OnCloseButtonClick);
            _textLinkClickDetector?.onClick.AddListener(OnTextLinkClick);
        }

        private void OnDisable()
        {
            _okButton?.onClick.RemoveListener(OnOkButtonClick);
            _cancelButton?.onClick.RemoveListener(OnCancelButtonClick);
            _closeButton?.onClick.RemoveListener(OnCloseButtonClick);
            _textLinkClickDetector?.onClick.RemoveListener(OnTextLinkClick);
        }

        private void OnTextLinkClick(LinkIDData linkIDData)
        {
            // #if UNITY_ANDROID
            //             Application.OpenURL("https://support.google.com/googleplay/answer/7018481?hl=en&co=GENIE.Platform%3DAndroid#zippy=%2Ccancel-a-subscription-on-the-google-play-app");
            // #elif UNITY_IOS
            //             Application.OpenURL("https://support.apple.com/en-us/HT202039");
            // #endif
            Application.OpenURL(_deleteAccountDataSO.GuideUrl);
        }

        private void OnCloseButtonClick()
        {
            _registrationInfoPopup.CloseButtonClickEvent?.Invoke();
            _registrationInfoPopup.Hide(true);
            ManagerSounds.PlayEffect("fx_page17");
        }

        private void OnCancelButtonClick()
        {
            _registrationInfoPopup.CancelButtonClickEvent?.Invoke();
            _registrationInfoPopup.Hide(true);
            ManagerSounds.PlayEffect("fx_page17");
        }

        private void OnOkButtonClick()
        {
            ManagerSounds.PlayEffect("fx_page16");
            _registrationInfoPopup.OkButtonClickEvent?.Invoke();
        }

        internal void Init(RegistrationInfoPopup registrationInfoPopup)
        {
            _registrationInfoPopup = registrationInfoPopup;
        }

        internal void Hide()
        {
            this.gameObject.SetActive(false);
        }

        internal void Show()
        {
            this.gameObject.SetActive(true);
        }
    }
}
