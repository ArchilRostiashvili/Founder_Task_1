using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using BebiLibs.PurchaseSystem.Core;

namespace BebiLibs.PurchaseSystem
{

    [CreateAssetMenu(fileName = "ReceiptDataSO", menuName = "BebiLibs/PurchaseSystem/ReceiptDataSO", order = 0)]
    public class PurchaseHistoryData : ScriptableObject, IResetOnPreBuild
    {
        private const string _PURCHASE_HISTORY_PREF_KEY = "ReceiptDataSOPRefName";
        public event System.Action PurchaseDataChangedEvent;

        [SerializeField] private List<PurchaseData> _purchasesDataList = new List<PurchaseData>();
        [SerializeField] private List<PurchaseData> _previousPurchasesDataList = new List<PurchaseData>();
        [SerializeField] private ReceiptParserBase _receiptParser;

        [JsonIgnore()]
        [SerializeField] private bool _testMode = false;

        public bool IsPurchaseDataInitialized { get; private set; }

        public List<PurchaseData> PurchasesDataList => _purchasesDataList;
        public List<PurchaseData> PreviousPurchasesDataList => _previousPurchasesDataList;
        public bool HasAnyPurchasedNonConsumable => _purchasesDataList.Any(x => x.IsPurchaseValid && x.PurchaseType == LocalPurchaseType.NonConsumable);
        public bool HasAnyActiveSubscription => _purchasesDataList.Any(x => x.IsPurchaseValid && x.IsSubscribed && x.PurchaseType == LocalPurchaseType.Subscription);
        public bool HasAnyNonExpiredSubscription => _purchasesDataList.Any(x => x.IsPurchaseValid && x.IsSubscribed && !x.isPurchaseExpired);
        public PurchaseData FirstActiveSubscription => _purchasesDataList.FirstOrDefault(x => x.IsPurchaseValid && x.IsSubscribed);
        public int SubscriptionTotalCount => _purchasesDataList.Sum(x => x.SubscriptionCount);

        internal void Initialize(ReceiptParserBase receiptParse)
        {
            _receiptParser = receiptParse;
            ResetData();
            LoadDataFromMemory();
        }

        public List<string> GetSubscriptionReceiptInfoList()
        {
            List<PurchaseData> purchasedSubscription = _purchasesDataList.OrderByDescending(x => x.PurchaseDate.DateTime).Where(x => x.IsPurchaseValid && x.IsSubscribed).ToList();
            return _receiptParser.GetJsonModels(purchasedSubscription);
        }


        public List<PurchaseData> GetAllPurchasedProducts()
        {
            return _purchasesDataList.Where(x => x.IsPurchaseValid).ToList();
        }

        public List<PurchaseData> GetPurchasedDataWith(LocalPurchaseType purchaseType, bool purchaseState)
        {
            return _purchasesDataList.Where(x => x.IsPurchaseValid == purchaseState && x.PurchaseType == purchaseType).ToList();
        }

        public bool TryGetPurchaseData(string productID, out PurchaseData purchaseData)
        {
            purchaseData = _purchasesDataList.Find(x => x.LocalProductID == productID || x.ProductID == productID);
            return purchaseData != null;
        }

        internal void LoadDataFromMemory()
        {
            JsonHandler.TryPopulateObjectFromPlayerPref(_PURCHASE_HISTORY_PREF_KEY, this, out string jsonText);
            //Debug.LogError("Purchase History: Load Data From Memory: " + jsonText);
        }

        internal void SaveJsonModelToMemory()
        {
            JsonHandler.TrySaveObjectIntoPlayerPref(_PURCHASE_HISTORY_PREF_KEY, this, out string jsonText);
            //Debug.LogError("Purchase History: Save Data Into Memory: " + jsonText);
        }


        internal void UpdateGamePurchaseData(List<PurchaseData> purchaseData)
        {
            if (_testMode && Application.isEditor)
            {
                IsPurchaseDataInitialized = true;
                return;
            }

            ParseReceiptData(purchaseData);

            if (UpdatePurchaseHistory(purchaseData))
            {
                PurchaseDataChangedEvent?.Invoke();
            }
        }

        internal void ParseReceiptData(List<PurchaseData> purchaseData)
        {
            foreach (var item in purchaseData)
            {
                if (item.HasReceipt)
                {
                    _receiptParser.ParseReceipt(item);
                }
            }
        }

        private bool UpdatePurchaseHistory(List<PurchaseData> newPurchaseData)
        {
            bool isDirty = false;
            foreach (var newPurchaseEntry in newPurchaseData)
            {
                PurchaseData newPurchase = newPurchaseEntry.Copy();
                PurchaseData oldPurchase = _purchasesDataList.Find(x => x.LocalProductID == newPurchase.LocalProductID);
                if (oldPurchase != null)
                {
                    if (oldPurchase != newPurchase)
                    {
                        UpdatePurchaseCount(newPurchase, oldPurchase.SubscriptionCount);
                        UpdateOldPurchaseData(oldPurchase);
                        _purchasesDataList.Remove(oldPurchase);
                        _purchasesDataList.Add(newPurchase);
                        isDirty = true;
                    }

                }
                else
                {
                    isDirty = true;
                    UpdatePurchaseCount(newPurchase, 0);
                    _purchasesDataList.Add(newPurchase);
                }
            }

            if (isDirty)
            {
                SaveJsonModelToMemory();
            }

            // foreach (var item in _purchasesDataList)
            // {
            //     Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, $"{item} \n At Frame: {Time.frameCount} \n ---------------------");
            // }

            IsPurchaseDataInitialized = true;
            return isDirty;
        }

        public void UpdateOldPurchaseData(PurchaseData oldPurchaseData)
        {
            _previousPurchasesDataList.RemoveAll(x => x.LocalProductID == oldPurchaseData.LocalProductID);
            _previousPurchasesDataList.Add(oldPurchaseData);
        }

        public void UpdatePurchaseCount(PurchaseData newPurchase, int lastSavedSubscriptionCount)
        {
            bool isValidPurchase = newPurchase.IsSubscribed && !newPurchase.IsCanceled && !newPurchase.IsFreeTrial && !newPurchase.IsExpired;
            newPurchase.SubscriptionCount = isValidPurchase ? lastSavedSubscriptionCount + 1 : lastSavedSubscriptionCount;
        }

        public void ClearInitialization()
        {
            IsPurchaseDataInitialized = false;
        }


        private void ResetData()
        {
            IsPurchaseDataInitialized = false;
            _purchasesDataList.Clear();
            _previousPurchasesDataList.Clear();
        }

        public void ResetOnPreBuild()
        {
            ResetData();
        }
    }
}
