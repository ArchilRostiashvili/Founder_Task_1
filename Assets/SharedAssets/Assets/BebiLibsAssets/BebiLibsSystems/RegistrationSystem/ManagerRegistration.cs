using System.Collections;
using UnityEngine;
using BebiLibs.RegistrationSystem.PopUp;
using BebiLibs.RegistrationSystem;
using BebiLibs.RegistrationSystem.Remote;
using BebiLibs.PurchaseSystem;
using System.Linq;
using BebiLibs.Analytics;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.PopupManagementSystem;
using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.RegistrationSystem.Core;
using BebiLibs.ServerConfigLoaderSystem.Environment;
using BebiLibs.ServerConfigLoaderSystem.Core;
using System;
using System.Collections.Generic;

namespace BebiLibs.RegistrationSystem
{
    public class ManagerRegistration : MonoBehaviour
    {
        public static event Action SignInStatusChangeEvent;

        public static event Action RegistrationUpdateStartEvent;
        public static event Action<bool> RegistrationUpdateEndEvent;

        public static event Action CallBack_BindSuccess;
        public static event Action<string> CallBack_Error;
        public static event Action CallBack_SignInSuccessfull;
        public static event Action CallBack_RequireUpdate;
        public static event Action CallBack_DeleteUserSuccessfully;
        public static event Action ManualRegistrationErrorEvent;


        [HideInInspector] public bool EnablePurchaseValidation = true;

        [SerializeField] private ServerEnvironmentConfig _environmentConfig;
        [SerializeField] private RegistrationEventMediatorSO _eventMediator;

        [Header("Game Shared Data")]
        [SerializeField] private PurchaseHistoryData _purchaseData;
        [SerializeField] private PurchaseManagerBase _purchaseManager;
        [SerializeField] private GameUserDataSO _userData;

        [Header("Registration Specific Data")]
        [SerializeField] private ServerConfigData _configData;
        [SerializeField] private ServerInitData _initData;
        [SerializeField] private RemotePurchaseDataSO _remotePurchases;
        [SerializeField] private TokenRequestData _tokenRequestData;
        [SerializeField] private TokenTransferData _tokenTransferData;
        [SerializeField] private SubscriptionPostDataSO _subPostDataSO;
        [SerializeField] private DeleteAccountDataSO _deleteAccountDataSO;
        [SerializeField] private ManualSignInPostDataSO _manualSignInPostDataSO;

        [Header("Registration Non Data Members")]
        [SerializeField] private BasePurchaseMergeHandler _purchaseMergeHandler;
        [SerializeField] private RegistrationDeepLinkListener _deepLinkListener;
        [SerializeField] private BaseRequestHandler _requestLoader;

        [Header("Editor: Data")]

        [SerializeField] private float _editorEndLoadDelay = 0f;
        [SerializeField] private float _editorStartLoadDelay = 0f;

        private PersistentBoolean _isReceiptSent = new PersistentBoolean("ManagerRegistration_IsSubscription_Data_Send", false);
        private ScreenOrientation _defaultScreenOrientation = ScreenOrientation.LandscapeLeft;
        private bool _isFetchOperationInProgress;
        private bool _wasDataRefetchRequested;
        private bool _isManualSignInStarted;


        private void Awake()
        {
            _defaultScreenOrientation = Screen.orientation;
#if AMAZON_BUILD
            EnablePurchaseValidation = false;
#endif

            PopupManager.GetPopup<PopUp_Registration>((popup) =>
            {
                popup.Init();
            });

            _purchaseData.PurchaseDataChangedEvent += OnPurchaseDataChanged;
            _deepLinkListener.Callback_OnDeepLinkLoginDone += OnUpdateUserToken;
            _deepLinkListener.Callback_OnDeepLinkLoginFail += OnDeepLinkSignInFailed;

            _eventMediator.SignInButtonClickEvent += OnSingInButtonClick;
            _eventMediator.SignOutButtonClickEvent += OnSignOutButtonClick;
            _eventMediator.BindButtonClickEvent += OnBindButtonClick;
            _eventMediator.DeleteButtonClickEvent += OnDeleteUserAccountClick;
            _eventMediator.PopupOpenEvent += OnPopupOpen;
            _eventMediator.CreateAccountButtonClickEvent += OnCreateAccountButtonClick;
            _eventMediator.ManualSignInButtonClickEvent += OnManualSignInButtonClick;
            _eventMediator.ForgetPasswordButtonClickEvent += OnForgetPasswordButtonClick;

        }

