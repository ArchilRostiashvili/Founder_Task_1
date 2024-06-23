using BebiLibs.Analytics;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.ServerConfigLoaderSystem.Core;
using BebiLibs.Analytics.GameEventLogger;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


namespace BebiLibs.PurchaseSystem
{
    public class PurchaseAnalytics : AnalyticsHelperBase, IParameterProvider
    {
        public const string IAP_ID = "inappID";
        public const string TRANSACTION_ID = "instanceID";
        public const string IS_SUBSCRIBED = "isSubscribed";
        public const string IS_PAID = "isPaid";
        public const string IS_EXPIRED = "isExpired";
        public const string IS_FREE_TRIAL = "isFreeTrial";
        public const string IS_CANCELED = "isCanceled";
        public const string DEVICE_ID = "deviceID";
        public const string SUB_STATE = "sub_state";

        public static event System.Action<PurchaseData> SubStateUpdateEvent;
        public static string BannerHistory;

        [SerializeField] private PurchaseManagerBase _purchaseManager;
        [SerializeField] private PurchaseHistoryData _purchaseHistory;
        [SerializeField] private AbstractInitData _initData;


        private PersistentBoolean _isFirstInstal = new PersistentBoolean("First_Install_Pref_Key", false);
        private GameParameterBuilder _subStateAnalyticsParameters = new GameParameterBuilder();
        private GameParameterBuilder _subStateChangeParameters = new GameParameterBuilder();
        private GameParameterBuilder _lockStateChangeParameters = new GameParameterBuilder();

        private string _sender;

        private bool _isInitiatedFromUser => !string.IsNullOrEmpty(_sender);
        public GameParameterBuilder GetParameters() => _subStateAnalyticsParameters;


        private void Awake()
        {
            _purchaseManager.PurchaseProcessStartEvent -= OnBuyProductButtonClick;
            _purchaseManager.PurchaseProcessStartEvent += OnBuyProductButtonClick;

            _purchaseManager.PurchaseStateUpdatedEvent -= OnProcessPurchase;
            _purchaseManager.PurchaseStateUpdatedEvent += OnProcessPurchase;

            _purchaseManager.PurchaseInitializeEvent -= OnPurchaseInitialized;
            _purchaseManager.PurchaseInitializeEvent += OnPurchaseInitialized;

            _purchaseManager.PurchaseInitializationFailedEvent -= OnPurchaseInitializeFailed;
            _purchaseManager.PurchaseInitializationFailedEvent += OnPurchaseInitializeFailed;

            _purchaseManager.PurchaseProcessFailedEvent -= OnPurchaseFailed;
            _purchaseManager.PurchaseProcessFailedEvent += OnPurchaseFailed;

            _purchaseHistory.PurchaseDataChangedEvent -= OnPurchaseDataChanged;
            _purchaseHistory.PurchaseDataChangedEvent += OnPurchaseDataChanged;

            _purchaseManager.ForceChangeLockStateEvent -= OnForceLockStateChange;
            _purchaseManager.ForceChangeLockStateEvent += OnForceLockStateChange;


            AnalyticsManager.RegisterParameterProvider(this);
        }

        private void OnDisable()
        {
            _purchaseHistory.PurchaseDataChangedEvent -= OnPurchaseDataChanged;
            _purchaseManager.PurchaseInitializeEvent -= OnPurchaseInitialized;
            _purchaseManager.PurchaseInitializationFailedEvent -= OnPurchaseInitializeFailed;
            _purchaseManager.PurchaseProcessStartEvent -= OnBuyProductButtonClick;
            _purchaseManager.PurchaseStateUpdatedEvent -= OnProcessPurchase;
            _purchaseManager.PurchaseProcessFailedEvent -= OnPurchaseFailed;
            _purchaseManager.ForceChangeLockStateEvent -= OnForceLockStateChange;
            AnalyticsManager.UnregisterParameterProvider(this);
        }

        public void OnPurchaseInitialized()
        {
            PurchaseData activePurchase = GetActivePurchaseData();
            PurchaseData previousPurchase = GetPreviousPurchaseData(activePurchase);
            UpdateSubStateValues(activePurchase, previousPurchase);
        }

        public void OnPurchaseInitializeFailed(PurchaseErrorData purchaseErrorData)
        {
            AnalyticsManager.LogEvent("purchase_initialization_failed", "error", purchaseErrorData.ToString());
        }

        public void OnPurchaseDataChanged()
        {
            PurchaseData activePurchase = GetActivePurchaseData();
            PurchaseData previousPurchase = GetPreviousPurchaseData(activePurchase);
            UpdateSubStateValues(activePurchase, previousPurchase);

            bool isPaidUser = _purchaseHistory.SubscriptionTotalCount > 0;
            SendPurchaseChangeEvent(activePurchase, previousPurchase, isPaidUser);
        }

        private void OnForceLockStateChange(string sender, bool isUnlocked)
        {
            _lockStateChangeParameters.Clear();
            _lockStateChangeParameters.Add("changed_from", sender);
            _lockStateChangeParameters.Add("is_unlocked", isUnlocked.ToString());
            AnalyticsManager.LogEvent("force_change_lock_state", _lockStateChangeParameters);
        }

