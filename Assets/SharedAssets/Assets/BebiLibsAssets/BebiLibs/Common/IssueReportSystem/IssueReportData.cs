using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IssueReportData
{
    public string ProductID;
    public string AppVersion;
    public string OperatingSystem;
    public string DeviceModel;
    public string LocalDeviceID;

    public int SystemMemorySize;
    public int GraphicsMemorySize;

    public string RemoteDeviceID;
    public string FirebaseUserID;

    public bool HasAnyActiveSubscription;
    public bool HasAnyPurchasedNonConsumable;
}