        private void OnPopupOpen()
        {
            _purchaseMergeHandler.MergePurchases();
        }

        private IEnumerator Start()
        {
            ResetAndLoadDataFromMemory();
            yield return StartFetchingRegistrationData();
        }

        public IEnumerator StartFetchingRegistrationData()
        {
            if (_isFetchOperationInProgress)
            {
                _wasDataRefetchRequested = true;
                yield break;
            }
            _isFetchOperationInProgress = true;

            yield return LoadRegistrationData();
            _isFetchOperationInProgress = false;

            if (_wasDataRefetchRequested)
            {
                _wasDataRefetchRequested = false;
                yield return StartFetchingRegistrationData();
            }
        }


        private IEnumerator LoadRegistrationData()
        {
            yield return new WaitForDone(30, () => _initData.isLoaded);

            if (!_initData.isLoaded)
                yield break;


            yield return new WaitForDone(15f, () => _purchaseManager.IsInitialized);

            float RemoteConfigWaitTime = RemoteConfigManager.TimeOutInSeconds + 1f;
            yield return new WaitForDone(RemoteConfigWaitTime, () => RemoteConfigManager.IsLoadFinished);

            yield return WaitForEditorDelay(_editorStartLoadDelay);

            RemoteConfigManager.TryGetBool("registration_switch", out bool enableRegistration);
            RemoteConfigManager.TryGetBool("georgian_mode", out bool _isGeorgianModeEnabled);

            _configData.SetRegistrationActive(enableRegistration && !_isGeorgianModeEnabled);

            if (!enableRegistration || _isGeorgianModeEnabled)
            {
                RegistrationUpdateEndEvent?.Invoke(false);
                yield break;
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            yield return WaitForEditorDelay(_editorEndLoadDelay);

            yield return CheckForUpdate();
            yield return CheckAuthorization();
            yield return TransferToken();
            yield return RefreshUserToken();

            yield return SendUserPurchaseRequest(_initData.receiptURL);
            yield return UpdateUserPurchaseData();
            RegistrationUpdateEndEvent?.Invoke(false);
        }

        private IEnumerator WaitForEditorDelay(float delay)
        {
            if (Application.isEditor)
            {
                yield return new WaitForSeconds(delay);
            }
        }

        private void ResetAndLoadDataFromMemory()
        {
            _manualSignInPostDataSO.ResetData();
            _remotePurchases.ResetData();
            _tokenRequestData.ResetData();
            _userData.RefreshDataOnInitialize();
            _deleteAccountDataSO.ResetData();
        }

        private void OnDeepLinkSignInFailed(string error)
        {
            BebiPlugins.CloseBrowser();
            AnalyticsManager.LogEvent("sign_in_fail", "error", "user cancel");
            SignUserOut();
            CallBack_Error?.Invoke("Sign in failed");
            SignInStatusChangeEvent.Invoke();
        }

        private void OnUpdateUserToken(string newUserToken)
        {
            BebiPlugins.CloseBrowser();
            bool isUserTokenValid = _userData.UpdateUserToken(newUserToken);
            if (isUserTokenValid)
            {
                StartUserPurchaseStateUpdate(_initData.receiptURL, false);
                _purchaseManager.SetAccountID();
                RegistrationUpdateStartEvent?.Invoke();
                CallBack_SignInSuccessfull?.Invoke();
            }
            else
            {
                Debug.LogError("Open Error Panel, User Token Not Valid");
                CallBack_Error?.Invoke("Invalid Token");
            }
            SignInStatusChangeEvent?.Invoke();
        }


        private void OnPurchaseDataChanged()
        {
            //Debug.LogError("Purchase Data Changed");
            StartUserPurchaseStateUpdate(_initData.receiptURL, true);
        }

        public void StartUserPurchaseStateUpdate(string receiptPostURL, bool forceSend)
        {
            StartCoroutine(UpdatePurchaseStates(receiptPostURL));
            IEnumerator UpdatePurchaseStates(string receiptPost)
            {
                RegistrationUpdateStartEvent?.Invoke();
                yield return SendUserPurchaseRequest(receiptPost, forceSend);
                yield return UpdateUserPurchaseData();
                RegistrationUpdateEndEvent?.Invoke(false);
            }
        }

        private IEnumerator TransferToken()
        {
            if (!_userData.isTokenTransferred) yield break;

            yield return _tokenTransferData.PostDataToServer(_environmentConfig.GetTokenTransferURL(_configData.urlEndpoint), "");
            if (!_tokenTransferData.isLoaded) yield break;
            OnUpdateUserToken(_tokenTransferData.token);
        }

        private IEnumerator CheckAuthorization()
        {
            yield return null;
            _userData.UpdateUserState();
        }

        private IEnumerator CheckForUpdate()
        {
            if (_initData.update)
            {
                bool hasVersionString = _initData.lastVersion.Length > 0 || !string.IsNullOrEmpty(_initData.lastVersion);
                if (!hasVersionString || _initData.lastVersion == Application.version)
                {
                    CallBack_RequireUpdate?.Invoke();
                }
                else
                {
                    _initData.update = false;
                }

                _initData.lastVersion = Application.version;
                _initData.SaveDataToMemory();
            }
            yield return null;
        }

        private IEnumerator RefreshUserToken()
        {
            if (!_userData.canRefreshUserToken) yield break;
            yield return _tokenRequestData.PostDataToServer(_environmentConfig.GetTokenRefreshURL(_configData.urlEndpoint), "");
            if (!_tokenRequestData.isLoaded) yield break;
            OnUpdateUserToken(_tokenRequestData.token);
        }

        public IEnumerator SendUserPurchaseRequest(string url, bool forceSend = false, Action<ResponseData> onResponseReceive = null)
        {
            if (!_configData.isRegistrationEnabled || !_userData.isUserSignedIn || !EnablePurchaseValidation) yield break;

            yield return new WaitForDone(_initData.network_timeout + 2, () => _initData.isLoaded);
            yield return new WaitForDone(30f, () => _purchaseData.IsPurchaseDataInitialized);

            if (_purchaseData.HasAnyActiveSubscription && _initData.isLoaded && (!_isReceiptSent || forceSend))
            {
                yield return PostSubscriptionDataToServer(url, forceSend, onResponseReceive);
            }
            else
            {
                onResponseReceive?.Invoke(new ResponseData("", RequestStatus.Failed, 500));
            }
        }

        private IEnumerator PostSubscriptionDataToServer(string url, bool forceSend, System.Action<ResponseData> onResponseReceive)
        {
            _isReceiptSent.SetValue(true);


            List<string> subscriptionInfoList = _purchaseData.GetSubscriptionReceiptInfoList();

            if (subscriptionInfoList.Count == 0)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "No Subscription Data to Send");
                yield break;
            }


