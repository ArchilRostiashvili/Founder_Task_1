using BebiLibs.PurchaseSystem.Core;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace BebiLibs.PurchaseSystem
{
    [System.Serializable]
    public class PurchaseData : IEquatable<PurchaseData>, ICloneable
    {
        public ProductStore StoreID = ProductStore.Unknown;

        [Header("Product Data:")]
        public string ProductID = string.Empty; //Warning ProductID is ProductIdentifier.storeSpecificID
        public string LocalProductID = string.Empty; //Warning Local Product ID is ProductIdentifier.ProductID
        public LocalPurchaseType PurchaseType = LocalPurchaseType.Consumable;
        public PurchaseMetadata PurchaseMetadata;


        [Header("Receipt Data:")]
        public bool HasReceipt = false;
        public bool IsPurchaseValid = false;
        public string TransactionID = string.Empty;

        [Header("Subscription Data:")]
        public SDateTime ExpirationDate = SDateTime.UtcMin;
        public SDateTime StartData = SDateTime.UtcMin;
        public bool IsSubscribed = false;
        public bool IsExpired = false;
        public bool IsCanceled = false;
        public bool IsFreeTrial = false;

        [Header("Data For Analytics:")]
        public string AnalyticsID = string.Empty;
        public int SubscriptionCount;
        public bool IsLastToPurchase;

        public SDateTime PurchaseDate = SDateTime.UtcMin;
        public string PurchaseToken;
        public string PackageName;

        [JsonIgnore()]
        public string Receipt = string.Empty;
        [JsonIgnore()]
        public string Payload = string.Empty;

        public bool isPurchaseExpired => ExpirationDate.DateTime < DateTime.UtcNow;

        public object Clone()
        {
            return Copy();
        }

        public void ResetData()
        {
            HasReceipt = false;
            IsPurchaseValid = false;
            ProductID = string.Empty;
            LocalProductID = string.Empty;
            ExpirationDate = SDateTime.UtcMin;
            StartData = SDateTime.UtcMin;
            IsSubscribed = false;
            IsExpired = false;
            IsCanceled = false;
            IsFreeTrial = false;
            StoreID = ProductStore.Unknown;
            PurchaseType = LocalPurchaseType.Consumable;
            TransactionID = string.Empty;
            PurchaseMetadata.ResetData();
            IsLastToPurchase = false;
            AnalyticsID = string.Empty;
            PurchaseDate = SDateTime.UtcMin;
            PurchaseToken = string.Empty;
            PackageName = string.Empty;
            Receipt = string.Empty;
            Payload = string.Empty;
        }


        public PurchaseData Copy()
        {
            return new PurchaseData()
            {
                HasReceipt = HasReceipt,
                IsPurchaseValid = IsPurchaseValid,

                ProductID = ProductID,
                LocalProductID = LocalProductID,
                ExpirationDate = ExpirationDate.Clone(),
                StartData = StartData.Clone(),
                IsSubscribed = IsSubscribed,
                IsExpired = IsExpired,
                IsCanceled = IsCanceled,
                IsFreeTrial = IsFreeTrial,
                StoreID = StoreID,
                PurchaseType = PurchaseType,
                TransactionID = TransactionID,
                PurchaseMetadata = PurchaseMetadata.Copy(),

                AnalyticsID = AnalyticsID,
                SubscriptionCount = SubscriptionCount,
                IsLastToPurchase = IsLastToPurchase,
                PurchaseDate = PurchaseDate.Clone(),
                PurchaseToken = PurchaseToken,
                PackageName = PackageName,
                Receipt = Receipt,
                Payload = Payload,
            };
        }


        public override string ToString()
        {
            string compText = "";
            compText += nameof(ProductID) + " - " + ProductID + "\n";
            compText += nameof(LocalProductID) + " - " + LocalProductID + "\n";
            compText += nameof(PurchaseType) + " - " + PurchaseType + "\n";
            compText += nameof(TransactionID) + " - " + TransactionID + "\n";
            compText += nameof(HasReceipt) + " - " + HasReceipt + "\n";
            compText += nameof(IsPurchaseValid) + " - " + IsPurchaseValid + "\n";
            compText += nameof(ExpirationDate) + " - " + ExpirationDate + "\n";
            compText += nameof(StartData) + " - " + StartData + "\n";
            compText += nameof(IsSubscribed) + " - " + IsSubscribed + "\n";
            compText += nameof(IsExpired) + " - " + IsExpired + "\n";
            compText += nameof(IsCanceled) + " - " + IsCanceled + "\n";
            compText += nameof(IsFreeTrial) + " - " + IsFreeTrial + "\n";
            compText += nameof(StoreID) + " - " + StoreID + "\n";
            compText += nameof(AnalyticsID) + " - " + AnalyticsID + "\n";
            compText += nameof(SubscriptionCount) + " - " + SubscriptionCount.ToString() + "\n";
            compText += nameof(IsLastToPurchase) + " - " + IsLastToPurchase.ToString() + "\n";
            compText += nameof(PurchaseToken) + " - " + PurchaseToken + "\n";
            compText += nameof(PackageName) + " - " + PackageName + "\n";
            compText += nameof(PurchaseDate) + " - " + PurchaseDate.ToString() + "\n";
            compText += nameof(PurchaseMetadata) + " - \n" + PurchaseMetadata.ToString("    ");
            return compText;
        }

        public void SetProductData(ProductIdentifier productInfo, ProductStore productStore)
        {
            StoreID = productStore;
            ProductID = GetStoreSpecificID(productInfo);
            AnalyticsID = productInfo.AnalyticsID;
            LocalProductID = productInfo.LocalProductID;
            PurchaseType = productInfo.ProductType;
        }

        public string GetStoreSpecificID(ProductIdentifier product) => StoreID == ProductStore.PlayStore ? product.GoogleProductID : product.AppleProductID;

        public void ClearPurchaseInfo()
        {
            HasReceipt = false;
            Receipt = string.Empty;
            IsPurchaseValid = false;
            PurchaseMetadata.SetEmptyData();
            ClearSubscriptionState();
        }

        public void ClearSubscriptionState()
        {
            IsSubscribed = false;
            IsExpired = false;
            IsCanceled = false;
            IsFreeTrial = false;
        }


        public bool Equals(PurchaseData other)
        {
            if (other is null) return false;

            bool rec = HasReceipt != other.HasReceipt;
            bool valid = IsPurchaseValid != other.IsPurchaseValid;
            bool pID = ProductID != other.ProductID;
            bool lod = LocalProductID != other.LocalProductID;
            bool expD = ExpirationDate != other.ExpirationDate;
            bool canD = StartData != other.StartData;
            bool sub = IsSubscribed != other.IsSubscribed;
            bool exp = IsExpired != other.IsExpired;
            bool can = IsCanceled != other.IsCanceled;
            bool fre = IsFreeTrial != other.IsFreeTrial;
            bool store = StoreID != other.StoreID;
            bool pType = PurchaseType != other.PurchaseType;
            bool trID = TransactionID != other.TransactionID;
            bool lastP = IsLastToPurchase != other.IsLastToPurchase;

            return !(valid || rec || pID || lod || expD || canD || sub || exp || can || store || pType || trID || fre || lastP);
        }

        public static bool operator ==(PurchaseData lhs, PurchaseData rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }

        public override bool Equals(object obj) => Equals(obj as PurchaseData);
        public override int GetHashCode() => (ExpirationDate, ProductID, StartData, IsSubscribed, IsExpired, IsCanceled, HasReceipt, IsPurchaseValid, IsFreeTrial, StoreID, PurchaseType, TransactionID).GetHashCode();
        public static bool operator !=(PurchaseData lhs, PurchaseData rhs) => !(lhs == rhs);
    }
}
