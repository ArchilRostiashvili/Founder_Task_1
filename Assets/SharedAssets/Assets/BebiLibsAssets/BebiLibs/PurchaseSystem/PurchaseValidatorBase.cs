using UnityEngine;

public abstract class PurchaseValidatorBase : ScriptableObject
{
    public abstract byte[] GetGooglePublicKey();
    public abstract byte[] GetAppleRootCert();
}