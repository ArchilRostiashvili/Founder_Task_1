using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace BebiLibs
{
    public class PurchaseFailureData : PurchaseErrorData
    {
        private string _purchaseError;

        public PurchaseFailureData(PurchaseFailureReason failureReason)
        {
            IsErrorNative = true;
            NativeErrorEnumID = (int)failureReason;
            _purchaseError = failureReason.ToString();
        }

        public override string GetFailureReason()
        {
            return _purchaseError;
        }
    }
}
#endif