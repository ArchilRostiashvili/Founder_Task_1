using UnityEngine;
using BebiLibs.RegistrationSystem.PopUp.Components;
using BebiLibs.PopupManagementSystem;
using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.RegistrationSystem.Core;
using System;

namespace BebiLibs.RegistrationSystem.PopUp
{
    public partial class PopUp_Registration : PopUpBase
    {
        private static PopUp_Registration _Instance;
        public string Sender;

        [Header("Registration Panel Components")]
        [SerializeField] private SignPopUpController _signInContent;
        [SerializeField] private ProfilePopUpController _profileContent;
        [SerializeField] private RegistrationSuccessfullPanel _successfullSignInPanel;

        [SerializeField] private RegistrationPanel _activeRegistrationPanel;
        [SerializeField] private RegistrationPanel _lastActivePanel;

        [SerializeField] private GameUserDataSO _userData;
        [SerializeField] private ServerConfigData _configData;
        [SerializeField] private ServerInitData _initializationDataSO;

        [SerializeField] private RegistrationEventMediatorSO _eventModifiers;


        private void OnEnable()
        {
            ManagerRegistration.SignInStatusChangeEvent -= OnUpdatePopUpView;
            ManagerRegistration.SignInStatusChangeEvent += OnUpdatePopUpView;

            ManagerRegistration.RegistrationUpdateStartEvent -= OnStartDataLoading;
            ManagerRegistration.RegistrationUpdateStartEvent += OnStartDataLoading;

            ManagerRegistration.CallBack_Error -= OnRegistrationError;
            ManagerRegistration.CallBack_Error += OnRegistrationError;

            ManagerRegistration.CallBack_BindSuccess -= OnBindSuccessfull;
            ManagerRegistration.CallBack_BindSuccess += OnBindSuccessfull;

            ManagerRegistration.ManualRegistrationErrorEvent -= OnManualRegistrationError;
            ManagerRegistration.ManualRegistrationErrorEvent += OnManualRegistrationError;

            ManagerRegistration.RegistrationUpdateEndEvent -= OnRegistrationUpdateEnd;
            ManagerRegistration.RegistrationUpdateEndEvent += OnRegistrationUpdateEnd;


            _successfullSignInPanel.ExitButtonClickEvent += OnExitButtonClick;
            _successfullSignInPanel.ContinueButtonClickEvent += OnContinueButtonClick;
            _signInContent.SignInControllerStateChangeEvent += OnSignInControllerStateChange;
        }

        private void OnSignInControllerStateChange(bool isSignInState)
        {
            _isSignInPanel = isSignInState;
        }

        private void OnManualRegistrationError()
        {
            _activeRegistrationPanel.SetLoaderActive(false);
            _signInContent.OnManualSignInErrorEvent();
        }

        private void OnDisable()
        {
            //Debug.Log("Disable PopUpView");
            ManagerRegistration.CallBack_Error -= OnRegistrationError;
            ManagerRegistration.SignInStatusChangeEvent -= OnUpdatePopUpView;
            ManagerRegistration.RegistrationUpdateStartEvent -= OnStartDataLoading;
            ManagerRegistration.CallBack_BindSuccess -= OnBindSuccessfull;
            ManagerRegistration.ManualRegistrationErrorEvent -= OnManualRegistrationError;
            ManagerRegistration.RegistrationUpdateEndEvent -= OnRegistrationUpdateEnd;

            _successfullSignInPanel.ExitButtonClickEvent -= OnExitButtonClick;
            _successfullSignInPanel.ContinueButtonClickEvent -= OnContinueButtonClick;

            _signInContent.SignInControllerStateChangeEvent -= OnSignInControllerStateChange;
        }

        public override void Init()
        {
            base.Init();
            _signInContent.Init(this);
            _profileContent.Init(this);
            _successfullSignInPanel.Init(this);
            _Instance = this;
        }

        private static int _ActivatedFromButton = -1;
        private static string _Error;
        private bool _isSignInPanel = true;

        public bool userParental { get; private set; }

        public static void Activate(bool fromButton = false, bool userParentalControl = false, string sender = "auto")
        {
            if (_Instance == null)
            {
                PopupManager.GetPopup<PopUp_Registration>((popup) =>
                {
                    _Instance = popup;
                    ActivatePopup(fromButton, userParentalControl, sender);
                });
            }
            else
            {
                ActivatePopup(fromButton, userParentalControl, sender);
            }
        }

