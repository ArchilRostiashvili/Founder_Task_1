using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem
{
    public class RegistrationError
    {
        public long ErrorCode;
        public string ErrorMessage;

        public RegistrationError(long errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }
}
