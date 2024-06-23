using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace BebiLibs
{
    public class InitializationFailureData : PurchaseErrorData
    {
        private string _purchaseError;

        public InitializationFailureData(InitializationFailureReason failureReason)
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