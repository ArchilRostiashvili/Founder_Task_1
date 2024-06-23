using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if AMAZON_BUILD
using com.amazon.device.iap.cpt;
#endif
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.RegistrationSystem.Core;

namespace BebiLibs.PurchaseSystem
{
    [CreateAssetMenu(fileName = "AmazonPurchaseController", menuName = "BebiLibs/PurchaseSystem/AmazonPurchaseController", order = 0)]
    public class AmazonPurchaseController : PurchaseControllerBase
    {
        public const string SUCCESSFUL = "SUCCESSFUL";
        public const string FAILED = "FAILED";
        public const string NOT_SUPPORTED = "NOT_SUPPORTED";
        public const string ALREADY_PURCHASED = "ALREADY_PURCHASED";

        public const string CONSUMABLE = "CONSUMABLE";
        public const string SUBSCRIPTION = "SUBSCRIPTION";
        public const string ENTITLED = "ENTITLED";
        public const string FULFILLED = "FULFILLED";
        public const string UNAVAILABLE = "UNAVAILABLE";

#if AMAZON_BUILD
        static IAmazonIapV2 _IapService = AmazonIapV2Impl.Instance;
        public override bool IsInitialized => _IapService != null;
        private List<PurchaseReceipt> _paidReceiptsList = new List<PurchaseReceipt>();
#else
        public override bool IsInitialized => false;
#endif

        [SerializeField] private List<ProductIdentifier> _purchaseIdentifierList;
        [SerializeField] private AbstractUserData _userData;
        [SerializeField] private PurchaseHistoryData _purchaseHistoryData;
        [SerializeField] private ReceiptParserBase _amazonReceiptParser;
        [SerializeField] private List<PurchaseData> _purchaseDataList = new List<PurchaseData>();

        //Purchase Data that currently returned from OnProcessPurchaseResponse Method
        private ProductIdentifier _activePurchaseData;


        public override void Initialize(ProductStore storeType, PurchaseManagerBase purchaseManager, List<ProductIdentifier> purchaseProductList)
        {
#if AMAZON_BUILD
            _purchaseIdentifierList = purchaseProductList;
            _purchaseHistoryData.Initialize(_amazonReceiptParser);
            _storeType = storeType;

            InitializeProductData();

            _IapService.AddGetUserDataResponseListener(OnUserDataResponse);
            _IapService.AddPurchaseResponseListener(OnPurchaseResponse);
            _IapService.AddGetProductDataResponseListener(OnProductDataResponse);
            _IapService.AddGetPurchaseUpdatesResponseListener(OnPurchaseUpdatesResponse);

            GetUserData();
            GetPurchaseUpdates();
            GetProductData();
#endif
        }



        public void InitializeProductData()
        {
            foreach (var item in _purchaseIdentifierList)
            {
                if (_purchaseHistoryData.TryGetPurchaseData(item.LocalProductID, out PurchaseData purchaseData))
                {
                    _purchaseDataList.Add(purchaseData.Copy());
                }
                else
                {
                    _purchaseDataList.Add(new PurchaseData()
                    {
                        ProductID = item.AmazonChildSku,
                        LocalProductID = item.LocalProductID,
                        StoreID = ProductStore.Amazon,
                        PurchaseType = (LocalPurchaseType)item.ProductType,
                        PurchaseMetadata = PurchaseMetadata.Empty
                    });
                }
            }
        }

        public bool TryGetProductIdentifier(string skuID, out ProductIdentifier productIdentifier)
        {
            productIdentifier = null;
            if (string.IsNullOrEmpty(skuID)) return false;
            productIdentifier = _purchaseIdentifierList.Find(x => x.LocalProductID == skuID || x.AmazonChildSku == skuID || x.AmazonParentSku == skuID);
            return productIdentifier != null;
        }

