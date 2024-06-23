using BebiLibs.PurchaseSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PurchaseSystem
{
    [CreateAssetMenu(fileName = "PurchaseManagerConfig", menuName = "BebiLibs/PurchaseSystem/PurchaseManagerConfig", order = 0)]
    public class PurchaseManagerConfig : ScriptableObject
    {
        public List<ProductIdentifier> PurchaseProductList;
        public AbstractUserData UserDataSO;
        public PurchaseHistoryData PurchaseHistoryData;
        public PurchaseManagerBase GameLocker;
        public GoogleReceiptParser GoogleReceiptParser;
        public AppleReceiptParser AppleReceiptParser;
        public AmazonReceiptParser AmazonReceiptParser;

        public static bool TryLoadPurchaseManagerConfig(string configPath, out PurchaseManagerConfig purchaseManagerConfig)
        {
            purchaseManagerConfig = Resources.Load<PurchaseManagerConfig>(configPath);
            return purchaseManagerConfig != null;
        }

    }
}
