using System;
using System.Collections.Generic;
using UnityEngine;


namespace BebiLibs.PurchaseSystem.Core
{
    public abstract class PurchaseManagerBase : ScriptableObject, IPurchaseListener
    {
        public event Action<ProductIdentifier, string> PurchaseProcessStartEvent;
        public event Action PurchaseInitializeEvent;
        public event Action<PurchaseErrorData> PurchaseInitializationFailedEvent;
        public event Action<ProductIdentifier, bool> PurchaseStateUpdatedEvent;
        public event Action<PurchaseErrorData, ProductIdentifier> PurchaseProcessFailedEvent;
        public event Action<bool> PurchaseRestoredEvent;
        public event Action<string, bool> ForceChangeLockStateEvent;

        public abstract bool IsSubscribed { get; }
        public abstract bool IsInAppUnlocked { get; }
        public abstract bool IsInitialized { get; }
        public abstract void InitializePurchaseController(ProductStore storeType, PurchaseControllerBase purchaseController);
        public abstract void ForceChangeLockState(string sender, bool newLockState);
        public abstract void ResetData();
        public abstract bool TryGetProductIdentifier(string productID, out ProductIdentifier productIdentifier);
        public abstract void BuyProduct(ProductIdentifier productToBuy, string sender);
        public abstract void RestorePurchases();
        public abstract void SetAccountID();
        public abstract List<ProductIdentifier> GetProductIdentifierWith(LocalPurchaseType purchaseType, bool purchaseState);

        void IPurchaseListener.OnPurchaseProcessStart(ProductIdentifier productIdentifier, string sender) => PurchaseProcessStartEvent?.Invoke(productIdentifier, sender);
        void IPurchaseListener.OnPurchaseInitialize() => PurchaseInitializeEvent?.Invoke();
        void IPurchaseListener.OnPurchaseStateUpdated(ProductIdentifier purchasedProduct, bool fromProcessPurchase) => PurchaseStateUpdatedEvent?.Invoke(purchasedProduct, fromProcessPurchase);
        void IPurchaseListener.OnPurchaseProcessFailed(PurchaseErrorData failureReason, ProductIdentifier failedProduct) => PurchaseProcessFailedEvent?.Invoke(failureReason, failedProduct);
        void IPurchaseListener.OnPurchaseRestore(bool isSuccessfull) => PurchaseRestoredEvent?.Invoke(isSuccessfull);
        void IPurchaseListener.OnPurchaseInitializeFailed(PurchaseErrorData purchaseErrorData) => PurchaseInitializationFailedEvent?.Invoke(purchaseErrorData);

        protected void InvokeOnPurchaseStateUpdated(ProductIdentifier purchasedProduct, bool fromProcessPurchase)
        {
            PurchaseStateUpdatedEvent?.Invoke(purchasedProduct, fromProcessPurchase);
        }

        protected void OnForceLockStateChange(string sender, bool isUnlocked) => ForceChangeLockStateEvent?.Invoke(sender, isUnlocked);
    }
}