        public bool TryGetPurchaseData(string skuID, out PurchaseData purchaseData)
        {
            purchaseData = null;
            if (string.IsNullOrEmpty(skuID)) return false;

            if (!TryGetProductIdentifier(skuID, out ProductIdentifier productIdentifier)) return false;

            purchaseData = _purchaseDataList.Find(x => x.ProductID == productIdentifier.LocalProductID || x.ProductID == productIdentifier.AmazonParentSku || x.ProductID == productIdentifier.AmazonChildSku || x.LocalProductID == productIdentifier.LocalProductID);
            return purchaseData != null;
        }


        public override void BuyProduct(ProductIdentifier productToBuy, string sender)
        {
#if AMAZON_BUILD
            IAmazonIapV2 iapService = AmazonIapV2Impl.Instance;
            BuyProduct(iapService, productToBuy);
            InvokeOnPurchaseProcessStartEvent(productToBuy, sender);
#endif
        }

        public override ProductIdentifier GetProductInfo(string productID)
        {
            return _purchaseIdentifierList.Find(x => x.LocalProductID == productID || x.AmazonChildSku == productID || x.AmazonParentSku == productID);
        }

#if AMAZON_BUILD
        private void GetPurchaseUpdates(bool reset = true)
        {
            if (reset)
                _paidReceiptsList.Clear();

            ResetInput request = new ResetInput();
            request.Reset = reset;
            RequestOutput response = _IapService.GetPurchaseUpdates(request);
            string requestIdString = response.RequestId;
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "Get Product Data Request ID " + requestIdString);
        }

