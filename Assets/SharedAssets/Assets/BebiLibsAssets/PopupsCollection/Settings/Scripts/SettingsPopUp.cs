using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs.Analytics;
using BebiLibs.PurchaseSystem;
using BebiLibs.PurchaseSystem.Core;
using BebiLibs.PopupManagementSystem;
using BebiLibs.GameApplicationConfig;
using BebiLibs.AudioSystem;
using BebiLibs.RemoteConfigSystem;

namespace BebiLibs
{
    public class SettingsPopUp : PopUpBase
    {

        public static SettingsPopUp Instance;
        public static event Action SettingPopUpCloseEvent;

        [Header("Background Music")]
        public SettingsToggle SC_BackgroundMusic;

        [Header("Sound Effects")]
        public SettingsToggle SC_SoundEffects;

        [Header("Hand Side")]
        public SettingsToggle SC_HandSide;

        [Header("Age Group")]
        public List<GameObject> arrayToggle = new List<GameObject>();
        public static Action<int> CallBack_AgeGroupChange;

        public RectTransform RT_ButtonTerms;
        public GameObject GO_ButtonRestore;
        public GameObject GO_PopUpButton;

        [SerializeField] private GraphicColorDimmer _restoreButtonDimmer;
        [SerializeField] private PurchaseManagerBase _purchaseManager;

        private Action _callBack_OnHide;

        private bool _analyticsPrevMusic;
        private bool _analyticsPrevEffects;

        public LocalizationPanel SC_LocalizationPanel;

        private bool _soundBackGroundOnStart;
        private bool _soundOnEffectsOnStart;
        private int _ageGroupOnStart;
        private bool _handSideOnStart;

        public string settingParams = "";
        private bool _isRestoreButtonPressed;

        override public void Init()
        {
            base.Init();
            // #if !UNITY_IOS
            //             GO_ButtonRestore.SetActive(false);
            //             //RT_ButtonTerms.anchoredPosition = new Vector2(0.0f, RT_ButtonTerms.anchoredPosition.y);
            // #endif
            GO_ButtonRestore.SetActive(true);
            _restoreButtonDimmer.RestoreColor();

        }

        private void OnEnable()
        {
            _purchaseManager.PurchaseRestoredEvent += OnPurchaseRestore;
            _restoreButtonDimmer.RestoreColor();
            _isRestoreButtonPressed = false;
        }

        private void OnDisable()
        {
            _purchaseManager.PurchaseRestoredEvent -= OnPurchaseRestore;
        }

        private void OnPurchaseRestore(bool isSuccessfull)
        {
            if (isSuccessfull)
            {
                ManagerSounds.PlayEffect("fx_successhigh1");
            }
            _restoreButtonDimmer.RestoreColor();
            _isRestoreButtonPressed = false;
        }

        public static void Activate()
        {
            if (Instance == null)
            {
                PopupManager.GetPopup<SettingsPopUp>(popup =>
                {
                    Instance = popup;
                    Instance.Show(true);
                });
            }
            else
            {
                Instance.Show(true);
            }
        }

        private void SetBackGroundViewActive(bool active)
        {
            SC_BackgroundMusic.SetToggleActive(active);
        }

        private void SetSoundEffectView(bool active)
        {
            SC_SoundEffects.SetToggleActive(active);
        }

        private void SetHandSideView(bool handSide)
        {
            SC_HandSide.SetToggleActive(!handSide);
        }

        //group is from 0 to 2, 0 for 1-2, 1 for 2-3, 2 for 3-4 ❤
        private void SetAgeGroupView(int group)
        {
            arrayToggle.ForEach(x => x.SetActive(false));
            arrayToggle[group].SetActive(true);
        }

        public void Trigger_ButtonClick_SetAgeGroup(int group)
        {
            ManagerSounds.PlayEffect("fx_page17");
            CallBack_AgeGroupChange?.Invoke(group);
            ManagerApp.Config.AgeGroup = group;
            SetAgeGroupView(group);
        }

        public void Trigger_ButtonClick_SetHandSide()
        {
            ManagerSounds.PlayEffect("fx_page17");
            ManagerApp.Config.HandSide = !ManagerApp.Config.HandSide;
            SetHandSideView(ManagerApp.Config.HandSide);
        }

        private bool GetSettingParam(out String stringParams)
        {
            bool changeBack = ManagerSounds.SoundOnBackgrounds != _soundBackGroundOnStart;
            bool changeEffect = ManagerSounds.SoundOnEffects != _soundOnEffectsOnStart;
            bool changeAge = ManagerApp.Config.AgeGroup != _ageGroupOnStart;
            bool changeHandSide = ManagerApp.Config.HandSide != _handSideOnStart;

            string soundBackGround = (changeBack ? "-" : "") + (ManagerSounds.SoundOnBackgrounds ? "2/" : "1/");
            string soundEffect = (changeEffect ? "-" : "") + (ManagerSounds.SoundOnEffects ? "2/" : "1/");
            string age = (changeAge ? "-" : "") + (ManagerApp.Config.AgeGroup + 1).ToString() + "/";
            string handSide = (changeHandSide ? "-" : "") + (ManagerApp.Config.HandSide ? 2 : 1).ToString();
            stringParams = soundBackGround + soundEffect + age + handSide;
            return changeBack || changeEffect || changeAge || changeHandSide;
        }

