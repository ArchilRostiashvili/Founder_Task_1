using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{

    public class PromotionDebugger : MonoBehaviour
    {

        public PromotionConfigData PromotionConfigData;
        public TMP_Text StartTimeText;

        private void OnEnable()
        {
            UpdateStartDateText();
        }

        public void UpdateStartDateText()
        {
            StartTimeText.text = PromotionConfigData.promotionStartDate.ToString();
        }

        public void SubtractDay()
        {
            System.DateTime startDate = PromotionConfigData.promotionStartDate;
            System.DateTime newStartDate = startDate.Subtract(System.TimeSpan.FromDays(1));
            PromotionConfigData.UpdateStartDate(newStartDate);
            UpdateStartDateText();
        }

    }
}