        private void SendPurchaseChangeEvent(PurchaseData activePurchase, PurchaseData previousPurchase, bool isPaid)
        {
            bool hasPurchase = activePurchase != null;
            _subStateChangeParameters.Clear();

            if (hasPurchase)
            {
                _subStateChangeParameters.Add(IAP_ID, activePurchase.ProductID);
                _subStateChangeParameters.Add(TRANSACTION_ID, activePurchase.TransactionID);
                _subStateChangeParameters.Add(IS_SUBSCRIBED, activePurchase.IsSubscribed.ToString());
                _subStateChangeParameters.Add(IS_FREE_TRIAL, activePurchase.IsFreeTrial.ToString());
                _subStateChangeParameters.Add(IS_CANCELED, activePurchase.IsCanceled.ToString());
            }
            else
            {
                _subStateChangeParameters.Add(IAP_ID, "null");
                _subStateChangeParameters.Add(TRANSACTION_ID, "null");
                _subStateChangeParameters.Add(IS_SUBSCRIBED, "false");
                _subStateChangeParameters.Add(IS_FREE_TRIAL, "false");
                _subStateChangeParameters.Add(IS_CANCELED, "false");
            }
            _subStateChangeParameters.Add(IS_PAID, isPaid.ToString());

            AnalyticsManager.LogEvent("sub" + "_" + GetUpdatedStateName(activePurchase, previousPurchase), _subStateChangeParameters);
        }

        public PurchaseData GetActivePurchaseData()
        {
            List<PurchaseData> purchaseDataList = _purchaseHistory.PurchasesDataList;
            PurchaseData activePurchase = purchaseDataList.Where(x => x.PurchaseType == LocalPurchaseType.Subscription).OrderByDescending(x => x.StartData.EpochTime).FirstOrDefault();

            return activePurchase;
        }

        public PurchaseData GetPreviousPurchaseData(PurchaseData activePurchase)
        {
            PurchaseData previousPurchase = activePurchase != null ? _purchaseHistory.PreviousPurchasesDataList.Find(x => x.LocalProductID == activePurchase.LocalProductID && x.PurchaseType == LocalPurchaseType.Subscription) : null;
            return previousPurchase;
        }

        public void UpdateSubStateValues(PurchaseData activePurchase, PurchaseData previousPurchase)
        {
            _subStateAnalyticsParameters.Clear();

            if (_initData.GetDeviceID(out string deviceID))
            {
                AnalyticsManager.SetProperty(DEVICE_ID, deviceID);
                _subStateAnalyticsParameters.Add(DEVICE_ID, deviceID);
            }

            string subState = GetUpdatedStateName(activePurchase, previousPurchase);
            _subStateAnalyticsParameters.Add(SUB_STATE, subState);

            AnalyticsManager.SetProperty(SUB_STATE, subState);

            SubStateUpdateEvent?.Invoke(activePurchase);
        }

        private static string GetUpdatedStateName(PurchaseData activePurchase, PurchaseData previousPurchase)
        {
            if (activePurchase != null)
            {
                if (activePurchase.IsSubscribed)
                {
                    string output = activePurchase.IsFreeTrial ? "trial_" : "paid_";
                    output += activePurchase.AnalyticsID;
                    if (activePurchase.IsCanceled)
                    {
                        output += "_canceled";
                    }
                    return output;
                }
                else if (previousPurchase != null)
                {
                    string output = previousPurchase.IsFreeTrial ? "trial_" : "paid_";
                    return output += previousPurchase.AnalyticsID + "_expired";
                }
            }
            return "free";
        }

        public void OnPurchaseFailed(PurchaseErrorData failureReason, ProductIdentifier productInfo)
        {
            string product = productInfo != null ? productInfo.AnalyticsID : "Unknown";
            SendSubscriptionFailedEvent("sub_fail", product, _sender, failureReason.ToString());
        }

        public void OnBuyProductButtonClick(ProductIdentifier productInfo, string sender)
        {
            if (productInfo.ProductType == LocalPurchaseType.Subscription)
            {
                SendProductPurchaseStartEvent("sub_start", productInfo.AnalyticsID, sender);
                SendProductPurchaseStartEvent("sub_start_" + productInfo.AnalyticsID, productInfo.AnalyticsID, sender);
            }
            else
            {
                SendProductPurchaseStartEvent("iap_start", productInfo.AnalyticsID, sender);
                SendProductPurchaseStartEvent("iap_start_" + productInfo.AnalyticsID, productInfo.AnalyticsID, sender);
            }
            _sender = sender;
        }


        public void OnProcessPurchase(ProductIdentifier productInfo, bool fromPurchaseManager)
        {
            if (!fromPurchaseManager || productInfo == null) return;
            if (productInfo.ProductType == LocalPurchaseType.Subscription)
                HandleSubscription(productInfo);
            else
                HandheldNonConsumable(productInfo);
        }

