using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.ServerConfigLoaderSystem.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using System;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.RegistrationSystem;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.PurchaseSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IssueReportDataCollector : MonoBehaviour
{
    [SerializeField] private IssueReportData _issueReportData;
    [SerializeField] private AbstractInitData _serverInitData;
    [SerializeField] private PurchaseManagerBase _purchaseManager;
    [SerializeField] private PurchaseHistoryData _purchaseHistoryData;

    [SerializeField] private string _reportURL;
    void Start()
    {
        _issueReportData = new IssueReportData()
        {
            ProductID = Application.identifier,
            AppVersion = Application.version,
            OperatingSystem = SystemInfo.operatingSystem,
            DeviceModel = SystemInfo.deviceModel,
            LocalDeviceID = SystemInfo.deviceUniqueIdentifier,
            SystemMemorySize = SystemInfo.systemMemorySize,
            GraphicsMemorySize = SystemInfo.graphicsMemorySize,
        };
    }

    private void UpdateReportDataState()
    {
        _issueReportData.HasAnyActiveSubscription = _purchaseHistoryData.HasAnyActiveSubscription;
        _issueReportData.HasAnyPurchasedNonConsumable = _purchaseHistoryData.HasAnyPurchasedNonConsumable;
        _issueReportData.FirebaseUserID = RemoteConfigManager.UserInstallationID;
        _serverInitData.GetDeviceID(out string deviceID);
        _issueReportData.RemoteDeviceID = deviceID;
    }

    public void StartReport()
    {
        UpdateReportDataState();
        if (TryGetBase64ReportJsonData(out string base64Data))
        {
            Application.OpenURL(_reportURL + "?ReportData=" + base64Data);
        }

        MailReportSystem.SendReport();
    }

    public bool TryGetBase64ReportJsonData(out string base64)
    {
        if (JsonHandler.TrySerializeObjectToJson(_issueReportData, out string jsonData))
        {
            Debug.Log(jsonData);
            base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonData));
            return true;
        }

        base64 = string.Empty;
        return false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IssueReportDataCollector))]
public class IssueReportDataCollectorEditor : Editor
{
    private IssueReportDataCollector _issueReportDataCollector;

    private void OnEnable()
    {
        _issueReportDataCollector = (IssueReportDataCollector)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Start Report"))
        {
            _issueReportDataCollector.StartReport();
        }
    }
}
#endif
