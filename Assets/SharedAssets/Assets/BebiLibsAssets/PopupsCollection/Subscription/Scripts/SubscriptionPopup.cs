using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Analytics;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.PurchaseSystem;
using BebiLibs.PopupManagementSystem;

namespace BebiLibs
{
    public class SubscriptionPopup : PopUpBase
    {
        private const int _PANEL_MAX_SHOW_TIME = 10;
        public static event System.Action<string, bool> PopUpHideEvent;
        public static string ShopActivationOrigin;
        public static bool CanOpenRegistration;
        private static double _LastClickTime = 0;

        public static SubscriptionPopup Instance;

        [Header("Purchase Data: ")]
        [SerializeField] BooleanVariable canUserSubscriptionPopup;
        [SerializeField] private PurchaseHistoryData _purchaseHistory;
        [SerializeField] PanelSubscription _activePanel;
        [SerializeField] SubscriptionPanelData _subscriptionPanelData;

        public static void Activate(ButtonActions buttonActions, string activationOrigin, bool anim, PromoAudioMode playPromoSound, bool canOpenRegistration)
        {
            ShopActivationOrigin = activationOrigin;
            CanOpenRegistration = canOpenRegistration;

            if (Instance == null)
            {
                InitializeInstance();
            }

            ActivatePopup(buttonActions, activationOrigin, anim, playPromoSound, Instance);
        }

        public static void InitializeInstance()
        {
            if (Instance == null)
            {
                PopupManager.GetPopup<SubscriptionPopup>(popup =>
                {
                    Instance = popup;
                });
            }
        }




        private static void ActivatePopup(ButtonActions buttonActions, string activationOrigin, bool anim, PromoAudioMode playPromoSound, SubscriptionPopup controller)
        {
            if (!controller._isInitialized) controller.Init();

            if (!Instance.canUserSubscriptionPopup)
            {
                PopUpHideEvent?.Invoke("force_close", false);
                return;
            }

            bool isFreeTrial = controller._purchaseHistory.SubscriptionTotalCount == 0;
            playPromoSound = isFreeTrial ? playPromoSound | PromoAudioMode.FREE_TRIAL : playPromoSound;
            LockedContentSound.PlaySubscribeVoice(playPromoSound);

            PanelSubscription.ShopActivationOrigin = activationOrigin;
            controller._activePanel.ButtonActions = buttonActions;
            controller.Show(anim);
        }

        public static void HidePanel(bool anim)
        {
            if (Instance == null) return;
            Instance.Hide(anim);
        }

        public override void Show(bool anim)
        {
            _popUpCanvas.worldCamera = Camera.main;
            _activePanel.CallBack_Cancel = () => Hide(true);
            _activePanel.Show(anim, _subscriptionPanelData);
            base.Show(anim);
        }

        public override void Hide(bool anim)
        {
            LockedContentSound.StopPlayingSubscriptionVoices();
            if (_activePanel != null)
            {
                _activePanel.Hide();
            }

            base.Hide(anim);
            PopUpHideEvent?.Invoke(ShopActivationOrigin, CanOpenRegistration);
        }


        public static bool CanShowPopup()
        {
            if (Instance == null)
            {
                InitializeInstance();
            }

            double now = Time.realtimeSinceStartupAsDouble;
            if (_LastClickTime == 0)
            {
                _LastClickTime = now;
                return true;
            }
            else if (now - _LastClickTime > _PANEL_MAX_SHOW_TIME)
            {
                _LastClickTime = now;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
