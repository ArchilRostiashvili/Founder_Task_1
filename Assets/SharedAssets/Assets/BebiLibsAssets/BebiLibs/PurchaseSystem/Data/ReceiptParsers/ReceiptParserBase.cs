using Newtonsoft.Json;

namespace BebiLibs
{
    using BebiLibs.PurchaseSystem;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public abstract class ReceiptParserBase : ScriptableObject
    {
        public virtual void ParseReceipt(PurchaseData purchaseData)
        {

        }

        public virtual List<string> GetJsonModels(List<PurchaseData> purchasedSubscription)
        {
            return new List<string>();
        }
    }
}
