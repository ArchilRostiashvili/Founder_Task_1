namespace BebiLibs.PurchaseSystem
{
    [System.Serializable]
    public class PurchaseDataPair
    {
        public PurchaseData NewPurchase;
        public PurchaseData OldPurchase;

        public PurchaseDataPair(PurchaseData newPurchaseData, PurchaseData oldPurchaseData)
        {
            NewPurchase = newPurchaseData;
            OldPurchase = oldPurchaseData;
        }
    }
}