            string subscriptionInfo = subscriptionInfoList.FirstOrDefault();

            string log = "Sending Subscription Receipt: " + (subscriptionInfo.Length > 100 ? subscriptionInfo.Substring(0, 100) : subscriptionInfo);
            SafeLogNoStack.Log(log);
            yield return _subPostDataSO.PostDataToServer(url, subscriptionInfo);

            HandleProtocolErrors(_subPostDataSO.RequestResponseData.ResponseCode);
            onResponseReceive?.Invoke(_subPostDataSO.RequestResponseData);
        }

        public RemotePurchaseDataSO GetRemotePurchaseData()
        {
            return _remotePurchases;
        }

        public IEnumerator UpdateUserPurchaseData()
        {
            if (!_configData.isRegistrationEnabled || !EnablePurchaseValidation) yield break;
            if (_userData.isUserSignedIn)
            {
                yield return _remotePurchases.GetDataFromServer(_environmentConfig.GetPurchaseURL(_configData.urlEndpoint));
                long responseCode = _remotePurchases.RequestResponseData.ResponseCode;
                HandleProtocolErrors(responseCode);
                _isReceiptSent.SetValue(responseCode != 200 || _remotePurchases.hasError);
                if (_remotePurchases.isLoaded && !_remotePurchases.hasError)
                {
                    _purchaseMergeHandler.MergePurchases();
                }
                _remotePurchases.LogData("Loaded");
            }

            RegistrationUpdateEndEvent?.Invoke(false);
        }


