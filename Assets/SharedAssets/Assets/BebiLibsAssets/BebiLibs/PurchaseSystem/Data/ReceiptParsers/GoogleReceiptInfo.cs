using UnityEngine;

namespace BebiLibs
{
    [System.Serializable]
    public class GoogleReceiptInfo
    {
        [Header("Recept Data:")]
        public string packageName;
        public string subscriptionId;

        [TextArea]
        public string purchaseToken;

        public GoogleReceiptInfo(string packageName, string subscriptionId, string purchaseToken)
        {
            this.packageName = packageName;
            this.subscriptionId = subscriptionId;
            this.purchaseToken = purchaseToken;
        }

        public override string ToString()
        {
            string output = "";
            output += nameof(packageName) + " " + packageName + "\n";
            output += nameof(subscriptionId) + " " + subscriptionId + "\n";
            output += nameof(purchaseToken) + " " + purchaseToken + "\n";
            return output;
        }
    }

}
