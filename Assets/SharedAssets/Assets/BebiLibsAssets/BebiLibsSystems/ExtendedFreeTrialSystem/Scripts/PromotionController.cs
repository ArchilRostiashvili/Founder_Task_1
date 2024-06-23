using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using BebiLibs.PurchaseSystem;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.PurchaseSystem.Core;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    public class PromotionController : MonoBehaviour
    {
        public const string REMOTE_CONFIG_KEY = "promotion_test_data";


        [Header("Purchase Data; ")]
        [SerializeField] private PurchaseManagerBase _purchaseManager;

        [Header("Promotion Data: ")]
        [SerializeField] private PromotionConfigData _configData;
        [SerializeField] private TextFormatContainer _promitionVariables;
        [SerializeField] private bool _isInitialized = false;

        [Header("Formating Data: ")]
        [SerializeField] private Color _textColor;
        [SerializeField] private FormatStringData _colorKey;
        [SerializeField] private FormatStringData _freeTrialDayCount;
        [SerializeField] private FormatStringData _promotionLength;
        [SerializeField] private FormatStringData _promotionDueDate;
        [SerializeField] private FormatStringData _promotionStartDate;
        [SerializeField] private FormatStringData _promotionRemainingDayCount;

        public bool IsInitialized => _isInitialized;
        public PromotionState PromotionState => _configData.PromotionState;

        private void Start()
        {
            _isInitialized = false;
            _configData.ResetData();
            _configData.LoadFromMemory();
        }

        public void InitializePromotionSystem()
        {
            if (_purchaseManager.IsSubscribed) return;
            if (!_configData.isPromotionStarted)
            {
                UpdateConfigDataFromRemoteConfig();
                _configData.StartPromotion();
                _configData.SaveIntoMemory();
            }
            else if (_configData.isPromotionStarted && !_configData.isPromotionExpired)
            {
                _configData.UpdatePromotionActiveState();
                _configData.SaveIntoMemory();
            }

            UpdatePromotionLocals();
            _configData.UpdatePromotionStateValue();
            _isInitialized = true;
        }

        private void UpdatePromotionLocals()
        {
            UpdateColorLocal();
            _freeTrialDayCount.SetValue(_configData.subscriptionTrialPeriod.ToString());
            _promotionLength.SetValue(_configData.promotionLength.ToString());
            _promotionRemainingDayCount.SetValue(_configData.remainingDayCount.ToString());
            _promotionDueDate.SetValue(_configData.GetPromotionDueDate());
            _promotionStartDate.SetValue(_configData.GetPromotionStartDateString());
        }

        public bool IsFreePromotionEnabled()
        {
            return !_configData.isPromotionExpired && _configData.isPromotionEnabled && _configData.isRemoteDataLoaded && _configData.isPromotionStarted;
        }

        private void UpdateConfigDataFromRemoteConfig()
        {
            if (RemoteConfigManager.TryGetString(REMOTE_CONFIG_KEY, out string promotionDataJson))
            {
                if (!_configData.TryLoadRemoteValue(promotionDataJson) && !Debug.isDebugBuild)
                {
                    Debug.LogError("Unable To Retrieve Values from promotionDataJson");
                }
            }
            else if (!Debug.isDebugBuild)
            {
                Debug.LogError($"Remote Config Does Not Have Value {REMOTE_CONFIG_KEY}");
            }
        }


        private void OnValidate()
        {
            UpdateColorLocal();
        }

        public void UpdateColorLocal()
        {
            string colorString = ColorUtility.ToHtmlStringRGBA(_textColor);
            string tmpColorString = $"<color=#{colorString}>";
            _colorKey.SetValue(tmpColorString);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

    }
}
