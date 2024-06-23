using BebiLibs.RegistrationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.RegistrationSystem.PopUp.Components.Sign;
using BebiLibs.RegistrationSystem.Core;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs.RegistrationSystem.PopUp.Components
{
    public class SignPopUpController : RegistrationPanel
    {
        public UnityAction<bool> SignInControllerStateChangeEvent;

        [SerializeField] private SignPanelBehaviour _signInPanel;
        [SerializeField] private SignPanelBehaviour _signUpPanel;
        [SerializeField] private SignPanelBehaviour _defaultPanel;
        [SerializeField] private RegistrationEventMediatorSO _eventMediator;
        private string _sender;

        private string panelName => _defaultPanel.PanelName;

        public override void Init(PopUp_Registration popUp_Registration)
        {
            base.Init(popUp_Registration);
        }

        private void OnEnable()
        {
            RegisterCallBacks(_signInPanel);
            RegisterCallBacks(_signUpPanel);
        }

        public void OnManualSignInErrorEvent()
        {
            _defaultPanel.OnManualSignInErrorEvent();
        }

        private void RegisterCallBacks(SignPanelBehaviour signPanelBehaviour)
        {
            signPanelBehaviour.OnExitButtonClickEvent = OnExitFromGameEvent;
            signPanelBehaviour.OnSignInButtonClickEvent = OnStartSignInEvent;
            signPanelBehaviour.OnSwitchPanelButtonClickEvent = OnSwitchPanelsEvent;

            signPanelBehaviour.ManualRegistrationPanel.OnNextButtonClickEvent = OnNextButtonClickEvent;
            signPanelBehaviour.ManualRegistrationPanel.OnSignInButtonClickEvent = OnManualSignInButtonClickEvent;
            signPanelBehaviour.ManualRegistrationPanel.OnForgetPasswordButtonClickEvent = OnForgetPasswordButtonClickEvent;
        }

        private void OnForgetPasswordButtonClickEvent(UserCredential userCredential)
        {
            _eventMediator.InvokeForgetPasswordButtonClickEvent(userCredential);
            RegistrationAnalyticsHandler.SendForgotPasswordButtonClickEvent(new RegistrationEventData(_sender, Provider.Unknown, panelName));
        }

        private void OnManualSignInButtonClickEvent(UserCredential userCredential)
        {
            if (userCredential.IsValid())
            {
                ManagerSounds.PlayEffect("fx_page16");
                _eventMediator.InvokeManualSignInButtonClickEvent(userCredential);
                RegistrationAnalyticsHandler.SignInClick(new RegistrationEventData(_sender, Provider.Email, panelName));
            }
            else
            {
                ManagerSounds.PlayEffect("fx_wrong7");
            }
        }

        private void OnNextButtonClickEvent(UserCredential userCredential)
        {
            bool isCredentialsValid = userCredential.IsValid();
            if (isCredentialsValid)
            {
                ManagerSounds.PlayEffect("fx_page16");
                _eventMediator.InvokeNextButtonClickEvent(userCredential);
                RegistrationAnalyticsHandler.NextButtonClick(new RegistrationEventData(_sender, Provider.Email, panelName));
            }
            else
            {
                ManagerSounds.PlayEffect("fx_wrong7");
            }
        }

        public override void Show(bool activatedFromButton, bool refreshUI, bool isSignIn)
        {
            ManagerSounds.PlayEffect("fx_page15");
            if (_defaultPanel != null && refreshUI)
            {
                _sender = _popUpRegistration.Sender;
                ShowDefaultPanel(new RegistrationEventData(_popUpRegistration.Sender), true);
            }

            base.Show(activatedFromButton, refreshUI, isSignIn);
        }

        public override void Hide(bool refreshUI)
        {
            if (refreshUI)
            {
                RegistrationAnalyticsHandler.SignInClose(new RegistrationEventData("auto", Provider.Unknown, panelName));
            }
            base.Hide(refreshUI);
        }

        private void OnExitFromGameEvent()
        {
            ManagerSounds.PlayEffect("fx_page16");
            RegistrationAnalyticsHandler.SignInClose(new RegistrationEventData(_sender, Provider.Unknown, panelName));
            base.Trigger_ButtonClick_Exit();
        }

        private void OnStartSignInEvent(Transform transform, Provider registrationProvider)
        {
            ManagerSounds.PlayEffect("fx_page16");

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (_popUpRegistration.userParental)
                {
                    Vector3 buttonPosition = transform.position;
                    buttonPosition.x -= 4;
                    ParentalController.Activate(buttonPosition, () =>
                    {
                        OnSignInButtonClick(registrationProvider);
                    }, "2_0");
                }
                else
                {
                    OnSignInButtonClick(registrationProvider);
                }
            }
            else
            {
                _popUpRegistration.Hide(false);
                PopUp_MessageInfo.Activate(MessageInfoType.NoInternet, panelName);
            }
        }

        private void OnSignInButtonClick(Provider registrationProvider)
        {
            RegistrationAnalyticsHandler.SignInClick(new RegistrationEventData(_sender, registrationProvider, panelName));
            _eventMediator.InvokeSingInAction(registrationProvider);
        }


        private void OnSwitchPanelsEvent()
        {
            ManagerSounds.PlayEffect("fx_page16");
            SwitchNewPanels(ReferenceEquals(_defaultPanel, _signInPanel) ? _signUpPanel : _signInPanel);
            SignInControllerStateChangeEvent?.Invoke(ReferenceEquals(_defaultPanel, _signInPanel));
        }

        private void SwitchNewPanels(SignPanelBehaviour newPanel)
        {
            _sender = _signUpPanel.PanelPopupName;
            _defaultPanel.Hide();
            _defaultPanel = newPanel;
            ShowDefaultPanel(new RegistrationEventData(_sender), true);
        }

        private void ShowDefaultPanel(RegistrationEventData registrationEventData, bool isFromButton)
        {
            registrationEventData.SignPrefix = panelName;
            RegistrationAnalyticsHandler.SignInShow(registrationEventData);
            _defaultPanel.UpdateSingInView();
            _defaultPanel.Show(isFromButton);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void SetLoaderActive(bool value)
        {
            if (_defaultPanel != null)
            {
                _defaultPanel.SetLoaderActive(value);
            }
        }
    }

}
