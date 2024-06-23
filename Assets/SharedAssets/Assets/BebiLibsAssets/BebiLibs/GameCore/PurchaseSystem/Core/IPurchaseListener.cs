using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PurchaseSystem.Core
{
    public interface IPurchaseListener
    {
        void OnPurchaseProcessStart(ProductIdentifier productIdentifier, string sender);
        void OnPurchaseInitialize();
        void OnPurchaseInitializeFailed(PurchaseErrorData purchaseErrorData);
        void OnPurchaseStateUpdated(ProductIdentifier purchasedProduct, bool fromProcessPurchase);
        void OnPurchaseProcessFailed(PurchaseErrorData failureReason, ProductIdentifier failedProduct);
        void OnPurchaseRestore(bool isSuccessfull);
    }

}
