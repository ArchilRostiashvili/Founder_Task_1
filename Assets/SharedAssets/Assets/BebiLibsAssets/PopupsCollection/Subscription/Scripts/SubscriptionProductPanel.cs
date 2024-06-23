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
namespace BebiLibs
{
    public class SubscriptionProductPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _productPriceDisplayText;
        [SerializeField] private GameObject _startTrialButtonGO;
        [SerializeField] private GameObject _subscribeButtonGO;
        [SerializeField] private string _priceTextFormatStringKey = "TEXT_MONTH_AFTER_TRIAL_PERIOD_ENDS";

        [SerializeField] private bool _hasSalePrice = false;
        [SerializeField] private TMP_Text _salePriceText;

        [SerializeField] private Color _priceColor;
        [SerializeField] private bool _enablePriceFormating = false;

        public void UpdatePanel(bool canUserFreeTrial, string productPrice, string savingAmount = "", string isoCurrency = "")
        {
            _productPriceDisplayText.text = GetFormatedPrice(productPrice, _priceTextFormatStringKey);
            _startTrialButtonGO.SetActive(canUserFreeTrial);
            _subscribeButtonGO.SetActive(!canUserFreeTrial);

            if (_hasSalePrice)
                _salePriceText.text = $"{LocalizationManager.GetTranslation("TEXT_SAVE_MONEY")} {savingAmount} {isoCurrency}";
        }

        public string GetFormatedPrice(string price, string textFormat)
        {
            string formatedPrice = $"<color=#{ColorUtility.ToHtmlStringRGBA(_priceColor)}>{price}</color>";
            string newPrice = _enablePriceFormating ? formatedPrice : price;
            return newPrice + " / " + LocalizationManager.GetTranslation(textFormat);
        }

    }
}