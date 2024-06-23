using BebiLibs.CrashAnalyticsSystem;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
#endif

namespace BebiLibs.PurchaseSystem
{

#if UNITY_PURCHASING
    [CreateAssetMenu(fileName = "DefaultPurchaseController", menuName = "BebiLibs/PurchaseSystem/DefaultPurchaseController", order = 0)]
    public class DefaultPurchaseController : PurchaseControllerBase, IStoreListener, IResetOnPreBuild
    {
        [SerializeField] private List<ProductIdentifier> _purchaseProductList;
        [SerializeField] private AbstractUserData _userData;
        [SerializeField] private PurchaseHistoryData _purchaseHistoryData;
        [SerializeField] private GoogleReceiptParser _googleReceiptParser;
        [SerializeField] private AppleReceiptParser _appleReceiptParser;
        [SerializeField] private PurchaseValidatorBase _purchaseValidator;

        [SerializeField] private List<PurchaseData> _localPurchaseDataList = new List<PurchaseData>();

        [Header("Editor Tests:")]
        [SerializeField] bool _useLocalTestPurchaseData;
        [SerializeField] bool _failTestPurchase;
        [SerializeField] bool _callTestProcessPurchase;
        [SerializeField] List<ProductIdentifier> _alwaysSuccessProductList = new List<ProductIdentifier>();


        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;
        private IAppleExtensions _appleExtensions;
        private IGooglePlayStoreExtensions _googlePlayStoreExtensions;
        private CrossPlatformValidator _crossPlatformValidator;
        private ConfigurationBuilder _purchaseConfigurationBuilder;
        private Dictionary<string, System.Action> _onReinitializePurchaseData = new Dictionary<string, Action>();

        [System.NonSerialized]
        private bool _isInitializeWaitPeriodStarted = false;

        public override bool IsInitialized => _storeController != null && _storeExtensionProvider != null;

        public override void Initialize(ProductStore storeType, PurchaseManagerBase purchaseManager, List<ProductIdentifier> purchaseProductList)
        {

            _purchaseProductList = purchaseProductList;
            _storeType = storeType;

            InitializePurchaseHistory();
            InitializeConfigurationBuilder();
            InitializeCrossPlatformValidator();
            SetAccountIDIntoGoogleConfiguration();
            InitializePurchaseProducts();

#if UNITY_EDITOR
            ProcessPurchaseTest();
#endif
        }

        private void ProcessPurchaseTest()
        {
            if (!(_callTestProcessPurchase && Application.isEditor)) return;

            foreach (var item in _alwaysSuccessProductList)
            {
                OnProcessPurchase(item.GoogleProductID);
            }
        }

        private ProductStore GetStoreType()
        {
#if UNITY_ANDROID
            return ProductStore.PlayStore;
#elif UNITY_IOS
            return ProductStore.AppStore;
#endif
        }

        private void InitializeConfigurationBuilder()
        {
            StandardPurchasingModule module = StandardPurchasingModule.Instance();
            _purchaseConfigurationBuilder = ConfigurationBuilder.Instance(module);
        }

        private void InitializeCrossPlatformValidator()
        {
            try
            {
                Debug.Log("Application Identifier: " + Application.identifier);
                _crossPlatformValidator = new CrossPlatformValidator(_purchaseValidator.GetGooglePublicKey(), _purchaseValidator.GetAppleRootCert(), Application.identifier);
            }
            catch
            {
                if (!Application.isEditor)
                {
                    Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "Unable to initialize cross platform validator");
                }
            }
        }

        public void SetAccountIDIntoGoogleConfiguration()
        {
            try
            {
#if UNITY_ANDROID
                IGooglePlayConfiguration configuration = _purchaseConfigurationBuilder.Configure<IGooglePlayConfiguration>();

                if (configuration == null)
                {
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "SetAccountID: Error - configuration is null");
                    return;
                }

                if (_userData == null)
                {
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "SetAccountID: Error - userData is null");
                    return;
                }