        public override void Show(bool anim)
        {
            //ManagerAnalytics.SetScene("p_set");//pop_settings
            //ManagerAnalytics.LogEvent("p_set_s");//pop_settings_show

            //Image_Language.sprite = ManagerLocalization.GetLanguageIcon(ManagerLocalization.CurrentLocalLanguage);
            SC_LocalizationPanel.Show();

            _soundBackGroundOnStart = ManagerSounds.SoundOnBackgrounds;
            _soundOnEffectsOnStart = ManagerSounds.SoundOnEffects;
            _ageGroupOnStart = ManagerApp.Config.AgeGroup;
            _handSideOnStart = ManagerApp.Config.HandSide;

            _analyticsPrevMusic = ManagerSounds.SoundOnBackgrounds;
            _analyticsPrevEffects = ManagerSounds.SoundOnEffects;

            base.Show(anim);
            ManagerSounds.PlayEffect("fx_page15");

            SetSoundEffectView(ManagerSounds.SoundOnEffects);
            SetBackGroundViewActive(ManagerSounds.SoundOnBackgrounds);
            SetAgeGroupView(ManagerApp.Config.AgeGroup);
            SetHandSideView(ManagerApp.Config.HandSide);
            CheckRateButtonStatus();
            //ManagerOtherApps.CheckToShowOtherAppsBanner(popupID, 0.54f);
        }

        private void CheckRateButtonStatus()
        {
            RatePopup.UpdateRateButtonState();
            GO_PopUpButton.SetActive(RatePopup.CanShowRateButton);
        }

        public override void Hide(bool anim)
        {
            if (GetSettingParam(out string settingParams))
            {
                AnalyticsManager.LogEvent("exit_settings", "state", settingParams);
            }

            _callBack_OnHide?.Invoke();

            base.Hide(anim);
            SC_LocalizationPanel.Hide();
            _popupParentGO.SetActive(false);
        }

        private IEnumerator DelayClose(float time)
        {
            yield return new WaitForSeconds(time);
            Hide(true);
        }

        public void Trigger_ButtonClick_SoundEffects()
        {
            ManagerSounds.PlayEffect("fx_page17");
            ManagerSounds.SoundOnEffects = !ManagerSounds.SoundOnEffects;
            AudioManager.SoundOnEffects = !AudioManager.SoundOnEffects;
            SetSoundEffectView(ManagerSounds.SoundOnEffects);
        }

        public void Trigger_ButtonClick_BackgroundMusic()
        {
            ManagerSounds.PlayEffect("fx_page17");
            ManagerSounds.SoundOnBackgrounds = !ManagerSounds.SoundOnBackgrounds;
            AudioManager.SoundOnBackgrounds = !AudioManager.SoundOnBackgrounds;
            SetBackGroundViewActive(ManagerSounds.SoundOnBackgrounds);
        }

        public void Trigger_ButtonClick_Restore()
        {
            if (_isRestoreButtonPressed)
            {
                ManagerSounds.PlayEffect("fx_wrong7");
                return;
            }
            _isRestoreButtonPressed = true;
            _restoreButtonDimmer.DimeColor();
            ManagerSounds.PlayEffect("fx_page17");
            _purchaseManager.RestorePurchases();
        }

        public void Trigger_ButtonClick_OpenMailReport()
        {
            MailReportSystem.SendReport();
        }

        public void Trigger_ButtonClick_OpenShare()
        {
            Hide(false);
            SharePopup.Activate("settings");
        }

        public void Trigger_ButtonClick_OpenRate()
        {
            Hide(false);
            RatePopup.Activate(RatePopup.MANUAL, "settings");
        }

        public override void Trigger_ButtonClick_Close()
        {
            SettingPopUpCloseEvent?.Invoke();
            //PopUp_Rate.Activate(PopUp_Rate.AUTO, "auto");
            ManagerSounds.PlayEffect("fx_page17");
            if (_analyticsPrevMusic != ManagerSounds.SoundOnBackgrounds || _analyticsPrevEffects != ManagerSounds.SoundOnEffects)
            {
                string state = (ManagerSounds.SoundOnBackgrounds ? 1 : 0) + "_" + (ManagerSounds.SoundOnEffects ? 1 : 0);
            }

            Hide(true);
        }

        public void Trigger_ButtonClick_TermsAndConditions()
        {
            ManagerSounds.PlayEffect("fx_page17");
            string privacyPolicyUrl = ApplicationConfigProvider.DefaultInstance().CurrentConfig.GetPrivacyPolicyUrl();
            Application.OpenURL(privacyPolicyUrl);
        }

        public void Trigger_ButtonClick_TermsOfUse()
        {
            ManagerSounds.PlayEffect("fx_page17");
            string termsOfUseUrl = ApplicationConfigProvider.DefaultInstance().CurrentConfig.GetTermsOfUseUrl();
            Application.OpenURL(termsOfUseUrl);
        }
    }
}