        private void HandheldNonConsumable(ProductIdentifier productInfo)
        {
            if (_isInitiatedFromUser && _purchaseHistory.TryGetPurchaseData(productInfo.LocalProductID, out PurchaseData purchaseData) && purchaseData.HasReceipt && purchaseData.IsPurchaseValid)
            {
                SendPurchaseDoneEvent("iap_done_" + productInfo.AnalyticsID, "iap_done", productInfo.AnalyticsID, _sender, purchaseData.PurchaseMetadata.localizedPriceString, purchaseData.PurchaseMetadata.isoCurrency, 0);
            }
        }

        private void HandleSubscription(ProductIdentifier productInfo)
        {
            if (_purchaseHistory.TryGetPurchaseData(productInfo.LocalProductID, out PurchaseData purchaseData) && purchaseData.HasReceipt && purchaseData.IsPurchaseValid)
            {
                //UnityEngine.Debug.LogError("PurchaseData: " + (_isInitiatedFromUser));
                if (_isInitiatedFromUser)
                {
                    HandleUserSubscription(productInfo, purchaseData);
                }
                else
                {
                    HandleAutoSubscription(productInfo, purchaseData);
                }
            }
        }

        private void HandleUserSubscription(ProductIdentifier productInfo, PurchaseData purchaseData)
        {
            //UnityEngine.Debug.LogError("HandleUserSubscription");

            int subscriptionCount = _purchaseHistory.SubscriptionTotalCount;
            PurchaseMetadata metadata = purchaseData.PurchaseMetadata;
            SendSubscriptionDoneEvent("sub_done_" + productInfo.AnalyticsID, "sub_done", productInfo.AnalyticsID, _sender, metadata.localizedPriceString, metadata.isoCurrency, subscriptionCount, BannerHistory);
        }

        private void HandleAutoSubscription(ProductIdentifier productInfo, PurchaseData purchaseData)
        {
            //UnityEngine.Debug.LogError("HandleAutoSubscription");

            if (!_isFirstInstal.isInitialized || _purchaseHistory.SubscriptionTotalCount == 1)
            {
                int subscriptionCount = _purchaseHistory.SubscriptionTotalCount;
                PurchaseMetadata metadata = purchaseData.PurchaseMetadata;
                SendSubscriptionDoneEvent("sub_done_auto_restore_" + productInfo.AnalyticsID, "sub_done_auto_restore", productInfo.AnalyticsID, _sender, metadata.localizedPriceString, metadata.isoCurrency, subscriptionCount, string.Empty);
            }
            _isFirstInstal.SetValue(true);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        private void CancellationEvent(string subscriptionType, string price = "", string priceCurr = "", int count = 0)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(8);
            parameters.Add("sub_type", subscriptionType);
            parameters.Add("cancel_price", price);
            parameters.Add("cancel_price_currency", priceCurr);
            parameters.Add("cancel_count", count);
            AnalyticsManager.LogEvent("sub_cancel", parameters);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        private void SendProductPurchaseStartEvent(string eventName, string productID, string actionOrigin)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(1);
            parameters.Add("bid", actionOrigin);
            parameters.Add("sub_id", productID);
            AnalyticsManager.LogEvent(eventName, parameters);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        private void SendSubscriptionDoneEvent(string eventName, string oldEventName, string subID, string activationOrigin, string subPrice, string currency, int subCount, string bannerHistory)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(3);
            parameters.Add("bid", activationOrigin);
            parameters.Add("sub_price", subPrice);
            parameters.Add("sub_price_currency", currency);
            parameters.Add("sub_count", subCount);
            parameters.Add("banner", bannerHistory);
            parameters.Add("sub_id", subID);
#if ACTIVATE_ANALYTICS
            parameters.Add(() => Firebase.Analytics.FirebaseAnalytics.ParameterValue, subPrice);
            parameters.Add(() => Firebase.Analytics.FirebaseAnalytics.ParameterCurrency, currency);
#endif
            AnalyticsManager.LogEvent(oldEventName, parameters);
            AnalyticsManager.LogEvent(eventName, parameters);
        }

        [Conditional("ACTIVATE_ANALYTICS")]
        private void SendPurchaseDoneEvent(string eventName, string oldEventName, string purchaseID, string activationOrigin, string subPrice, string currency, int subCount)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(3);
            parameters.Add("bid", activationOrigin);
            parameters.Add("iap_price", subPrice);
            parameters.Add("iap_price_currency", currency);
            parameters.Add("iap_id", purchaseID);
            // parameters.Add("iap_count", subCount);
#if ACTIVATE_ANALYTICS
            parameters.Add(() => Firebase.Analytics.FirebaseAnalytics.ParameterValue, subPrice);
            parameters.Add(() => Firebase.Analytics.FirebaseAnalytics.ParameterCurrency, currency);
#endif
            AnalyticsManager.LogEvent("iap_done", parameters);
            AnalyticsManager.LogEvent(eventName, parameters);
        }


        [Conditional("ACTIVATE_ANALYTICS")]
        private void SendSubscriptionFailedEvent(string eventName, string productId, string activationOrigin, string error)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(2);
            parameters.Add("bid", activationOrigin);
            parameters.Add("error", error);
            parameters.Add("product_id", productId);
            AnalyticsManager.LogEvent(eventName, parameters);
        }


    }
}