        private void HandleProtocolErrors(long responseCode)
        {
            if (responseCode == 401 || responseCode == 403)
            {
                SignUserOut();
                CallBack_Error?.Invoke(responseCode.ToString());
            }
            else if (responseCode != 200)
            {
                CallBack_Error?.Invoke(responseCode.ToString());
            }
        }


        private void OnBindButtonClick()
        {
            StartCoroutine(StartBindPurchaseProcess());
        }

        private IEnumerator StartBindPurchaseProcess()
        {
            RegistrationUpdateStartEvent?.Invoke();
            yield return SendUserPurchaseRequest(_initData.bindURL, true, (ResponseData responseData) =>
            {
                if (responseData.ResponseCode == 200 && _userData.isUserSignedIn)
                {
                    CallBack_BindSuccess?.Invoke();
                }
                else
                {
                    CallBack_Error?.Invoke(responseData.ResponseCode.ToString());
                    SignUserOut();
                }
            });

            yield return UpdateUserPurchaseData();
            RegistrationUpdateEndEvent?.Invoke(true);
        }

        private void OnSingInButtonClick(Provider provide)
        {
            _userData.singInProvider = provide;
            Login login = _initData.logins.Find(x => x.provider == provide.ToString());
            if (login != null)
            {
                BebiPlugins.OpenURL(login.url);
                BebiPlugins.BrowserCloseEvent += OnBrowserClose;
                Loader.Show(LoaderData.OnlyColor(new Color32(217, 217, 217, 255), 3f));
            }
            else
                Debug.LogError($"Unable To Find provider named {provide.ToString()} in Loaded Json Data");
        }

        private void OnDeleteUserAccountClick()
        {
            PopUp_Registration.HidePanel();
            RegistrationInfoPopup.Activate(RegistrationInfoType.DeleteUserPrompt, true, "registration_popup", OnDeleteUserAccountConfirm, OnDeleteUserAccountCancel, OnDeleteUserAccountCancel);
        }

        private void OnDeleteUserAccountConfirm()
        {
            RegistrationInfoPopup.Activate(RegistrationInfoType.Loader, false, "registration_popup");
            StartCoroutine(DeleteUserAccount());
        }

        private void OnDeleteUserAccountCancel()
        {
            RegistrationInfoPopup.HidePopup();
            PopUp_Registration.Activate(true, false, "delete_user_popup");
        }

