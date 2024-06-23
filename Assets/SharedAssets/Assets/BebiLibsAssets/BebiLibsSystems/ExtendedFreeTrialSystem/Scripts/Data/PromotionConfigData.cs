using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using BebiLibs.PurchaseSystem.Core;

namespace BebiLibs.ExtendedFreeTrialSystem
{

    [CreateAssetMenu(fileName = "PromotionConfigData", menuName = "BebiLibs/ExtendedFreeTrialSystem/PromotionConfigData", order = 0)]
    public class PromotionConfigData : ScriptableObject
    {
        public const string PREF_NAME_KEY = "PromotionConfigDataPrefNames";
        public event Action PromotionStartEvent;
        public event Action PromotionExpiredEvent;
        public event Action<PromotionState> PromotionStateChangedEvent;

        [Header("Promotion State")]
        [JsonIgnore()]
        [SerializeField] private PromotionState _promotionState;
        [JsonIgnore()]
        [SerializeField] private PurchaseManagerBase _purchaseManager;

        [Header("Remote Promotion Data:")]
        [SerializeField] private PromotionRemoteData _promotionRemoteData;

        [Header("Remote Promotion Stats:")]
        [SerializeField] private SDateTime _promotionStartDate;
        [SerializeField] private bool _isPromotionStarted = false;
        [SerializeField] private bool _isRemoteDataLoaded = false;
        [SerializeField] private bool _isPromotionExpired = false;

        [Header("Editor Data: ")]
        [JsonIgnore()]
        [SerializeField] private List<PromotionTextContainerSO> _promTextContainerList;

        public bool isPromotionEnabled => _promotionRemoteData.IsEnabled;
        public bool isRemoteDataLoaded => _isRemoteDataLoaded;
        public bool isPromotionStarted => _isPromotionStarted;
        public bool isPromotionExpired => _isPromotionExpired;
        public bool isPromotionExpiring => remainingDayCount < _promotionRemoteData.WarningLength;
        public int PromotionWarningLength => _promotionRemoteData.WarningLength;
        public int promotionLength => _promotionRemoteData.PromotionLength;
        public int subscriptionTrialPeriod => _promotionRemoteData.FreeTrialLength;
        public int remainingDayCount => (promotionDueDate - DateTime.UtcNow).Days;
        public DateTime promotionDueDate => _promotionStartDate.DateTime + TimeSpan.FromDays(promotionLength);
        public DateTime promotionStartDate => _promotionStartDate.DateTime;
        public PromotionState PromotionState => _promotionState;

        public void ResetData()
        {
            _promotionRemoteData.ResetData();
            _promotionStartDate = SDateTime.UtcMin;
            _isPromotionStarted = false;
            _isRemoteDataLoaded = false;
            _isPromotionExpired = false;
        }

        internal void StartPromotion()
        {
            if (_isRemoteDataLoaded && _promotionRemoteData.IsEnabled)
            {
                _promotionStartDate = SDateTime.UtcNow;
                _isPromotionStarted = true;
                PromotionStartEvent?.Invoke();
            }
            else if (Debug.isDebugBuild)
            {
                //Debug.LogError("Unable Start Promotion, It's Data is not Loaded");
            }
        }

        internal void UpdatePromotionActiveState()
        {
            _isPromotionExpired = remainingDayCount <= 0;
            if (_isPromotionExpired)
            {
                PromotionExpiredEvent?.Invoke();
            }
        }

        internal void UpdatePromotionStateValue()
        {
            if (!_purchaseManager.IsSubscribed && isPromotionEnabled && isPromotionStarted)
            {
                _promotionState = isPromotionExpired ? PromotionState.EXPIRED : isPromotionExpiring ? PromotionState.EXPIRING : PromotionState.ACTIVE;
            }
            else
            {
                _promotionState = PromotionState.NO_PROMOTION;
            }
            PromotionStateChangedEvent?.Invoke(_promotionState);
        }


        public void LoadFromMemory()
        {
            JsonHandler.TryPopulateObjectFromPlayerPref(PREF_NAME_KEY, this, out string jsonFromMemory);
            //Debug.LogError("Load Json Data from Memory " + jsonFromMemory);
        }

        public void SaveIntoMemory()
        {
            JsonHandler.TrySaveObjectIntoPlayerPref(PREF_NAME_KEY, this, out string dataSavedintoMemory);
            //Debug.LogError("Save Json Data To Memory " + dataSavedintoMemory);
        }

        public PromotionPopUpText GetPromotionPopUpText()
        {
            bool hasActivePromotion = _promotionRemoteData.TryGetActivePromotionText(out PromotionPopUpText promotionPopUpText);
            return hasActivePromotion ? promotionPopUpText : _promTextContainerList[0].PromotionPopUpText;
        }

        public virtual bool TryLoadRemoteValue(string jsonText)
        {
            _isRemoteDataLoaded = JsonHandler.TryPopulateObjectFromJson(jsonText, _promotionRemoteData, false);
            return _isRemoteDataLoaded;
        }

        public string GetPromotionStartDateString()
        {
            System.DateTime dateTime = _promotionStartDate.DateTime;
            return DateToString(dateTime.Month) + "." + DateToString(dateTime.Day);
        }

        public string GetPromotionDueDate()
        {
            System.DateTime dateTime = promotionDueDate;
            return DateToString(dateTime.Month) + "." + DateToString(dateTime.Day);
        }

        private string DateToString(int date)
        {
            return (date / 10.0) < 1 ? "0" + date : date.ToString();
        }

        internal string PrintPromotionDataJson()
        {
            _promotionRemoteData.PromotionPopUpTextsList.Clear();
            foreach (var item in _promTextContainerList)
            {
                _promotionRemoteData.PromotionPopUpTextsList.Add(item.PromotionPopUpText.Clone());
            }

            JsonHandler.TrySerializeObjectToJson(_promotionRemoteData, out string jsonData);
            return jsonData;
        }

        internal string GetDefaultDataAsJsonString()
        {
            JsonHandler.TrySerializeObjectToJson(_promTextContainerList[0].PromotionPopUpText, out string jsonData);
            return jsonData;
        }

        internal void UpdateStartDate(DateTime dateTime)
        {
            _promotionStartDate = new SDateTime(dateTime);
            SaveIntoMemory();
        }
    }
}