        private static void ActivatePopup(bool fromButton, bool userParentalControl, string sender)
        {
            _ActivatedFromButton = fromButton ? 1 : 0;
            _Instance.Sender = sender;
            _Instance.Show(true);
            _Instance.userParental = userParentalControl;
        }

        private void OnStartDataLoading()
        {
            _activeRegistrationPanel.SetLoaderActive(true);
        }

        private void DeactivateLoadingWheel()
        {
            _activeRegistrationPanel.SetLoaderActive(false);
        }


        private void OnBindSuccessfull()
        {
            HidePanel();
            ManagerSounds.PlayEffect("fx_successhigh2");
            PopUp_MessageInfo.Activate(MessageInfoType.BindSuccessful, "profile_popup");
        }

        private void OnRegistrationError(string error)
        {
            HidePanel();
            PopUp_MessageInfo.Activate(MessageInfoType.SomethingWentWrong, Sender, () =>
            {
                Activate(false, false, "profile_popup");
            }, error);
        }

        public static void HidePanel()
        {
            if (_Instance == null) return;
            _Instance.userParental = false;
            _Instance.Hide(false);
            _ActivatedFromButton = -1;
        }

        override public void Show(bool anim)
        {
            _eventModifiers.InvokePopupOpenAction();
            _popUpCanvas.worldCamera = Camera.main;
            if (_configData.isLoaded && _initializationDataSO.isLoaded)
            {
                UpdatePopUpView();
                TR_Content = _activeRegistrationPanel.TR_Content;
                base.Show(anim);
            }
        }

        public void OnRegistrationUpdateEnd(bool updatePopUpView)
        {
            if (updatePopUpView)
            {
                if (_userData.isUserSignedIn)
                {
                    Debug.LogWarning("OnRegistrationUpdateEnd" + updatePopUpView);
                    _activeRegistrationPanel.Hide(true);
                    _activeRegistrationPanel = _successfullSignInPanel;
                    _activeRegistrationPanel.Show(true, true, _isSignInPanel);
                }
                else
                {
                    Debug.LogWarning("OnRegistrationUpdateEnd" + updatePopUpView);
                    UpdatePopUpView();
                }
            }
        }

        private void OnContinueButtonClick()
        {
            ManagerSounds.PlayEffect("fx_page15");
            _ActivatedFromButton = 0;
            UpdatePopUpView();
            TR_Content = _activeRegistrationPanel.TR_Content;
            gameObject.SetActive(true);
        }

        private void OnExitButtonClick()
        {
            Trigger_ButtonClick_Close();
        }

        public void OnUpdatePopUpView()
        {
            _ActivatedFromButton = 0;
            UpdatePopUpView();
            TR_Content = _activeRegistrationPanel.TR_Content;
            gameObject.SetActive(true);
        }

        public void UpdatePopUpView()
        {
            _activeRegistrationPanel = _userData.isUserSignedIn ? _profileContent : _signInContent;

            DeactivateLoadingWheel();

            _successfullSignInPanel.Hide(false);
            _profileContent.Hide(_activeRegistrationPanel == _signInContent && _ActivatedFromButton != 1);
            _signInContent.Hide(_activeRegistrationPanel == _profileContent && _ActivatedFromButton != 1);

            _activeRegistrationPanel.Show(_ActivatedFromButton == 1, _lastActivePanel != _activeRegistrationPanel, _isSignInPanel);
            _lastActivePanel = _activeRegistrationPanel;
        }

        public override void Hide(bool anim)
        {
            _lastActivePanel = null;
            _ActivatedFromButton = -1;
            //ManagerOtherApps.HideOtherAppsBanner();
            _eventModifiers.InvokeRegistrationPopupCloseAction(false);
            base.Hide(anim);
        }

        public override void Trigger_ButtonClick_Close()
        {
            ManagerSounds.PlayEffect("fx_page17");
            _eventModifiers.InvokeRegistrationPopupCloseAction(true);
            base.Trigger_ButtonClick_Close();
        }

        internal void HideFromButton(bool v)
        {
            _eventModifiers.InvokeRegistrationPopupCloseAction(true);
            Hide(v);
        }
    }
}
