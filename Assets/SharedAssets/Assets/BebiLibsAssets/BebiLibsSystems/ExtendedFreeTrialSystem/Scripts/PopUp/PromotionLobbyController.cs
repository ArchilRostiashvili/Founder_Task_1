using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    public class PromotionLobbyController : MonoBehaviour
    {
        [Header("Promotion Controller")]
        [SerializeField] protected PromotionConfigData promotionConfig;

        [Header("Lobby Controller")]
        [SerializeField] private GameObject _promotionActiveButtonGO;
        [SerializeField] private GameObject _promotionExpiredButtonGO;
        [SerializeField] private GameObject _promotionExpiringButtonGO;


        private void OnEnable()
        {
            InitializeCurrentLobbyButton();
            promotionConfig.PromotionStateChangedEvent += OnPromotionChanged;
        }

        private void OnDisable()
        {
            promotionConfig.PromotionStateChangedEvent -= OnPromotionChanged;
        }

        private void OnPromotionChanged(PromotionState state)
        {
            InitializeCurrentLobbyButton();
        }

        public void InitializeCurrentLobbyButton()
        {
            _promotionActiveButtonGO.SetActive(promotionConfig.PromotionState == PromotionState.ACTIVE);
            _promotionExpiringButtonGO.SetActive(promotionConfig.PromotionState == PromotionState.EXPIRING);
            _promotionExpiredButtonGO.SetActive(false);
        }

        public void OnPromotionPopUpOpenButtonClick(Transform triggerPoint)
        {
            //ManagerSounds.PlayEffect("fx_page15");
            ParentalController.Activate(triggerPoint.position, () =>
            {
                PopUp_Promotion.Activate(false);
            });
        }

    }
}
