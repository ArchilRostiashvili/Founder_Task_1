using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    using BebiLibs.Localization;
    using UnityEngine;
    [CreateAssetMenu(fileName = "PromotionRemoteData", menuName = "BebiLibs/ExtendedFreeTrialSystem/PromotionRemoteData", order = 0)]
    public class PromotionRemoteData : ScriptableObject
    {
        public bool IsEnabled = false;
        public int FreeTrialLength;
        public int WarningLength;
        public int PromotionLength;
        public List<PromotionPopUpText> PromotionPopUpTextsList = new List<PromotionPopUpText>();

        public void ResetData()
        {
            IsEnabled = false;
            FreeTrialLength = 0;
            WarningLength = 0;
            PromotionPopUpTextsList.Clear();
        }

        public bool TryGetActivePromotionText(out PromotionPopUpText promotionPopUpText)
        {

            promotionPopUpText = PromotionPopUpTextsList.Find(x => x.languageID == LocalizationManager.ActiveLanguageUniversalID);
            return promotionPopUpText != null;
        }

    }
}
