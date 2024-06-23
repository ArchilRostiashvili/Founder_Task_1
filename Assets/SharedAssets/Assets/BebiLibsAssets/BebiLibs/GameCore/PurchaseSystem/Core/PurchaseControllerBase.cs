using System;
using System.Collections.Generic;

namespace BebiLibs.PurchaseSystem
{
    using BebiLibs.PurchaseSystem.Core;
    using UnityEngine;

    public abstract class PurchaseControllerBase : ScriptableObject
    {
        public abstract bool IsInitialized { get; }
        [SerializeField] protected ProductStore _storeType;

        public abstract void BuyProduct(ProductIdentifier productInfo, string sender);
        public abstract ProductIdentifier GetProductInfo(string productID);
        public abstract void Initialize(ProductStore storeType, PurchaseManagerBase purchaseManager, List<ProductIdentifier> purchaseProductList);
        public abstract void RestorePurchases();
        public abstract void SetAccountID();

        protected List<IPurchaseListener> _purchaseListenersList = new List<IPurchaseListener>();

        public virtual void AddPurchaseListener(IPurchaseListener purchaseListener)
        {
            if (!_purchaseListenersList.Contains(purchaseListener))
                _purchaseListenersList.Add(purchaseListener);
        }

        public virtual void RemovePurchaseListener(IPurchaseListener purchaseListener)
        {
            int listenerIndex = _purchaseListenersList.IndexOf(purchaseListener);
            if (listenerIndex != -1)
                _purchaseListenersList.RemoveAt(listenerIndex);
        }



        protected void InvokeOnPurchaseProcessStartEvent(ProductIdentifier productToBuy, string sender) => _purchaseListenersList.ForEach(x => x.OnPurchaseProcessStart(productToBuy, sender));
        protected void InvokeOnPurchaseInitializeEvent() => _purchaseListenersList.ForEach(x => x.OnPurchaseInitialize());
        protected void InvokeOnPurchaseInitializeFailedEvent(PurchaseErrorData purchaseErrorData) => _purchaseListenersList.ForEach(x => x.OnPurchaseInitializeFailed(purchaseErrorData));
        protected void InvokeOnPurchaseStateUpdatedEvent(ProductIdentifier purchasedProduct, bool fromProcessPurchase) => _purchaseListenersList.ForEach(x => x.OnPurchaseStateUpdated(purchasedProduct, fromProcessPurchase));
        protected void InvokeOnPurchaseProcessFailedEvent(PurchaseErrorData failureReason, ProductIdentifier failedProduct) => _purchaseListenersList.ForEach(x => x.OnPurchaseProcessFailed(failureReason, failedProduct));
        protected void InvokeOnPurchaseRestoreEvent(bool isSuccessfull) => _purchaseListenersList.ForEach(x => x.OnPurchaseRestore(isSuccessfull));
    }

}
