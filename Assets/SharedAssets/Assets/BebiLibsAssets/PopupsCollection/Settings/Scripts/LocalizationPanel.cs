using BebiLibs.Analytics;
using BebiLibs.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class LocalizationPanel : MonoBehaviour
    {
        public List<LocalizePopUpElement> arrayChecks;

        public LocalizePopUpElement SC_ActiveElement;

        public GameObject GO_LanguageSwitchOpen;
        public GameObject GO_LanguageSwitchClose;

        private void OnEnable()
        {
            AddressableAudioManager.OnAudioAddressableAssetLoadEndEvent += OnAudioAddressableAssetLoadEndEvent;
        }

        private void OnDisable()
        {
            AddressableAudioManager.OnAudioAddressableAssetLoadEndEvent -= OnAudioAddressableAssetLoadEndEvent;
        }


        public void Trigger_ButtonClick_SwitchLanguage(LocalizePopUpElement localizePopUpElement)
        {
            ManagerSounds.PlayEffect("fx_page14");

            LocalizationManager.SetActiveLanguage(localizePopUpElement.localLanguageType);

            UpdateCurrentSelectedLanguageIcon();
            UpdateFlags();
            //Hide();
            AnalyticsManager.SetProperty("User_Lang", LocalizationManager.ActiveLanguageName);
            AnalyticsManager.LogEvent("Lang_Change", "lang", LocalizationManager.ActiveLanguageName);
            GO_LanguageSwitchOpen.SetActive(false);
            GO_LanguageSwitchClose.SetActive(true);

            if (LocalizationManager.IsEnglish() || LocalizationManager.ActiveLanguage == localizePopUpElement.localLanguageType)
            {
                Loader.Show("TEXT_CHANGE_LANGUAGE", 0.6f);
            }
            else
            {
                Loader.Show("TEXT_CHANGE_LANGUAGE", 6f);
            }
        }

        private void OnAudioAddressableAssetLoadEndEvent()
        {
            Loader.Hide();
        }

        public void Trigger_ButtonClick_OpenLocalizationPanel(LocalizePopUpElement localizePopUpElement)
        {
            ManagerSounds.PlayEffect("fx_page14");
            LocalizePopUpElement active = arrayChecks.Find(x => x.localLanguageType == localizePopUpElement.localLanguageType);
            active.transform.SetAsFirstSibling();
            UpdateFlags();

            GO_LanguageSwitchOpen.SetActive(true);
            GO_LanguageSwitchClose.SetActive(false);
        }

        private void UpdateFlags()
        {

            for (int i = 0; i < arrayChecks.Count; i++)
            {
                if (arrayChecks[i].localLanguageType == LocalizationManager.ActiveLanguage)
                {
                    arrayChecks[i].SetCheckMarkActive(true);
                }
                else
                {
                    arrayChecks[i].SetCheckMarkActive(false);
                }
            }
        }

        private void UpdateCurrentSelectedLanguageIcon()
        {

            Sprite sprite = LocalizationManager.ActiveLanguage.LanguageIcon;
            SC_ActiveElement.SetImage(sprite);
            SC_ActiveElement.localLanguageType = LocalizationManager.ActiveLanguage;
        }

        public void Show()
        {
            UpdateCurrentSelectedLanguageIcon();
            GO_LanguageSwitchOpen.SetActive(false);
            GO_LanguageSwitchClose.SetActive(true);
        }

        public void Hide()
        {
            GO_LanguageSwitchOpen.SetActive(false);
            GO_LanguageSwitchClose.SetActive(false);
        }
    }
}
