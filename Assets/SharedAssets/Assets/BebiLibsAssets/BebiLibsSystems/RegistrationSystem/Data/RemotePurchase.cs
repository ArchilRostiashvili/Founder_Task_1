namespace BebiLibs.RegistrationSystem.Remote
{
    using BebiLibs.PurchaseSystem;
    using System;

    public enum AppProductType
    {
        Subscription = 10,
        NonConsumable = 20,
        Consumable = 30
    }

    [System.Serializable]
    public class RemotePurchase
    {
        public AppProductType productTypeId;
        public string productId;
        public string id;
        public string partnerId;
        public ProductStore storeId;
        public SDateTime startTime = new SDateTime(System.DateTime.UtcNow);
        public SDateTime expiryTime = new SDateTime(System.DateTime.UtcNow);


        public bool IsSubscribed => startTime.DateTime < DateTime.UtcNow && DateTime.UtcNow < expiryTime.DateTime;

        public override string ToString()
        {
            string text = "";
            text += nameof(productTypeId) + ": " + productTypeId + "\n";
            text += nameof(productId) + ": " + productId + "\n";
            text += nameof(id) + ": " + id + "\n";
            text += nameof(storeId) + ": " + storeId + "\n";
            text += nameof(startTime) + ": " + startTime + "\n";
            text += nameof(expiryTime) + ": " + expiryTime + "\n";
            return text;
        }

        public PurchaseData GetSubscriptionState()
        {
            return new PurchaseData()
            {
                ExpirationDate = expiryTime,
                StartData = startTime,
                StoreID = storeId,
                IsCanceled = !IsSubscribed,
                IsSubscribed = IsSubscribed,
                IsExpired = !IsSubscribed
            };
        }
    }
}
