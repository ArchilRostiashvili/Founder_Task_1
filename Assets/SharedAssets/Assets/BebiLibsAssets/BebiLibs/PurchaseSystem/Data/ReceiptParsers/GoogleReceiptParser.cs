using BebiLibs.PurchaseSystem;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BebiLibs
{

    [CreateAssetMenu(fileName = "GoogleReceiptData", menuName = "BebiLibs/PurchaseSystem/GoogleReceiptData", order = 0)]
    public class GoogleReceiptParser : ReceiptParserBase
    {
        public override void ParseReceipt(PurchaseData purchaseData)
        {
            // try
            // {
            //     PlayStoreReceipt playStoreReceipt = JsonConvert.DeserializeObject<PlayStoreReceipt>(receipt);
            //     if (!playStoreReceipt.TryLoadPayLoad(out PayloadString payloadString))
            //     {
            //         Debug.LogError("Unable To Get payload sting");
            //         return;
            //     }

            //     if (!payloadString.TryLoadJsonReceipt(out JsonReceipt jsonReceipt))
            //     {
            //         Debug.LogError("Unable To Get jsonRecept sting");
            //         return;
            //     }

            //     Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "GoogleReceiptParser: jsonReceipt: {0}, {1}", jsonReceipt, purchaseData.ProductID);

            // }
            // catch (System.Exception e)
            // {
            //     if (!Debug.isDebugBuild)
            //         Debug.LogError("Unable To Parse Play Store Receipt, Error: " + e);
            // }
        }

        public override List<string> GetJsonModels(List<PurchaseData> purchasedSubscription)
        {
            List<string> jsonModels = new List<string>();

            foreach (var purchaseData in purchasedSubscription)
            {
                GoogleReceiptInfo googleReceiptInfo = new GoogleReceiptInfo(purchaseData.PackageName, purchaseData.ProductID, purchaseData.PurchaseToken);
                string jsonModel = JsonConvert.SerializeObject(googleReceiptInfo);
                //Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "GoogleReceiptParser: jsonModel: {0}, {1}", jsonModel, purchaseData.ProductID);
                jsonModels.Add(jsonModel);
            }
            return jsonModels;
        }
    }


    [System.Serializable]
    public class PlayStoreReceipt
    {
        public string Payload;
        public PlayStoreReceipt()
        {
        }

        public bool TryLoadPayLoad(out PayloadString payloadString)
        {
            try
            {
                payloadString = JsonConvert.DeserializeObject<PayloadString>(Payload);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable To Parse Payload, Error: " + e);
                payloadString = null;
                return false;
            }
        }
    }

    [System.Serializable]
    public class PayloadString
    {
        public string json;
        public PayloadString()
        {
        }

        public bool TryLoadJsonReceipt(out JsonReceipt jsonReceipt)
        {
            try
            {
                jsonReceipt = JsonConvert.DeserializeObject<JsonReceipt>(json);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Unable To Parse Payload, Error: " + e);
                jsonReceipt = null;
                return false;
            }
        }
    }

    [System.Serializable]
    public class JsonReceipt
    {
        public string orderId;
        public string productId;
        public int purchaseState;
        public string purchaseToken;

        public JsonReceipt()
        {
        }
        public override string ToString()
        {
            return string.Format("orderId: {0}\n, productId: {1}\n, purchaseState: {2}\n, purchaseToken: {3}", orderId, productId, purchaseState, purchaseToken);
        }
    }
}
