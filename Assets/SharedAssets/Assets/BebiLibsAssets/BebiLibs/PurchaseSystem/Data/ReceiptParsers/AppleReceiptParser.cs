namespace BebiLibs
{
    using BebiLibs.PurchaseSystem;
    using Newtonsoft.Json;
    using SimpleJSON;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AppleReceiptData", menuName = "BebiLibs/PurchaseSystem/AppleReceiptData", order = 0)]
    public class AppleReceiptParser : ReceiptParserBase
    {

        public override void ParseReceipt(PurchaseData purchaseData)
        {
            try
            {
                AppStoreReceipt appStoreReceipt = JsonConvert.DeserializeObject<AppStoreReceipt>(purchaseData.Receipt);
                purchaseData.Payload = appStoreReceipt.Payload;
            }
            catch (System.Exception e)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Unable Parse Apple Subscription Receipt, Error: {0}", e);
                purchaseData.Payload = "error parsing payload, from AppleReceiptParser";
            }
        }

        public override List<string> GetJsonModels(List<PurchaseData> purchasedSubscription)
        {
            List<string> jsonModels = new List<string>();

            foreach (var purchaseData in purchasedSubscription)
            {
                AppleReceiptInfo appleReceiptInfo = new AppleReceiptInfo(purchaseData.Payload);
                string jsonModel = JsonConvert.SerializeObject(appleReceiptInfo);
                jsonModels.Add(jsonModel);
            }

            return jsonModels;
        }


        [System.Serializable]
        public class AppStoreReceipt
        {
            public string Payload;
            public AppStoreReceipt()
            {
            }
        }
    }
}
