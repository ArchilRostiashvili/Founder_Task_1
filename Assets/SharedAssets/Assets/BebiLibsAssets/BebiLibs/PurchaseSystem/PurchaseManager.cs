using System.Collections.Generic;
using UnityEngine;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using System.Linq;
using System;

namespace BebiLibs.PurchaseSystem
{
    [CreateAssetMenu(fileName = "SubscriptionLocker", menuName = "BebiLibs/PurchaseSystem/SubscriptionLocker", order = 0)]
    public class PurchaseManager : PurchaseManagerBase
    {
        [SerializeField] AbstractUserData _userData;
        [SerializeField] private PurchaseHistoryData _purchaseHistory;
        [SerializeField] PurchaseControllerBase _purchaseController;
        [SerializeField] ProductStore _storeType;
        [SerializeField] List<ProductIdentifier> _purchaseProductList;
        [SerializeField] private bool _forceLockValue = false;
        [NonSerialized] private bool _isStoreInitialized = false;


        public bool IsStoreInitialize => _isStoreInitialized;
        public override bool IsSubscribed => _purchaseHistory.HasAnyActiveSubscription || _forceLockValue;
        public override bool IsInAppUnlocked => _purchaseHistory.HasAnyPurchasedNonConsumable || _forceLockValue;
        public override bool IsInitialized => _purchaseHistory.IsPurchaseDataInitialized;

        public override bool TryGetProductIdentifier(string productID, out ProductIdentifier productIdentifier)
        {
            productIdentifier = _purchaseController.GetProductInfo(productID);
            return productIdentifier != null;
        }


        public override void InitializePurchaseController(ProductStore storeType, PurchaseControllerBase purchaseController)
        {
            ResetData();
            _purchaseController = purchaseController;
            _purchaseController.AddPurchaseListener(this);
            _purchaseController.Initialize(storeType, this, _purchaseProductList);
            _storeType = storeType;
            _isStoreInitialized = true;
        }

        public override void BuyProduct(ProductIdentifier productToBuy, string sender)
        {
            _purchaseController.BuyProduct(productToBuy, sender);
        }

        public override void ForceChangeLockState(string sender, bool newLockState)
        {
            _forceLockValue = newLockState;
            OnForceLockStateChange(sender, IsSubscribed);
            InvokeOnPurchaseStateUpdated(null, false);
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, sender + " " + newLockState);
        }

        public override void ResetData()
        {
            _forceLockValue = false;
        }

        public override void RestorePurchases()
        {
            _purchaseController.RestorePurchases();
        }

        public override void SetAccountID()
        {
            _purchaseController.SetAccountID();
        }

        public override List<ProductIdentifier> GetProductIdentifierWith(LocalPurchaseType purchaseType, bool purchaseState)
        {
            var purchaseMatchList = _purchaseHistory.GetPurchasedDataWith(purchaseType, purchaseState);
            return _purchaseProductList.Where(x => purchaseMatchList.Any(y => y.ProductID == x.LocalProductID || y.LocalProductID == x.LocalProductID)).ToList();
        }

        public void SetPurchaseProductList(List<ProductIdentifier> purchaseProductList)
        {
            _purchaseProductList = purchaseProductList.Select(x => x).ToList();
        }
    }
}
