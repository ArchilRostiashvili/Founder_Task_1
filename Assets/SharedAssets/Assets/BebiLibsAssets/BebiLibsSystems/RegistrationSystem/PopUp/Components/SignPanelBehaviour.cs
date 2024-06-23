using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.RegistrationSystem.Core;
using System;
using UnityEngine.Events;

namespace BebiLibs.RegistrationSystem.PopUp.Components.Sign
{
    public class SignPanelBehaviour : MonoBehaviour
    {
        public UnityAction OnExitButtonClickEvent;
        public UnityAction OnSwitchPanelButtonClickEvent;
        public UnityAction<Transform, Provider> OnSignInButtonClickEvent;

        [SerializeField] private string _panelName;
        [SerializeField] private string _panelPopupName;

        [SerializeField] ManualRegistrationPanel _manualRegistrationPanel;

        [SerializeField] private GameObject _buttonContainerGO;
        [SerializeField] private GameObject _orTextGO;

        [Header("Buttons: ")]
        [SerializeField] private List<SignButton> _signInButtonList = new List<SignButton>();
        [SerializeField] private ButtonScale _exitButton;
        [SerializeField] private Button _switchPanelsButton;
        [SerializeField] private ServerInitData _registrationInitDataSO;

        [Header("Loader Management: ")]
        [SerializeField] private GameObject _loaderGO;
        [SerializeField] private GameObject _registrationPromptGO;

        public ManualRegistrationPanel ManualRegistrationPanel => _manualRegistrationPanel;

        public string PanelName => _panelName;
        public string PanelPopupName => _panelPopupName;

        private void OnEnable()
        {
            foreach (SignButton button in _signInButtonList)
            {
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener(() => OnSignInButtonClick(button));
            }

            //SortSignInButtonsInHierarchy(Provider.Apple);

            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(OnExitButtonClickEvent);

            _switchPanelsButton.onClick.RemoveAllListeners();
            _switchPanelsButton.onClick.AddListener(OnSwitchPanelButtonClickEvent);
        }

        private void SortSignInButtonsInHierarchy(Provider providerToPutOnTop)
        {
            SignButton buttonToPutOnTop = _signInButtonList.Find(x => x.provider == providerToPutOnTop);
            if (buttonToPutOnTop != null)
            {
                buttonToPutOnTop.transform.SetAsFirstSibling();
            }
        }

        private void OnDisable()
        {
            foreach (SignButton button in _signInButtonList)
            {
                button.OnClick.RemoveAllListeners();
            }

            _exitButton.onClick.RemoveAllListeners();
            _switchPanelsButton.onClick.RemoveAllListeners();
        }


        public void Show(bool resetPanel)
        {
            gameObject.SetActive(true);

            if (resetPanel)
            {
                _manualRegistrationPanel.ResetFields();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        internal void OnManualSignInErrorEvent()
        {
            _manualRegistrationPanel.DisplayError();
        }

        public void SetLoaderActive(bool value)
        {
            _loaderGO.gameObject.SetActive(value);
            _switchPanelsButton.gameObject.SetActive(!value);
            _registrationPromptGO.SetActive(!value);
            _signInButtonList.ForEach(x => x.SetButtonActive(false));
            _buttonContainerGO.SetActive(!value);
            _manualRegistrationPanel.gameObject.SetActive(!value);
            _orTextGO.SetActive(!value);

            if (!value)
            {
                UpdateSingInView();
            }
        }

        public void UpdateSingInView()
        {
            List<Login> logins = _registrationInitDataSO.logins;
            if (logins.Count > 0)
            {
                foreach (var item in _signInButtonList)
                {
                    Login loginUrl = logins.Find(x => x.provider == item.provider.ToString());
                    item.SetButtonActive(loginUrl != null);
                }
            }
            else
            {
                Debug.LogWarning("Autorithation Url List is empty, not provided from server, Sign in button should be disabled");
            }

            _switchPanelsButton.gameObject.SetActive(true);
        }

        public void OnSignInButtonClick(SignButton buttonObject)
        {
            SignButton signInButton = _signInButtonList.Find(x => x.provider == buttonObject.provider);
            if (signInButton != null)
            {
                OnSignInButtonClickEvent?.Invoke(buttonObject.transform, signInButton.provider);
            }
            else
            {
                Debug.LogError($"Make Sure \"{nameof(buttonObject)}\" is not null and it placed in \"{nameof(_signInButtonList)}\" array");
            }
        }


    }
}
