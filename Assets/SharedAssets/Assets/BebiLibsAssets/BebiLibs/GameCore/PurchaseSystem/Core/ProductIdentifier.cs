using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PurchaseSystem.Core
{
    [CreateAssetMenu(fileName = "PurchaseProductInfo", menuName = "BebiLibs/PurchaseSystem/PurchaseProductInfo", order = 0)]
    public class ProductIdentifier : ScriptableObject
    {
        [SerializeField] private string _analyticsID;
        [SerializeField] private string _localProductID;
        [SerializeField] private LocalPurchaseType _productType;

        [Header("Store Specific ID's")]
        [SerializeField] private string _appleProductID;
        [SerializeField] private string _googleProductID;

        [SerializeField] private string _amazonParentSku;
        [SerializeField] private string _amazonChildSku;

        public string AnalyticsID => _analyticsID;
        public string LocalProductID => _localProductID;
        public string AppleProductID => _appleProductID;
        public string GoogleProductID => _googleProductID;
        public string AmazonParentSku => _amazonParentSku;
        public string AmazonChildSku => _amazonChildSku;

        public LocalPurchaseType ProductType => _productType;

        public static ProductIdentifier Create(string analyticsID, string productID, LocalPurchaseType productType, string appleProductID, string googleProductID, string amazonParentSku, string amazonChildSku)
        {
            ProductIdentifier identifier = ScriptableObject.CreateInstance<ProductIdentifier>();
            identifier._analyticsID = analyticsID;
            identifier._localProductID = productID;
            identifier._productType = productType;
            identifier._appleProductID = appleProductID;
            identifier._googleProductID = googleProductID;
            identifier._amazonParentSku = amazonParentSku;
            identifier._amazonChildSku = amazonChildSku;
            return identifier;
        }
    }
}
