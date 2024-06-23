using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace BebiLibs.PurchaseSystem
{
    public static class SubscriptionUtility
    {
        public static bool TryGetSubscriptionInfo(Product item, IAppleExtensions iAppleExtensions, out SubscriptionInfo info)
        {
            try
            {
                Dictionary<string, string> introductory_info_dict = iAppleExtensions.GetIntroductoryPriceDictionary();
                info = null;
                if (item.receipt != null)
                {
                    ProductDefinition definition = item.definition;
                    if (definition.type == ProductType.Subscription)
                    {
                        string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(definition.storeSpecificId)) ? null : introductory_info_dict[definition.storeSpecificId];
                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
                        info = p.getSubscriptionInfo();
                        return true;
                    }
                    else
                    {
#if !UNITY_EDITOR
                        Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "The product is not a subscription product");
#endif
                    }
                }
                else
                {

#if !UNITY_EDITOR
                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, $"The product {item.definition.storeSpecificId} should have a valid receipt");
#endif
                }
                return false;
            }
            catch (StoreSubscriptionInfoNotSupportedException ex)
            {
                Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "Store Is Not Supported , Error :" + ex.Message);
                info = null;
                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "Unable To Parse Subscription: Error {0}", e.Message);
                info = null;
                return false;
            }
        }
    }
}
#endif