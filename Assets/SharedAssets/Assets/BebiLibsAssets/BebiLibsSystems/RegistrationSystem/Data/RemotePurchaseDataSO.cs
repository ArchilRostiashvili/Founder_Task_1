using BebiLibs.ServerConfigLoaderSystem.Core;
using BebiLibs.RegistrationSystem.Core;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using BebiLibs.ServerConfigLoaderSystem;

namespace BebiLibs.RegistrationSystem.Remote
{
    [CreateAssetMenu(fileName = "PurchasedData", menuName = "BebiLibs/RegistrationSystem/PurchaseData", order = 0)]
    public class RemotePurchaseDataSO : BaseRequestData
    {
        [JsonIgnore()]
        public GameUserDataSO _userData;

        [SerializeField] private bool error;
        [SerializeField] private string message;
        [SerializeField] private List<RemotePurchase> purchases;

        public bool hasError => error;
        public List<RemotePurchase> PurchasesList => purchases;

        public override void ResetData()
        {
            error = false;
            message = string.Empty;
            purchases.Clear();
        }

        public override void LoadDataFromMemory()
        {
            ResetData();
            base.LoadDataFromMemory();
            bool isModified = false;
            for (int i = purchases.Count - 1; i >= 0; i--)
            {
                if (purchases[i].expiryTime.DateTime < System.DateTime.UtcNow)
                {
                    purchases.RemoveAt(i);
                    isModified = true;
                }
            }
            if (isModified)
            {
                SaveDataToMemory();
            }
        }

        public void SignUserOut()
        {
            ResetData();
            SaveDataToMemory();
        }


        protected override bool ValidateDeserializedData()
        {
            return !error;
        }

        public override void ModifyRequestHeader(UnityWebRequest request)
        {
            base.ModifyRequestHeader(request);
            BaseRequestHandler.SetBearerToken(request, _userData);
        }

        public void LogData(string header)
        {
            try
            {
                if (isLoaded)
                {
                    int count = PurchasesList.Count;
                    string log = $"{header} Remote Purchase List {(count > 0 ? "Has " + count + " Items:  \n" : "Is Empty")}";
                    foreach (var item in PurchasesList)
                    {
                        log += $"Product ID: {item.productId}, PartnerID: {item.partnerId}, ID {item.id}";
                    }
                    Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, this, log);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Unable To Log Remote Pyrchase Data, Error: " + e);
            }
        }
    }
}
