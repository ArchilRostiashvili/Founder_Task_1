using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "PromotionTextContainerSO", menuName = "BebiLibs/ExtendedFreeTrialSystem/PromotionTextContainerSO", order = 0)]
    public class PromotionTextContainerSO : ScriptableObject
    {
        [SerializeField] private PromotionPopUpText _promotionPopUp;
        public PromotionPopUpText PromotionPopUpText => _promotionPopUp;
    }
}
