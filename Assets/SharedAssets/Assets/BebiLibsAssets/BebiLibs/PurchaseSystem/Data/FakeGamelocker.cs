using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PurchaseSystem
{
    using BebiLibs.PurchaseSystem.Core;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FakeGamelocker", menuName = "BebiLibs/PurchaseSystem/FakeGamelocker", order = 0)]
    public class FakeGamelocker : PurchaseManagerBase
    {
        [SerializeField] private bool _isInitialized = true;
        [SerializeField] private bool _isUnlocked = true;
        [SerializeField] private bool _isInAppUnlocked = true;

        public override bool IsSubscribed => _isUnlocked;
        public override bool IsInitialized => _isInitialized;
        public override bool IsInAppUnlocked => _isInAppUnlocked;

        public override void InitializePurchaseController(ProductStore storeType, PurchaseControllerBase purchaseController)
        {
            Debug.Log("Initialize Fake Game Locker");
        }


        public override void BuyProduct(ProductIdentifier productToBuy, string sender)
        {
            Debug.Log($"Buying Fake Product From {sender}");
        }

        public override void ForceChangeLockState(string sender, bool newLockState)
        {
            _isUnlocked = newLockState;
        }

        public override void ResetData()
        {
            _isInitialized = true;
            _isUnlocked = true;
        }

        public override void RestorePurchases()
        {
            Debug.Log($"Restoring Fake Purchase");
        }

        public override bool TryGetProductIdentifier(string productID, out ProductIdentifier productIdentifier)
        {
            productIdentifier = ProductIdentifier.Create("test", "testID", LocalPurchaseType.Subscription, "appleID", "googleID", "amazonID", "amazonChild");
            return true;
        }

        public override List<ProductIdentifier> GetProductIdentifierWith(LocalPurchaseType purchaseType, bool purchaseState)
        {
            return new List<ProductIdentifier>();
        }

        public override void SetAccountID()
        {
            Debug.Log("Set Account ID");
        }
    }
}
