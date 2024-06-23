using BebiLibs.PurchaseSystem;
using BebiLibs.PurchaseSystem.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class FakePurchaseController : PurchaseControllerBase
    {
        public IPurchaseListener PurchaseListener;

        public override bool IsInitialized => true;

        private ProductIdentifier _identifier;

        public override void BuyProduct(ProductIdentifier productInfo, string sender)
        {
            InvokeOnPurchaseProcessStartEvent(productInfo, sender);
            InvokeOnPurchaseStateUpdatedEvent(productInfo, true);
        }

        public override ProductIdentifier GetProductInfo(string productID)
        {
            return _identifier;
        }

        public override void Initialize(ProductStore storeType, PurchaseManagerBase purchaseManager, List<ProductIdentifier> purchaseProductList)
        {
            _storeType = storeType;
            _identifier = ProductIdentifier.Create("analytic_test", "test", LocalPurchaseType.Subscription, "test.apple", "test.google", "test.amazon.parent", "test.amazon.child");
            InvokeOnPurchaseInitializeEvent();
        }

        public override void RestorePurchases()
        {
            InvokeOnPurchaseRestoreEvent(true);
        }

        public override void SetAccountID()
        {
            Debug.Log("SetAccountID");
        }
    }
}
