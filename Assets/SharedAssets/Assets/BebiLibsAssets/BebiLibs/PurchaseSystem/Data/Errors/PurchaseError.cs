using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class PurchaseError : PurchaseErrorData
    {
        private string _purchaseError;

        public PurchaseError(string purchaseError)
        {
            _purchaseError = purchaseError;
            IsErrorNative = false;
            NativeErrorEnumID = 0;
        }

        public override string GetFailureReason()
        {
            return _purchaseError;
        }
    }
}