        private IEnumerator DeleteUserAccount()
        {
            if (!_userData.isUserSignedIn || !_configData.isRegistrationEnabled) yield break;

            yield return _deleteAccountDataSO.PostDataToServer(_environmentConfig.GetDeleteUserAccountURL(_configData.urlEndpoint), "");
            long responseCode = _deleteAccountDataSO.RequestResponseData.ResponseCode;
            //HandleProtocolErrors(responseCode);

            if (responseCode == 200)
            {
                RegistrationInfoPopup.Activate(RegistrationInfoType.SuccessfullyRemoveUserAccount, false, "registration_popup", null, OnDeleteUserAccountCancel, OnDeleteUserAccountCancel);

                SignUserOut();
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "User Account Deleted Successfully");
                CallBack_DeleteUserSuccessfully?.Invoke();
            }
            else if (responseCode == 405 && !string.IsNullOrEmpty(_deleteAccountDataSO.GuideUrl))
            {
                RegistrationInfoPopup.Activate(RegistrationInfoType.SubscriptionWarning, false, "registration_popup", null, OnDeleteUserAccountCancel, OnDeleteUserAccountCancel);
            }
            else if (responseCode == 400 || responseCode == 401 || responseCode == 403)
            {
                RegistrationInfoPopup.Activate(RegistrationInfoType.DeleteUserError, false, "registration_popup", null, OnDeleteUserAccountCancel, OnDeleteUserAccountCancel);
            }
            else
            {
                HandleProtocolErrors(responseCode);
            }
        }

        private void OnManualSignInButtonClick(UserCredential userCredential)
        {
            //Debug.Log("OnManualSignInButtonClick " + userCredential);
            _userData.singInProvider = Provider.Email;
            if (_isManualSignInStarted) return;
            _isManualSignInStarted = true;
            StartCoroutine(OnManualSignInStart(userCredential));
        }

        private IEnumerator OnManualSignInStart(UserCredential userCredential)
        {
            RegistrationUpdateStartEvent?.Invoke();

            if (!_configData.isRegistrationEnabled || !EnablePurchaseValidation || _userData.isUserSignedIn) yield break;

            if (!userCredential.TryGetJsonData(out string jsonData))
            {
                Debug.LogError("OnManualSignInStart: Failed to get json data from user credential");
                _isManualSignInStarted = false;
                yield break;
            }

            yield return _manualSignInPostDataSO.PostDataToServer(_environmentConfig.GetEmailLoginURL(_configData.urlEndpoint), jsonData);

            if (_manualSignInPostDataSO.HasError)
            {
                ManualRegistrationErrorEvent?.Invoke();
            }
            else
            {
                OnUpdateUserToken(_manualSignInPostDataSO.Token);
            }

            _isManualSignInStarted = false;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            RegistrationUpdateEndEvent?.Invoke(true);
        }

        private void OnCreateAccountButtonClick(UserCredential userCredential)
        {
            _userData.singInProvider = Provider.Email;
            Application.OpenURL(_initData.RegistrationURL + $"&username={userCredential.Email}");
            Debug.Log("OnCreateAccountButtonClick " + userCredential);
        }

        private void OnForgetPasswordButtonClick(UserCredential userCredential)
        {
            _userData.singInProvider = Provider.Email;
            Application.OpenURL(_initData.ResetUrl + $"&username={userCredential.Email}");
            Debug.Log("OnForgetPasswordButtonClick " + userCredential);
        }


        private void OnBrowserClose()
        {
            Screen.orientation = _defaultScreenOrientation;
            Loader.Hide();
            BebiPlugins.BrowserCloseEvent -= OnBrowserClose;
        }

        private void OnSignOutButtonClick()
        {
            SignUserOut();
        }

        private void SignUserOut()
        {
            bool userHadProfile = _userData.isUserSignedIn;
            _isReceiptSent.SetValue(false);
            _userData.SignUserOut();
            _remotePurchases.SignUserOut();

            if (!_purchaseData.HasAnyActiveSubscription && userHadProfile)
            {
                ManagerTime.DelayUntil(this, 10, () => RemoteConfigManager.IsLoadFinished, () =>
                {
                    if (_purchaseMergeHandler.IsActive)
                    {
                        _purchaseManager.ForceChangeLockState("registration_sign_out", false);
                    }
                });
            }
            SignInStatusChangeEvent?.Invoke();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
