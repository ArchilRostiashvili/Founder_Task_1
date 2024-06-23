using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BebiLibs.PurchaseSystem;
using BebiLibs.RegistrationSystem.Core;
using System;

namespace BebiLibs.RegistrationSystem.PopUp.Components
{
    public class ProfilePopUpController : RegistrationPanel
    {
        [SerializeField] private RegistrationEventMediatorSO _eventMediator;
        [SerializeField] private ProviderIconDataSO _iconDataSO;
        [SerializeField] private GameUserDataSO _userData;
        [SerializeField] private PurchaseHistoryData _purchaseDataSO;


        [SerializeField] private TMP_Text Text_UserName;
        [SerializeField] private TMP_Text Text_Email;

        [SerializeField] private GameObject GO_Submit;
        [SerializeField] private GameObject GO_SignOut;

        [Header("Bind Options")]
        [SerializeField] private GameObject GO_BindPanel;
        [SerializeField] private ButtonScale _deleteAccountButton;


        [Header("Loader Management")]
        [SerializeField] private GameObject GO_Loader;
        [SerializeField] private GameObject GO_ProfileContent;
        [SerializeField] private GameObject GO_SignOutButton;
        [SerializeField] private Image Image_SignInProvider;

        public bool activatedFromButton = false;

        private void OnEnable()
        {
            _deleteAccountButton.onClick.AddListener(OnDeleteAccountButtonClick);
        }

        private void OnDisable()
        {
            _deleteAccountButton.onClick.RemoveListener(OnDeleteAccountButtonClick);
        }


        public override void Show(bool activatedFromButton, bool refreshUI, bool isSignIn)
        {
            if (refreshUI && _userData.isUserSignedIn)
            {
                RegistrationAnalyticsHandler.ShowProfilePopUp(new RegistrationEventData()
                {
                    Sender = _popUpRegistration.Sender,
                    Provider = _userData.singInProvider,
                    SignPrefix = "profile"
                }, _userData);
            }

            this.activatedFromButton = activatedFromButton;
            base.Show(activatedFromButton, refreshUI, isSignIn);
            UpdateUserState();
            UpdateBindState();
            UpdateDeleteUserButtonState();
        }

        public void UpdateUserState()
        {
            ManagerSounds.PlayEffect(activatedFromButton ? "fx_page15" : "fx_successhigh2");

            GO_Submit.SetActive(!activatedFromButton);
            GO_SignOutButton.SetActive(activatedFromButton);

            if (_userData.isUserSignedIn)
            {
                Text_UserName.text = string.IsNullOrWhiteSpace(_userData.userName) ? GetUserNameFromEmail(_userData.userEmail) : _userData.userName;
                Text_Email.text = _userData.userEmail;
            }
            else
            {
                _popUpRegistration.Hide(false);
            }

            if (_iconDataSO.TryGetIcon(_userData.singInProvider, out Sprite sprite))
            {
                Image_SignInProvider.gameObject.SetActive(true);
                Image_SignInProvider.sprite = sprite;
            }
            else
            {
                Image_SignInProvider.gameObject.SetActive(false);
            }
        }

        private string GetUserNameFromEmail(string email)
        {
            try
            {
                string[] split = email.Split('@');
                return split[0];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public void UpdateBindState()
        {
            if (_purchaseDataSO.HasAnyActiveSubscription && _userData.isBindButtonEnabled)
            {
                GO_BindPanel.SetActive(true);
            }
            else
            {
                GO_BindPanel.SetActive(false);
            }
        }

        public void UpdateDeleteUserButtonState()
        {
            _deleteAccountButton.gameObject.SetActive(_userData.isUserSignedIn);
        }

        private void OnDeleteAccountButtonClick()
        {
            ManagerSounds.PlayEffect("fx_page16");
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                RegistrationAnalyticsHandler.BindButtonClick(new RegistrationEventData()
                {
                    Sender = _popUpRegistration.Sender,
                    Provider = _userData.singInProvider,
                    SignPrefix = "bind_click"
                }, _userData);

                _eventMediator.InvokeDeleteAction();
            }
            else
            {
                _popUpRegistration.Hide(false);
                PopUp_MessageInfo.Activate(MessageInfoType.NoInternet, "profile_popup");
            }
        }

        public void Trigger_ButtonClick_Bind()
        {
            ManagerSounds.PlayEffect("fx_page16");
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                RegistrationAnalyticsHandler.BindButtonClick(new RegistrationEventData()
                {
                    Sender = _popUpRegistration.Sender,
                    Provider = _userData.singInProvider,
                    SignPrefix = "bind_click"
                }, _userData);

                GO_BindPanel.SetActive(false);
                _eventMediator.InvokeBindAction();
            }
            else
            {
                _popUpRegistration.Hide(false);
                PopUp_MessageInfo.Activate(MessageInfoType.NoInternet, "profile_popup");
            }
        }

        public override void Hide(bool refreshUI)
        {
            if (refreshUI)
            {
                RegistrationAnalyticsHandler.CloseProfilePopUp(new RegistrationEventData()
                {
                    Sender = "auto",
                    Provider = _userData.singInProvider,
                    SignPrefix = "profile"
                }, _userData);
            }
            base.Hide(refreshUI);
        }

        public override void Trigger_ButtonClick_Exit()
        {
            RegistrationAnalyticsHandler.CloseProfilePopUp(new RegistrationEventData()
            {
                Sender = _popUpRegistration.Sender,
                Provider = _userData.singInProvider,
                SignPrefix = "profile"
            }, _userData);

            ManagerSounds.PlayEffect("fx_page17");
            base.Trigger_ButtonClick_Exit();
        }

        public void Trigger_ButtonClick_SignOut()
        {
            RegistrationAnalyticsHandler.SignUserOut(new RegistrationEventData()
            {
                Sender = _popUpRegistration.Sender,
                Provider = _userData.singInProvider,
                SignPrefix = "sign_out"
            }, _userData);

            ManagerSounds.PlayEffect("fx_page17");
            _eventMediator.InvokeSignOutAction();
        }

        public override void SetLoaderActive(bool value)
        {
            GO_Loader.SetActive(value);
            GO_ProfileContent.SetActive(!value);
            GO_SignOutButton.SetActive(!value);
        }
    }
}
