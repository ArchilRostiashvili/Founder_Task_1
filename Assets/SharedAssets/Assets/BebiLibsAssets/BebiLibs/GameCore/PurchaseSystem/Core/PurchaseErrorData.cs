public abstract class PurchaseErrorData
{
    public bool IsErrorNative { get; protected set; }
    public int NativeErrorEnumID { get; protected set; }

    public abstract string GetFailureReason();

    public override string ToString()
    {
        return GetFailureReason();
    }
}