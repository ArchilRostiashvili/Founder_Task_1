using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem.Core
{
    public abstract class AbstractUserData : ScriptableObject
    {
        public abstract bool isUserSignedIn { get; }
        public abstract string userEmail { get; }
        public abstract string userName { get; }
        public abstract string userID { get; }
        public abstract string rawToken { get; }
        public abstract TimeSpan tokenExpiresIn { get; }

        public abstract bool GetRawUserToken(out string rawUserToken);
    }
}
