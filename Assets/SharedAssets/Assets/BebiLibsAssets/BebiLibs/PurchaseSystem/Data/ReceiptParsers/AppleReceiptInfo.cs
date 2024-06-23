namespace BebiLibs
{
    [System.Serializable]
    public class AppleReceiptInfo
    {
        public string receiptData;

        public AppleReceiptInfo(string receiptData)
        {
            this.receiptData = receiptData;
        }

        public override string ToString()
        {
            return receiptData;
        }
    }
}