                if (!_userData.isUserSignedIn)
                {
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "SetAccountID: Error - user is not signed in");
                    return;
                }

                configuration.SetObfuscatedAccountId(_userData.userID);
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "SetAccountID: " + _userData.userID + "");
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void InitializePurchaseHistory()
        {
            ReceiptParserBase parser = _storeType == ProductStore.PlayStore ? _googleReceiptParser : _appleReceiptParser;
            _purchaseHistoryData.Initialize(parser);

            _localPurchaseDataList.Clear();
            foreach (var item in _purchaseProductList)
            {
                if (_purchaseHistoryData.TryGetPurchaseData(item.LocalProductID, out PurchaseData purchaseData))
                {
                    _localPurchaseDataList.Add(purchaseData.Copy());
                }
                else
                {
                    PurchaseData NewPurchaseData = new PurchaseData();
                    NewPurchaseData.SetProductData(item, _storeType);
                    NewPurchaseData.PurchaseMetadata = PurchaseMetadata.Empty;
                    _localPurchaseDataList.Add(NewPurchaseData);
                }
            }
        }

        private void InitializePurchaseProducts()
        {
            foreach (ProductIdentifier productInfo in _purchaseProductList)
            {
                IDs storeProductID = new IDs();
                storeProductID.Add(productInfo.AppleProductID, AppleAppStore.Name);
                storeProductID.Add(productInfo.GoogleProductID, GooglePlay.Name);
                _purchaseConfigurationBuilder.AddProduct(productInfo.LocalProductID, (ProductType)productInfo.ProductType, storeProductID);
            }
            UnityPurchasing.Initialize(this, _purchaseConfigurationBuilder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeExtensionProvider = extensions;
            _appleExtensions = extensions.GetExtension<IAppleExtensions>();
            _googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
            //Debug.LogError("OnInitialized: PASS " + (_storeController != null) + " " + (_storeExtensionProvider != null) + ", Hook " + IsInitialized);
            ReinitializePurchaseData(() =>
            {
                InvokeOnPurchaseInitializeEvent();
            });
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, $"{nameof(DefaultPurchaseController)}: Initialization Failed, Error {error}");
            InvokeOnPurchaseInitializeFailedEvent(new InitializationFailureData(error));
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            ProductIdentifier productInfo = GetProductInfo(product.definition.storeSpecificId);
            if (failureReason == PurchaseFailureReason.DuplicateTransaction)
            {
                ReinitializePurchaseData(null);
                return;
            }

            Debug.LogError($"{nameof(DefaultPurchaseController)}: Purchase Of Product \"{productInfo.LocalProductID}\" Failed. Error: {failureReason}");
            InvokeOnPurchaseProcessFailedEvent(new PurchaseFailureData(failureReason), productInfo);
        }

        public override void RestorePurchases()
        {
            if (IsInitialized)
            {
                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    if (_appleExtensions == null)
                    {
                        Debug.LogError($"{nameof(DefaultPurchaseController)}: RestorePurchases FAIL.  Apple Extensions is null.  Current = {Application.platform}");
                        return;
                    }
                    _appleExtensions.RestoreTransactions((result) =>
                    {
                        InvokeOnPurchaseRestoreEvent(result);
                        Debug.LogError($"{nameof(DefaultPurchaseController)}: RestorePurchases continuing: {result}. If no further messages, no purchases available to restore.");
                    });
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    if (_googlePlayStoreExtensions == null)
                    {
                        Debug.LogError($"{nameof(DefaultPurchaseController)}: RestorePurchases FAIL. Google Extension is null. Current = {Application.platform}");
                        return;
                    }

                    _googlePlayStoreExtensions.RestoreTransactions((result) =>
                    {
                        InvokeOnPurchaseRestoreEvent(result);
                        Debug.LogError($"{nameof(DefaultPurchaseController)}: RestorePurchases continuing: {result}. If no further messages, no purchases available to restore.");
                    });
                }
                else
                {
                    Debug.LogError($"{nameof(DefaultPurchaseController)}: RestorePurchases FAIL. Not supported on this platform. Current = {Application.platform}");
                }
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            Product product = purchaseEvent.purchasedProduct;
            OnProcessPurchase(product.definition.storeSpecificId);
            return PurchaseProcessingResult.Complete;
        }

        private void OnProcessPurchase(string productStoreSpecificID)
        {
            SafeLogNoStack.Log("OnProcessPurchase: " + productStoreSpecificID, "Purchase Process");

            ProductIdentifier productInfo = GetProductInfo(productStoreSpecificID);
            UpdateLastPurchaseState(productInfo);
            ReinitializePurchaseData(() =>
            {
                InvokeOnPurchaseStateUpdatedEvent(productInfo, true);
            });
        }

        private void UpdateLastPurchaseState(ProductIdentifier product)
        {
            _localPurchaseDataList.ForEach(x => x.IsLastToPurchase = x.ProductID == product.LocalProductID || x.LocalProductID == product.LocalProductID);
        }

        public override ProductIdentifier GetProductInfo(string productID)
        {
            return _purchaseProductList.Find(x => x.LocalProductID == productID || x.AppleProductID == productID || x.GoogleProductID == productID);
        }


        public override void BuyProduct(ProductIdentifier productInfo, string sender)
        {
#if UNITY_EDITOR
            InitiateFakePurchase(productInfo, sender);
#else

        Debug.LogWarning("IsInitialized " + IsInitialized);
        if (IsInitialized)
        {   
            BuyProductID(productInfo);
            InvokeOnPurchaseProcessStartEvent(productInfo, sender);
        }
        else
        {
            Debug.LogError($"{nameof(DefaultPurchaseController)}: FAILED Buying product \"{productInfo.LocalProductID}\", Purchase System is Not Initialized");
            InvokeOnPurchaseProcessFailedEvent(new PurchaseError($"Unable to buy product: {productInfo.LocalProductID}, Reason: Purchase System Is Not Initialize"), productInfo);
        }
#endif
        }


        private void InitiateFakePurchase(ProductIdentifier productInfo, string sender)
        {
            Debug.Log("InitiateFakePurchase: " + productInfo.LocalProductID);
            if (_failTestPurchase && Application.isEditor)
            {
                Debug.Log("InitiateFakePurchase: " + "Always Fail " + productInfo.LocalProductID);
                InvokeOnPurchaseProcessStartEvent(productInfo, sender);
                ManagerTime.Delay(1f, () =>
                   {
                       foreach (var item in _purchaseHistoryData.PurchasesDataList)
                       {
                           item.IsSubscribed = false;
                           item.HasReceipt = false;
                           item.IsPurchaseValid = false;
                       }
                       _purchaseHistoryData.UpdateGamePurchaseData(_purchaseHistoryData.PurchasesDataList);
                       InvokeOnPurchaseProcessFailedEvent(new PurchaseError($"Unable to buy fake product: {productInfo.LocalProductID}, Reason: Fail TestPurchase Flag is Enabled"), productInfo);
                   });
            }
            else
            {
                InvokeOnPurchaseProcessStartEvent(productInfo, sender);
                ManagerTime.Delay(1f, () =>
                {
                    foreach (var item in _purchaseHistoryData.PurchasesDataList)
                    {
                        item.IsSubscribed = true;
                        item.HasReceipt = true;
                        item.IsPurchaseValid = true;
                        item.PurchaseDate = DateTime.Now;
                        item.ExpirationDate = DateTime.Now.AddDays(30);
                        item.StartData = DateTime.Now;
                    }
                    _purchaseHistoryData.UpdateGamePurchaseData(_purchaseHistoryData.PurchasesDataList);
                    InvokeOnPurchaseStateUpdatedEvent(productInfo, true);
                    Debug.Log("Fake Purchase Success");
                });
            }
        }

        protected void BuyProductID(ProductIdentifier productInfo)
        {
            if (IsInitialized)
            {
                Product product = _storeController.products.WithID(productInfo.LocalProductID);
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format($"{nameof(DefaultPurchaseController)}: Purchasing product asynchronously: '{product.definition.id}'"));
                    try
                    {
                        _storeController.InitiatePurchase(product);
                    }
                    catch
                    {
                        Debug.LogError($"{nameof(DefaultPurchaseController)}: Unable To Initiate Purchase Of {productInfo.LocalProductID}");
                        InvokeOnPurchaseProcessFailedEvent(new PurchaseError($"Unable to initiate purchase of {productInfo.LocalProductID}"), productInfo);
                    }
                }
                else
                {
                    Debug.LogError($"{nameof(DefaultPurchaseController)}: FAIL. Not purchasing product \"{productInfo.LocalProductID}\", either is not found or is not available for purchase");
                    InvokeOnPurchaseProcessFailedEvent(new PurchaseError($"Product {productInfo.LocalProductID} is not available for purchase "), productInfo);
                }
            }
        }

        private void ReinitializePurchaseData(Action onReinitializeFinished)
        {
            if (!_onReinitializePurchaseData.ContainsKey(onReinitializeFinished.Method.Name))
            {
                _onReinitializePurchaseData.Add(onReinitializeFinished.Method.Name, onReinitializeFinished);
            }

            if (!_isInitializeWaitPeriodStarted)
            {
                _isInitializeWaitPeriodStarted = true;
                ManagerTime.DelayFrame(5, () =>
                {
                    //Debug.LogWarning($"Update Data At Frame: {Time.frameCount}");
                    _isInitializeWaitPeriodStarted = false;
                    UpdatePurchaseDataFromStoreController();
                    _purchaseHistoryData.UpdateGamePurchaseData(_localPurchaseDataList);

                    foreach (var purchaseAction in _onReinitializePurchaseData)
                    {
                        purchaseAction.Value?.Invoke();
                    }

                    _onReinitializePurchaseData.Clear();
                });
            }
        }

        private void UpdatePurchaseDataFromStoreController()
        {
            foreach (ProductIdentifier productInfo in _purchaseProductList)
            {
                Product product = _storeController.products.WithID(productInfo.LocalProductID);

                if (!product.availableToPurchase) continue;
                PurchaseData purchaseData = _localPurchaseDataList.Find(x => x.ProductID == productInfo.LocalProductID || x.LocalProductID == productInfo.LocalProductID);

                if ((ProductType)productInfo.ProductType == ProductType.Subscription)
                {
                    SubscriptionUtility.TryGetSubscriptionInfo(product, _appleExtensions, out SubscriptionInfo info);
                    UpdatePurchaseData(purchaseData, productInfo, product, info);
                }
                else
                {
                    UpdatePurchaseData(purchaseData, productInfo, product, null);
                }

                //Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, $"{purchaseData} \n At Frame: {Time.frameCount} \n ---------------------");
            }
        }

        private void UpdatePurchaseData(PurchaseData purchase, ProductIdentifier productInfo, Product product, SubscriptionInfo subscriptionInfo = null)
        {
            purchase.SetProductData(productInfo, _storeType);

            if (product == null)
            {
                purchase.ClearPurchaseInfo();
                return;
            }

            ProductDefinition definition = product.definition;
            purchase.HasReceipt = product.hasReceipt;
            purchase.Receipt = product.hasReceipt ? product.receipt : string.Empty;
            purchase.IsPurchaseValid = product.hasReceipt && ValidateReceipt(product.receipt, purchase, productInfo);
#if UNITY_EDITOR
            purchase.ProductID = purchase.GetStoreSpecificID(productInfo);
#else
             purchase.ProductID = definition.storeSpecificId;
#endif
            purchase.PurchaseType = (LocalPurchaseType)definition.type;

            UpdateTestData(purchase, productInfo);

            PopulateSubscriptionData(purchase, productInfo, subscriptionInfo);
            UpdatePurchaseMetadata(purchase.PurchaseMetadata, product);
        }

        private void UpdateTestData(PurchaseData purchase, ProductIdentifier productInfo)
        {
            if (!(_useLocalTestPurchaseData && Application.isEditor) || !_alwaysSuccessProductList.Contains(productInfo)) return;

            Debug.Log("Setting Test Purchase Data");
            purchase.HasReceipt = true;
            purchase.Receipt = "{\"Payload\":\"TEST\",\"Store\":\"AppleAppStore\",\"TransactionID\":\"1000000895256550\"}";
            ValidateReceipt(purchase.Receipt, purchase, productInfo);
            purchase.IsPurchaseValid = true;
            purchase.ProductID = purchase.GetStoreSpecificID(productInfo);
            purchase.PurchaseType = productInfo.ProductType;
        }

        private void PopulateSubscriptionData(PurchaseData purchase, ProductIdentifier identifier, SubscriptionInfo subscriptionInfo)
        {
            if (subscriptionInfo == null)
            {
                purchase.ClearSubscriptionState();
                UpdateSubscriptionTest(purchase, identifier);
                return;
            }
            purchase.ExpirationDate = subscriptionInfo.getExpireDate();
            purchase.StartData = subscriptionInfo.getPurchaseDate();
            purchase.IsSubscribed = subscriptionInfo.isSubscribed() == Result.True;
            purchase.IsExpired = subscriptionInfo.isExpired() == Result.True;
            purchase.IsCanceled = subscriptionInfo.isCancelled() == Result.True;
            purchase.IsFreeTrial = subscriptionInfo.isFreeTrial() == Result.True;

            UpdateSubscriptionTest(purchase, identifier);
        }

        private void UpdateSubscriptionTest(PurchaseData purchase, ProductIdentifier productInfo)
        {
            if (!(_useLocalTestPurchaseData && Application.isEditor) || !_alwaysSuccessProductList.Contains(productInfo)) return;

            Debug.Log("Setting Test Subscription Data");
            purchase.ExpirationDate = System.DateTime.UtcNow + System.TimeSpan.FromSeconds(5);
            purchase.StartData = System.DateTime.UtcNow;
            purchase.IsSubscribed = true;
            purchase.IsExpired = false;
            purchase.IsCanceled = false;
            purchase.IsFreeTrial = false;
        }

        private void UpdatePurchaseMetadata(PurchaseMetadata metadata, Product product)
        {
            ProductMetadata productMetadata = product.metadata;
            metadata.LocalizedPriceString = productMetadata.localizedPriceString;
            metadata.LocalizedTitle = productMetadata.localizedTitle;
            metadata.LocalizedDescription = productMetadata.localizedDescription;
            metadata.IsoCurrencyCode = productMetadata.isoCurrencyCode;
            metadata.LocalizedPrice = productMetadata.localizedPrice;
        }

        private bool ValidateReceipt(string receipt, PurchaseData purchaseData, ProductIdentifier productIdentifier)
        {
            LogReceiptPreview(receipt, purchaseData);
            try
            {
                List<IPurchaseReceipt> parsedReceiptData = _crossPlatformValidator.Validate(receipt).OrderByDescending(x => x.purchaseDate).ToList();
                IPurchaseReceipt purchaseReceipt = null;

                if (parsedReceiptData.Count > 1)
                {
                    purchaseReceipt = parsedReceiptData.FirstOrDefault(x => x.productID == purchaseData.ProductID || x.productID == purchaseData.LocalProductID);
                }
                else if (parsedReceiptData.Count == 1)
                {
                    purchaseReceipt = parsedReceiptData[0];
                }
                else if (parsedReceiptData.Count == 0)
                {
                    if (!Application.isEditor)
                    {
                        Debug.LogWarning("There is no receipt to process for " + purchaseData.ProductID);
                    }

                    purchaseReceipt = null;
                }

                if (purchaseReceipt == null)
                {
                    return false;
                }

                return ValidateStorePurchaseReceipt(purchaseReceipt, purchaseData); ;
            }
            catch (Exception e)
            {
                UpdateTestValidationData(purchaseData, productIdentifier);
                Debug.LogError($"Receipt Validation Failed For {productIdentifier.AnalyticsID}, Error {e}");
                return false;
            }
        }

        private void UpdateTestValidationData(PurchaseData purchaseData, ProductIdentifier identifier)
        {
            if (!(_useLocalTestPurchaseData && Application.isEditor) || !_alwaysSuccessProductList.Contains(identifier)) return;

            purchaseData.TransactionID = "Test Transaction ID " + identifier.GoogleProductID;
            purchaseData.PurchaseDate = System.DateTime.UtcNow;
            purchaseData.PurchaseToken = "Test Purchase Token " + identifier.GoogleProductID;
            purchaseData.PackageName = "Test Package Name " + Application.identifier;
        }

        private bool ValidateStorePurchaseReceipt(IPurchaseReceipt purchaseReceipt, PurchaseData purchaseData)
        {
#if UNITY_ANDROID
            return ValidateGooglePurchase(purchaseReceipt, purchaseData);
#elif UNITY_IOS
            return ValidateApplePurchase(purchaseReceipt, purchaseData);
#endif
        }

        private bool ValidateApplePurchase(IPurchaseReceipt purchaseReceipt, PurchaseData purchaseData)
        {
            try
            {
                AppleInAppPurchaseReceipt receipt = (AppleInAppPurchaseReceipt)purchaseReceipt;
                if (receipt != null)
                {
                    purchaseData.TransactionID = receipt.transactionID;
                    purchaseData.PurchaseDate = receipt.purchaseDate;
                    LogApplePurchaseData(receipt);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateGooglePurchase(IPurchaseReceipt purchaseReceipt, PurchaseData purchaseData)
        {
            try
            {
                GooglePlayReceipt receipt = (GooglePlayReceipt)purchaseReceipt;
                if (receipt != null)
                {
                    purchaseData.TransactionID = receipt.transactionID;
                    purchaseData.PurchaseDate = receipt.purchaseDate;
                    purchaseData.PurchaseToken = receipt.purchaseToken;
                    purchaseData.PackageName = receipt.packageName;
                    LogGooglePurchaseData(receipt);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void LogGooglePurchaseData(GooglePlayReceipt receipt)
        {
            try
            {
                string logText = "";
                logText += $"GPR.{nameof(receipt.productID)}: {receipt.productID} \n";
                logText += $"GPR.{nameof(receipt.packageName)}: {receipt.packageName} \n";
                logText += $"GPR.{nameof(receipt.transactionID)}: {receipt.transactionID} \n";
                logText += $"GPR.{nameof(receipt.purchaseDate)}: {receipt.purchaseDate} \n";
                //logText += $"GPR.{nameof(receipt.purchaseToken)}: {receipt.purchaseToken} \n";
                // logText += $"GPR.{nameof(Time.frameCount)}: {Time.frameCount}---------------------------- \n";
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "GooglePlayReceipt: \n" + logText);
            }
            catch (Exception e)
            {
                Debug.Log("Unable To Log Google Receipt Data, Error: " + e);
            }
        }

        private void LogApplePurchaseData(AppleInAppPurchaseReceipt receipt)
        {
            try
            {
                string logText = "";
                logText += $"AIAPR.{nameof(receipt.productID)}: {receipt.productID} \n";
                logText += $"AIAPR.{nameof(receipt.transactionID)}: {receipt.transactionID} \n";
                logText += $"AIAPR.{nameof(receipt.originalTransactionIdentifier)}: {receipt.originalTransactionIdentifier} \n";
                logText += $"AIAPR.{nameof(receipt.purchaseDate)}: {receipt.purchaseDate} \n";
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "AppleInAppPurchaseReceipt: \n" + logText);
            }
            catch (Exception e)
            {
                Debug.Log("Unable To Log Apple Receipt Data, Error: " + e);
            }
        }

        private void LogReceiptPreview(string receipt, PurchaseData purchaseData)
        {
            string receiptData = receipt.Length > 40 ? receipt.Substring(0, 40) : " Receipt: " + receipt;
            string logText = "Validating Product " + purchaseData.LocalProductID + " " + purchaseData.ProductID + " " + receiptData;
            SafeLogNoStack.Log(logText, "Receipt Preview");
        }

        public override void SetAccountID()
        {
            SetAccountIDIntoGoogleConfiguration();
        }

        public void ResetOnPreBuild()
        {
            _localPurchaseDataList.Clear();
            _useLocalTestPurchaseData = false;
            _failTestPurchase = false;
            _callTestProcessPurchase = false;
        }
    }

#else
    [CreateAssetMenu(fileName = "DefaultPurchaseController", menuName = "BebiLibs/PurchaseSystem/DefaultPurchaseController", order = 0)]
    public class DefaultPurchaseController : PurchaseControllerBase
    {
        public override bool IsInitialized => true;

        public override void BuyProduct(ProductIdentifier productInfo, string sender)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "You Probably Forgot To Enable \"UNITY_PURCHASING\" preprocessor directive Or Amazon Version Is Enabled From AppVersionSwitch");
        }

        public override ProductIdentifier GetProductInfo(string productID)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "You Probably Forgot To Enable \"UNITY_PURCHASING\" preprocessor directive Or Amazon Version Is Enabled From AppVersionSwitch");
            return null;
        }

        public override void Initialize(ProductStore storeType, PurchaseManagerBase purchaseManager, List<ProductIdentifier> purchaseProductList)
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "You Probably Forgot To Enable \"UNITY_PURCHASING\" preprocessor directive Or Amazon Version Is Enabled From AppVersionSwitch");
        }

        public override void RestorePurchases()
        {
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "You Probably Forgot To Enable \"UNITY_PURCHASING\" preprocessor directive Or Amazon Version Is Enabled From AppVersionSwitch");
        }

        public override void SetAccountID()
        {
            
        }
    }
#endif
}