        [System.Diagnostics.Conditional("AMAZON_BUILD")]
        private void GetProductData()
        {
            SkusInput request = new SkusInput();
            List<string> skus = _purchaseIdentifierList.Select(x => x.AmazonChildSku).ToList();
            request.Skus = skus;
            RequestOutput response = _IapService.GetProductData(request);
            string requestIdString = response.RequestId;
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "Initialization Request ID " + requestIdString);
        }

        private void BuyProduct(IAmazonIapV2 iapService, ProductIdentifier productIdentifier)
        {
            SkuInput request = new SkuInput();
            request.Sku = productIdentifier.AmazonChildSku;
            RequestOutput response = iapService.Purchase(request);
        }


        private void OnProductDataResponse(GetProductDataResponse args)
        {
            Dictionary<string, ProductData> productDataMap = args.ProductDataMap;
            List<string> unavailableSkus = args.UnavailableSkus;
            if (args.Status == SUCCESSFUL)
            {
                GatherProductInfo(productDataMap);
                InvokeOnPurchaseInitializeEvent();
            }
            else
            {
                Debug.LogError("#AmazonPurchaseManager:OnProductDataResponse: Unable To Initialize Product Data");
                InvokeOnPurchaseInitializeFailedEvent(new PurchaseError("Unable To Initialize Product Data"));
            }
        }


        [System.Diagnostics.Conditional("AMAZON_BUILD")]
        public void GatherProductInfo(Dictionary<string, ProductData> productDataMap)
        {
            foreach (var item in productDataMap)
            {
                ProductData productData = item.Value;

                if (TryGetPurchaseData(productData.Sku, out PurchaseData purchaseData))
                {
                    purchaseData.PurchaseMetadata.LocalizedPriceString = productData.Price;
                    purchaseData.PurchaseMetadata.LocalizedTitle = productData.Title;
                    purchaseData.PurchaseMetadata.LocalizedDescription = productData.Description;
                    purchaseData.PurchaseMetadata.IsoCurrencyCode = string.Empty;
                    purchaseData.PurchaseMetadata.LocalizedPrice = ToDecimal(productData.Price);
                }
                else
                {
                    Debug.LogError($"Unable To Find PurchaseData With Name {item.Key}");
                }
            }
        }

        public decimal ToDecimal(string price)
        {
            try
            {
                string value = Regex.Match(price, @"-?\d{1,3}(,\d{3})*(\.\d+)?").Value;
                return decimal.Parse(value, NumberStyles.Currency | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return 0m;
            }
        }


        public void OnPurchaseUpdatesResponse(GetPurchaseUpdatesResponse args)
        {
            if (args.Status == SUCCESSFUL)
            {
                _paidReceiptsList.AddRange(args.Receipts);
                Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "---------------Receipt History: {0}---------------", args.Receipts.Count);
                args.Receipts?.ForEach(x => LogPurchaseReceipt(x));
                if (args.HasMore)
                {
                    Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Has More {0}", args.HasMore);
                    GetPurchaseUpdates(false);
                }
                else
                {
                    List<PurchaseReceipt> paidReceipts = _paidReceiptsList.OrderBy(x => x.CancelDate).ToList();
                    if (paidReceipts != null && paidReceipts.Count > 0)
                    {
                        UpdateSortedReceiptData(paidReceipts);
                    }
                    _activePurchaseData = null;
                }
            }
            else
            {
                _activePurchaseData = null;
                Debug.LogError($"#AmazonPurchaseManager:OnPurchaseUpdatesResponse: Unable To Update Purchase Data, Status {args.Status}");
            }
        }


        private void UpdateSortedReceiptData(List<PurchaseReceipt> purchaseReceipts)
        {
            foreach (PurchaseData purchaseData in _purchaseDataList)
            {
                TryGetProductIdentifier(purchaseData.ProductID, out ProductIdentifier productID);

                if (CheckProductSubscription(purchaseReceipts, productID, purchaseData, out PurchaseReceipt purchaseReceipt))
                    UpdatePurchaseData(purchaseReceipt, purchaseData);
                else
                    UpdatePurchaseData(null, purchaseData);

                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "-----------------------------------------");
                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Updated Purchase data {0}", purchaseData);
            }

            _purchaseHistoryData.UpdateGamePurchaseData(_purchaseDataList);

            if (_activePurchaseData != null)
                InvokeOnPurchaseStateUpdatedEvent(_activePurchaseData, true);
        }

        private bool CheckProductSubscription(List<PurchaseReceipt> purchaseReceipts, ProductIdentifier productID, PurchaseData purchaseData, out PurchaseReceipt receipt)
        {
            receipt = purchaseReceipts.Find(x => (x.Sku == productID.AmazonChildSku || x.Sku == productID.AmazonParentSku) && x.CancelDate == 0);
            if (receipt != null) return true;

            receipt = purchaseReceipts.Find(x => x.ReceiptId == purchaseData.TransactionID);
            if (receipt != null && receipt.CancelDate > 0)
            {
                return false;
            }
            else if(!string.IsNullOrEmpty(purchaseData.TransactionID))
            {
                receipt = PseudoRecept(purchaseData);
                return true;
            }

            return false;
        }


        public void UpdatePurchaseData(PurchaseReceipt receipt, PurchaseData purchaseData)
        {
            bool hasSubscription = receipt != null;
            purchaseData.HasReceipt = hasSubscription;
            purchaseData.IsPurchaseValid = hasSubscription;
            long startDate = hasSubscription ? receipt.PurchaseDate : DateTimeOffset.MinValue.Millisecond;
            DateTime starDate = DateTimeOffset.FromUnixTimeMilliseconds(startDate).ToUniversalTime().DateTime;
            purchaseData.ExpirationDate = starDate.Add(TimeSpan.FromDays(30));
            purchaseData.StartData = new SDateTime(starDate);
            purchaseData.IsSubscribed = hasSubscription;
            purchaseData.IsCanceled = !hasSubscription;
            purchaseData.IsExpired = !hasSubscription;
            purchaseData.IsFreeTrial = !hasSubscription;
            purchaseData.TransactionID = hasSubscription ? receipt.ReceiptId : string.Empty;
        }


        public void AddPurchasedProductToPurchaseList(PurchaseReceipt purchaseReceipt)
        {
            if (purchaseReceipt != null)
            {
                if (TryGetProductIdentifier(purchaseReceipt.Sku, out ProductIdentifier productIdentifier))
                {
                    PurchaseData purchaseData = _purchaseDataList.Find(x => x.LocalProductID == productIdentifier.LocalProductID || x.ProductID == productIdentifier.AmazonChildSku || x.ProductID == productIdentifier.AmazonParentSku);
                    if (purchaseData != null)
                    {
                        UpdatePurchaseData(purchaseReceipt, purchaseData);
                    }
                }
            }
        }

        private void OnPurchaseResponse(PurchaseResponse args)
        {
            string status = args.Status;

            if (status == SUCCESSFUL && args.PurchaseReceipt != null)
            {
                LogPurchaseReceipt(args.PurchaseReceipt);

                if (args.PurchaseReceipt.CancelDate == 0)
                {
                    SetFulfilledState(args.PurchaseReceipt.ReceiptId, true, _IapService);
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Purchase Fulfilled");
                }
                else
                {
                    SetFulfilledState(args.PurchaseReceipt.ReceiptId, false, _IapService);
                    Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Purchase NOT Fulfilled");
                }

                AddPurchasedProductToPurchaseList(args.PurchaseReceipt);
                TryGetProductIdentifier(args.PurchaseReceipt.Sku, out _activePurchaseData);
                GetPurchaseUpdates();
            }
            else if (status == ALREADY_PURCHASED)
            {
                if (args.PurchaseReceipt != null)
                {
                    LogPurchaseReceipt(args.PurchaseReceipt);
                }
                else
                {
                    Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, $"#OnProductPurchase Failed Status {args.Status} and has no PurchaseReceipt");
                }
            }
            else
            {
                InvokeOnPurchaseProcessFailedEvent(new PurchaseError("Amazon Product Purchase Failed"), null);
                Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "OnProductPurchase: -> FAILED");
            }
        }

        private void SetFulfilledState(string receiptId, bool fulfilled, IAmazonIapV2 iapService)
        {
            NotifyFulfillmentInput request = new NotifyFulfillmentInput
            {
                ReceiptId = receiptId,
                FulfillmentResult = fulfilled ? FULFILLED : UNAVAILABLE
            };
            iapService.NotifyFulfillment(request);
        }


        private void GetUserData()
        {
            RequestOutput response = _IapService.GetUserData();
            string requestIdString = response.RequestId;
            Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "UserData Request ID " + requestIdString);
        }


        private void OnUserDataResponse(GetUserDataResponse args)
        {
            string requestId = args.RequestId;
            string userId = args.AmazonUserData.UserId;
            string marketplace = args.AmazonUserData.Marketplace;
            string status = args.Status;
        }


        private void LogPurchaseReceipt(PurchaseReceipt purchase)
        {
            string text = "///////////////////////////////////////////////////\n";
            text += "   SKU: " + purchase.Sku + "\n";
            text += "   ReceiptID: " + purchase.ReceiptId + "\n";
            text += "   PurchaseDate: " + purchase.PurchaseDate.ToString() + "\n";
            text += "   CancelDate: " + purchase.CancelDate.ToString() + "\n";
            Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, text);
        }

        private PurchaseReceipt PseudoRecept(PurchaseData purchaseData)
        {
            return new PurchaseReceipt()
            {
                ReceiptId = purchaseData.TransactionID,
                CancelDate = 0,
                PurchaseDate = purchaseData.StartData.EpochTime,
                Sku = purchaseData.ProductID,
                ProductType = SUBSCRIPTION,
            };
        }
#endif

        public override void RestorePurchases()
        {

        }

        public override void SetAccountID()
        {
            Debug.Log("SetAccountID");
        }
    }
}

//adb shell setprop debug.amazon.sandboxmode debug
//adb shell setprop debug.amazon.sandboxmode none
//adb push C:\Users\TEDCH\Desktop\amazon.sdktester.json /sdcard/amazon.sdktester.json