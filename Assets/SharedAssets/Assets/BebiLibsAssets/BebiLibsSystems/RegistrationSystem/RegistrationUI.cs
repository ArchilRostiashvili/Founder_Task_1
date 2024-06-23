using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs.RegistrationSystem;
using BebiLibs.RegistrationSystem.PopUp;
using BebiLibs.Analytics;
using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.RegistrationSystem.Core;

namespace BebiLibs.RegistrationSystem
{
    //[ExecuteInEditMode]
    public class RegistrationUI : MonoBehaviour
    {
        [SerializeField] private ButtonScale _signInButton;
        [SerializeField] private ButtonScale _profileButton;

        [SerializeField] private ButtonScale _signInButtonIpad;
        [SerializeField] private ButtonScale _profileButtonIpad;

        [SerializeField] private ServerConfigData _initDataSO;
        [SerializeField] private GameUserDataSO _userData;

        private void Awake()
        {
            //ManagerRegistration.CallBack_ConfigLoadFinished += OnSingInUpdated;
            ManagerRegistration.SignInStatusChangeEvent += OnSignInUpdate;
            ManagerRegistration.RegistrationUpdateEndEvent += OnRegistrationUpdateEnd;
            ManagerRegistration.CallBack_Error += OnSingInError;
            ManagerRegistration.CallBack_RequireUpdate += OnUpdateRequired;
        }

        private void OnDestroy()
        {
            ManagerRegistration.SignInStatusChangeEvent -= OnSignInUpdate;
            ManagerRegistration.RegistrationUpdateEndEvent -= OnRegistrationUpdateEnd;
            ManagerRegistration.CallBack_Error -= OnSingInError;
            ManagerRegistration.CallBack_RequireUpdate -= OnUpdateRequired;
        }

        private void OnEnable()
        {
            OnSignInUpdate();

            _profileButton.onClick.RemoveAllListeners();
            _profileButton.onClick.AddListener(() => OnRegistrationButtonClick(_profileButton.transform));

            _signInButton.onClick.RemoveAllListeners();
            _signInButton.onClick.AddListener(() => OnRegistrationButtonClick(_signInButton.transform));

            _profileButtonIpad.onClick.RemoveAllListeners();
            _profileButtonIpad.onClick.AddListener(() => OnRegistrationButtonClick(_profileButtonIpad.transform));

            _signInButtonIpad.onClick.RemoveAllListeners();
            _signInButtonIpad.onClick.AddListener(() => OnRegistrationButtonClick(_signInButtonIpad.transform));
        }

        public void OnUpdateRequired()
        {
            PopUp_MessageInfo.Activate(MessageInfoType.UpdateApp, "auto");
        }

        public void OnRegistrationButtonClick(Transform transform)
        {
            AnalyticsManager.LogEvent("click_sing_in_button");
            ParentalController.Activate(transform.position, () =>
            {
                PopUp_Registration.Activate(true, false, "lobby");
            });
            UpdateSignInButtonStates();
        }

        public void OnRegistrationUpdateEnd(bool value)
        {
            OnSignInUpdate();
        }

        public void OnSignInUpdate()
        {
            DisableAllRegistrationButtons();
            UpdateSignInButtonStates();
        }

        public void OnSingInError(string error)
        {
            OnSignInUpdate();
            UpdateSignInButtonStates();
        }

        public void UpdateSignInButtonStates()
        {
            _profileButton.SetActive(_initDataSO.isRegistrationEnabled && _userData.isUserSignedIn);
            _signInButton.SetActive(_initDataSO.isRegistrationEnabled && !_userData.isUserSignedIn);

            // if (deviceType == MobileDeviceType.PHONE)
            // {
            //     _profileButton.SetActive(_userData.isUserSignedIn == true);
            //     _signInButton.SetActive(_userData.isUserSignedIn != true);
            // }
            // else
            // {
            //     _profileButtonIpad.SetActive(_userData.isUserSignedIn == true);
            //     _signInButtonIpad.SetActive(_userData.isUserSignedIn != true);
            // }
        }

        public void DisableAllRegistrationButtons()
        {
            _profileButton.SetActive(false);
            _signInButton.SetActive(false);

            _profileButtonIpad.SetActive(false);
            _signInButtonIpad.SetActive(false);
        }

    }
}
