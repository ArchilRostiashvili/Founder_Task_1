using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using BebiLibs.Analytics.GameEventLogger;
using BebiLibs.PopupManagementSystem;
using BebiLibs.Analytics;

namespace BebiLibs.ExtendedFreeTrialSystem
{
    public class PopUp_Promotion : PopUpBase
    {
        private static PopUp_Promotion _Instance;
        private static bool _UseParental;

        [SerializeField] private PromotionConfigData _promotionConfig;
        [SerializeField] private List<PromotionStateData> _promotionStateDataList;
        public static event System.Action PromotionPopupClosedEvent;
        public static event System.Action SubscriptionButtonClickedEvent;

        public override void Init()
        {
            base.Init();
            _Instance = this;
        }

        public static void Activate(bool useParental)
        {
            if (_Instance == null)
            {
                PopupManager.GetPopup<PopUp_Promotion>((popup) =>
                {
                    _Instance = popup;
                    ActivatePopup(useParental);
                });
            }
            else
            {
                ActivatePopup(useParental);
            }
        }

        public static void ActivatePopup(bool useParental)
        {
            if (_Instance != null)
            {
                _UseParental = useParental;
                _Instance.Show(true);
            }
            else
            {
                Debug.Log($"{nameof(PopUp_Promotion)} instance is not initialized inside ManagerPopUp");
            }
        }

        public override void Show(bool anim)
        {
            _popUpCanvas.worldCamera = Camera.main;

            AnalyticsManager.LogEvent("open_promotion_popup", "type", _promotionConfig.PromotionState.ToString().ToLower());
            PromotionStateData promotionStateData = _promotionStateDataList.Find(x => x.PromotionState == _promotionConfig.PromotionState);
            if (promotionStateData != null)
            {
                ManagerSounds.PlayEffect("fx_page16");
                promotionStateData.PromotionPopUpObjectGO.SetActive(false);
                TR_Content = promotionStateData.PromotionPopUpObjectGO.transform;
                base.Show(anim);
            }
        }

        private void CloseAllPanels(bool anim)
        {
            // _promotionStateDataList.ForEach(x => x.PromotionPopUpObjectGO.SetActive(false));
            AnalyticsManager.LogEvent("close_promotion_popup", "type", _promotionConfig.PromotionState.ToString().ToLower());
            base.Hide(anim);
            ManagerSounds.PlayEffect("fx_page17");
            PromotionPopupClosedEvent?.Invoke();
        }

        public void OnCloseButtonClick() => CloseAllPanels(true);
        public void OnSubmitButtonClick() => CloseAllPanels(true);

        public void OnSubscribeButtonClick(Transform triggerPoint)
        {
            if (_UseParental)
            {
                ParentalController.Activate(triggerPoint.position, () =>
                {
                    Hide(false);
                    SubscriptionButtonClickedEvent?.Invoke();
                });
            }
            else
            {
                Hide(false);
                SubscriptionButtonClickedEvent?.Invoke();
            }
        }


        [System.Serializable]
        public class PromotionStateData
        {
            public GameObject PromotionPopUpObjectGO;
            public PromotionState PromotionState;
        }
    }
}
