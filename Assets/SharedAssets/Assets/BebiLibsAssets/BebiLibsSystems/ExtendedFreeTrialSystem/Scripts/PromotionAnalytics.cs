using BebiLibs.Analytics;
using BebiLibs.Analytics.GameEventLogger;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    public class PromotionAnalytics : AnalyticsHelperBase
    {
        [SerializeField] private PromotionConfigData _promotionConfigData;

        private void OnEnable()
        {
            _promotionConfigData.PromotionExpiredEvent += OnPromotionExpired;
            _promotionConfigData.PromotionStartEvent += OnPromotionStart;
        }

        private void OnDisable()
        {
            _promotionConfigData.PromotionExpiredEvent += OnPromotionExpired;
            _promotionConfigData.PromotionStartEvent += OnPromotionStart;
        }

        private void OnPromotionStart()
        {
            GameParameterBuilder parameterBuilder = new GameParameterBuilder();
            parameterBuilder.Add("promotion_length", _promotionConfigData.promotionLength);
            parameterBuilder.Add("promotion_start_date", _promotionConfigData.promotionStartDate.ToString("s"));
            parameterBuilder.Add("promotion_warning", _promotionConfigData.PromotionWarningLength);
            parameterBuilder.Add("promotion_expiration_date", _promotionConfigData.promotionDueDate.ToString("s"));
            AnalyticsManager.LogEvent("promotion_started", parameterBuilder);
        }

        private void OnPromotionExpired()
        {
            GameParameterBuilder parameterBuilder = new GameParameterBuilder();
            parameterBuilder.Add("promotion_length", _promotionConfigData.promotionLength);
            parameterBuilder.Add("promotion_start_date", _promotionConfigData.promotionStartDate.ToString("s"));
            parameterBuilder.Add("promotion_warning", _promotionConfigData.PromotionWarningLength);
            parameterBuilder.Add("promotion_expiration_date", _promotionConfigData.promotionDueDate.ToString("s"));
            AnalyticsManager.LogEvent("promotion_expired", parameterBuilder);
        }

    }
}
