using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using TMPro;
using BebiLibs.Analytics;
using UnityEngine.UI;
using I2.Loc;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using BebiLibs.PurchaseSystem;
using BebiLibs.PurchaseSystem.Core;

namespace BebiLibs
{
    public class PanelSubscription : MonoBehaviour
    {
        [SerializeField] private Transform TR_Content;

        [Header("Button And Header")]
        [SerializeField] private SubscriptionProductPanel _monthlySubscriptionPanel;
        [SerializeField] private SubscriptionProductPanel _annualSubscriptionPanel;
        [SerializeField] private GameObject _freeTrialDescriptionTextGO;

        [SerializeField] private GameObject _freeTrialHeaderTextGO;
        [SerializeField] private GameObject _subscriptionHeaderTextGO;

        [Header("Purchase Data:")]
        [SerializeField] protected PurchaseHistoryData _purchaseHistory;
        [SerializeField] private ProductIdentifier _monthlyProduct;
        [SerializeField] private ProductIdentifier _yearlyProduct;
        private ProductIdentifier _baseProduct;
        [SerializeField] private PurchaseManagerBase _purchaseManager;

        [HideInInspector] public ButtonActions ButtonActions;

        [Tooltip("How frequent user be able to show subscription panel, Time in seconds")]
        public static string ShopActivationOrigin;
        private static long _LastOpenTime;

        private string _monthlyPrice = "6.99 USD";
        private string _annualPrice = "39.99 USD";
        private string _savingAmount = "28.99";
        private string _isoCurrency = "USD";

        public System.Action CallBack_Cancel;


        public void Show(bool anim, SubscriptionPanelData subscriptionPanelData)
        {
            this.gameObject.SetActive(true);

            if (subscriptionPanelData != null)
            {
                _monthlyProduct = subscriptionPanelData.MonthlySubscription;
                _yearlyProduct = subscriptionPanelData.YearlySubscription;
                _baseProduct = subscriptionPanelData.BaseMonthlySubscription;
            }

            _LastOpenTime = TimeUtils.UnixTimeInSeconds();
            SharedAnalyticsManager.ShopOpen(ShopActivationOrigin);
            ManagerSounds.PlayEffect("fx_page15");
            bool canUserFreeTrial = _purchaseHistory.SubscriptionTotalCount == 0;

            _freeTrialDescriptionTextGO.SetActive(canUserFreeTrial);
            _freeTrialHeaderTextGO.SetActive(canUserFreeTrial);
            _subscriptionHeaderTextGO.SetActive(!canUserFreeTrial);

            if (_purchaseManager.IsInitialized)
            {
                _purchaseHistory.TryGetPurchaseData(_monthlyProduct.LocalProductID, out PurchaseData subMonthly);
                _purchaseHistory.TryGetPurchaseData(_baseProduct.LocalProductID, out PurchaseData baseProduct);
                _monthlyPrice = subMonthly.PurchaseMetadata.localizedPriceWithCurrency;
                decimal localizedMonthlyPrice = subMonthly.PurchaseMetadata.localizedPrice;

                if (_baseProduct != null)
                {
                    _monthlyPrice = "<s>" + baseProduct.PurchaseMetadata.localizedPrice + "</s>   " + _monthlyPrice;
                }

                _monthlySubscriptionPanel.UpdatePanel(canUserFreeTrial, _monthlyPrice);

                _purchaseHistory.TryGetPurchaseData(_yearlyProduct.LocalProductID, out PurchaseData subYearly);
                _annualPrice = subYearly.PurchaseMetadata.localizedPriceWithCurrency;

                decimal localizedYearPrice = subYearly.PurchaseMetadata.localizedPrice;
                _savingAmount = ((localizedMonthlyPrice * 12) - localizedYearPrice).ToString("F", new System.Globalization.CultureInfo("en-US"));
                _isoCurrency = subYearly.PurchaseMetadata.isoCurrency;
                _annualSubscriptionPanel.UpdatePanel(canUserFreeTrial, _annualPrice, _savingAmount, _isoCurrency);
            }
            else
            {
#if UNITY_EDITOR
                _monthlySubscriptionPanel.UpdatePanel(canUserFreeTrial, _monthlyPrice);
                _annualSubscriptionPanel.UpdatePanel(canUserFreeTrial, _annualPrice, _savingAmount, _isoCurrency);
#endif
            }
        }


        public void Trigger_ButtonClick_BuyMonthly(Transform buttonTransform)
        {
            if (ButtonActions == ButtonActions.UseParental)
            {
                ParentalController.Activate(buttonTransform.position, () => this.Subscribe(_monthlyProduct));
            }
            else
            {
                this.Subscribe(_monthlyProduct);
            }
        }

        public void Trigger_ButtonClick_BuyYearly(Transform buttonTransform)
        {
            if (ButtonActions == ButtonActions.UseParental)
            {
                ParentalController.Activate(buttonTransform.position, () =>
                {
                    this.Subscribe(_yearlyProduct);
                });
            }
            else
            {
                this.Subscribe(_yearlyProduct);
            }
        }

        string noInternetError = "It appears that there is no internet connection. The purchase process may fail";
        private void Subscribe(ProductIdentifier subscriptionType)
        {
            bool isInternet = Application.internetReachability != NetworkReachability.NotReachable;
            string error = isInternet ? "" : noInternetError;

            ManagerSounds.PlayEffect("fx_gameready18");
            Loader.Show("TEXT_PURCHASING_PROCESS", 20.0f, error, true);
            _purchaseManager.BuyProduct(subscriptionType, ShopActivationOrigin);

            // #if UNITY_EDITOR
            //             ManagerSubscription.FakeUnlock();
            // #endif
        }

        public void Trigger_ButtonClick_Close()
        {
            this.CallBack_Cancel?.Invoke();
        }

        public void Hide()
        {
            ManagerSounds.PlayEffect("fx_page17");
            SharedAnalyticsManager.ShopClose(ShopActivationOrigin, TimeUtils.UnixTimeInSeconds() - _LastOpenTime);
        }
    }

    public enum ButtonActions
    {
        None, UseParental
    }
}