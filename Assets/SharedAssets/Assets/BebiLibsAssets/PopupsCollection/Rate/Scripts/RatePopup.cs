using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BebiLibs.Analytics;
using BebiLibs.RemoteConfigSystem;
using BebiLibs.ABTestingSystem;
using BebiLibs.PopupManagementSystem;
using BebiLibs.Localization;
using BebiLibs.GameApplicationConfig;
//using VoxelBusters.EssentialKit;

namespace BebiLibs
{
    public class RatePopup : PopUpBase
    {
        public enum ActivationType
        {
            MANUAL, AUTO, NOTIFICATION
        }

        public const ActivationType MANUAL = ActivationType.MANUAL;
        public const ActivationType AUTO = ActivationType.AUTO;
        public const ActivationType NOTIFICATION = ActivationType.NOTIFICATION;

        public ActivationType activationType { get; private set; }
        public string activationSuffix => activationType == BebiLibs.RatePopup.MANUAL ? "manual" : "auto";
        public string activationOrigin;

        [Header("Rate Pop Up Components")]
        private static RatePopup instance;

        [SerializeField] private int _maxInteractiveDelayDayCount = 2;
        [SerializeField] private int _maxRateDelayDayCount = 5;

        [SerializeField] private BasePopUpRatePanel _popUpInteractive;
        [SerializeField] private List<BasePopUpRatePanel> _arrayPopUpPanels = new List<BasePopUpRatePanel>();


        private readonly PersistentBoolean _isFirstTimeRunningReview = new PersistentBoolean("IsFirstTimeRunningReview", true);
        private static readonly PersistentBoolean isAutoReviewUsed = new PersistentBoolean("IsInteractiveReviewUsed", false);
        private readonly PersistentInteger _gameLaunchCount = new PersistentInteger("GameLaunchCount", 0);
        private static readonly PersistentInteger _userReviewValue = new PersistentInteger("UserIntractiveReviewPrefValue", 0);

        private static readonly PersistentBoolean _IsExperimentDisabled = new PersistentBoolean("isRateExperimentDisabled", false);
        private BasePopUpRatePanel _activeBasePopUpPanel;


        public static bool IsSubscribed;
        public static PersistentInteger PurchasedDayCount = new PersistentInteger("PurchasedDayCount", 0);
        public static bool CanShowRateButton { get; private set; }
        public static bool HasUserUsedAutoReview => isAutoReviewUsed;

        [SerializeField] private RateExperiment _rateExperiment;

        public void InitializeExperiment()
        {
            if (RemoteConfigManager.TryGetBool("georgian_mode", out bool _isGeoModeEnabled, false))
            {
                if (_isGeoModeEnabled)
                    _IsExperimentDisabled.SetValue(true);
            }

            RateExperimentVariant defaultVariant = new RateExperimentVariant(_arrayPopUpPanels[0], 1, _arrayPopUpPanels[0].PopupID);
            _rateExperiment = RateExperiment.CreateExperiment("rate_pop_v4_test_key", defaultVariant);

            if (!_rateExperiment.IsStarted && !LocalizationManager.IsEnglish())
            {
                _IsExperimentDisabled.SetValue(true);
            }

            _IsExperimentDisabled.SetValue(true);
            if (_IsExperimentDisabled)
            {
                _activeBasePopUpPanel = _rateExperiment.DefaultVariant.BasePopUpRatePanel;
                return;
            };

            float probability = 1.0f / _arrayPopUpPanels.Count;
            foreach (var item in _arrayPopUpPanels)
            {
                _rateExperiment.AddVariant(new RateExperimentVariant(item, probability, item.PopupID));
            }

            if (_rateExperiment.IsStarted == false)
            {
                AnalyticsManager.SetProperty("popup_rate_v4", _rateExperiment.ActiveVariant.VariantID);
                AnalyticsManager.LogEvent("popup_rate_v4", "value", _rateExperiment.ActiveVariant.VariantID);
            }

            _activeBasePopUpPanel = _rateExperiment.ActiveVariant.BasePopUpRatePanel;
        }

        public override void Init()
        {
            InitializeExperiment();
            base.Init();
            instance = this;
            _gameLaunchCount.SetValue(_gameLaunchCount + 1);
            _activeBasePopUpPanel = _arrayPopUpPanels[0];
            CanShowRateButton = _userReviewValue.GetValue() >= 0;
        }

        public static void UpdateRateButtonState()
        {
            CanShowRateButton = false;
            if (!RemoteConfigManager.TryGetString("rate_popup_data", out string data)) return;
            if (!RateRemoteData.LoadFromJson(data, out RateRemoteData rateRemoteData)) return;

            if (!rateRemoteData.IsEnabled) return;

            //Debug.Log("UpdateRateButtonState " + rateRemoteData);

            if (!IsSubscribed && ApplicationSessionInfo.PlayedDayCount > rateRemoteData.MinActivateDayRangeForFreeUsers)
            {
                CanShowRateButton = true;
            }

            if (IsSubscribed && PurchasedDayCount > rateRemoteData.MinActivateDayRangeForPaidUsers)
            {
                CanShowRateButton = true;
            }

            CanShowRateButton = CanShowRateButton && _userReviewValue.GetValue() >= 0;
        }

        public static void Activate(ActivationType activationType, string origin)
        {
            if (instance == null)
            {
                PopupManager.GetPopup<RatePopup>(popup =>
                {
                    instance = popup;
                    ActivatePopup(activationType, origin);
                });
            }
            else
            {
                ActivatePopup(activationType, origin);
            }
        }

        private static void ActivatePopup(ActivationType activationType, string origin)
        {
            if (!CanShowRateButton) return;
            instance.UpdateAndShowCurrentPanel(activationType, origin);
        }

        public void SetUserReview(int value)
        {
            //Debug.LogError("7777 Set Review");
            _userReviewValue.SetValue(value);
            isAutoReviewUsed.SetValue(true);
            _gameLaunchCount.SetValue(0);

            string logText = value >= 0 ? "good_rate" : "bad_rate";
            AnalyticsManager.LogEvent(logText);
        }


        private void UpdateAndShowCurrentPanel(ActivationType activationType, string origin)
        {
            this.activationType = activationType;
            activationOrigin = origin;



            if (activationType == ActivationType.MANUAL)
            {
                ActivateElementsFromList();
                Show(true);
            }
            else if (activationType == ActivationType.AUTO && LocalizationManager.ActiveLanguage != LocalizationManager.Georgian)
            {
                if (!isAutoReviewUsed && _gameLaunchCount > _maxInteractiveDelayDayCount)
                {
                    _activeBasePopUpPanel = _popUpInteractive;
                    Show(true);
                }
                else if (_gameLaunchCount > _maxRateDelayDayCount)
                {
                    _gameLaunchCount.SetValue(0);
                    ActivateElementsFromList();
                    Show(true);
                }
            }
            else if (activationType == ActivationType.NOTIFICATION)
            {
                _activeBasePopUpPanel = _popUpInteractive;
                Show(true);
            }
        }

        public override void Show(bool anim)
        {
            TR_Content = _activeBasePopUpPanel.transform;
            base.Show(anim);
            ManagerSounds.PlayEffect("fx_page15");
            AnalyticsManager.LogEvent("show_rate", "bid", activationSuffix);
            AnalyticsManager.LogEvent("show_rate_" + activationSuffix, "bid", activationOrigin);
            _activeBasePopUpPanel.Show(this);
        }



        private void ActivateElementsFromList()
        {
            if (_rateExperiment != null)
            {
                _activeBasePopUpPanel = _rateExperiment.ActiveVariant.BasePopUpRatePanel;
            }
            else
            {
                _activeBasePopUpPanel = _arrayPopUpPanels[0];
                Debug.LogError($"{nameof(_popupID)} value {_popupID} is out of bounds of array {nameof(_arrayPopUpPanels)} with count {_arrayPopUpPanels.Count}");
            }
        }


        public void Trigger_ButtonClick_Rate() => Rate();
        public override void Trigger_ButtonClick_Close() => ClosePopup();

        internal void Rate()
        {
            _activeBasePopUpPanel.Activate(this);
            instance.Hide(false);
        }

        internal void ClosePopup()
        {
            ManagerSounds.PlayEffect("fx_page17");
            Hide(false);
        }

        public override void Hide(bool anim)
        {
            _activeBasePopUpPanel.Hide(this);
            base.Hide(anim);

            AnalyticsManager.LogEvent("close_rate", "bid", activationSuffix);
            AnalyticsManager.LogEvent("close_rate_" + activationSuffix, "bid", activationOrigin);
        }


        public void GoToRate()
        {
            if (_isFirstTimeRunningReview.GetValue() == true)
            {
                MoveToInAppRate();
            }
            else
            {
                MoveToStoreRate();
            }
            _isFirstTimeRunningReview.SetValue(false);
        }

        public void GoToReport()
        {
            PopUp_ThankAndReport.Activate(false);
        }

        private void MoveToInAppRate()
        {
            //Utilities.RequestStoreReview();
            BebiPlugins.RequestInAppReview();
        }

        private void MoveToStoreRate()
        {
#if UNITY_ANDROID
            Application.OpenURL($"market://details?id={Application.identifier}");
#elif UNITY_IOS
            string url = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id=" + ApplicationConfigProvider.DefaultInstance().CurrentConfig.GetStoreSpecificID() + "&onlyLatestVersion=true&pageNumber=0&sortOrdering=1&type=Purple+Software";
            Application.OpenURL(url);
#endif
        }

        public static void DebagClose()
        {
            instance.Hide(false);
        }

        public static void DebugShow(int index)
        {
            instance._activeBasePopUpPanel = instance._arrayPopUpPanels[index];
            instance.Show(true);
        }

        public static void DebugShowInteractive()
        {
            instance._activeBasePopUpPanel = instance._popUpInteractive;
            instance.Show(true);
        }

        public class RateExperiment : Experiment<RateExperimentVariant>
        {
            public static RateExperiment CreateExperiment(string experimentName, RateExperimentVariant defaultExperiment)
            {
                RateExperiment experiment = ScriptableObject.CreateInstance<RateExperiment>();
                experiment.SetExperimentParameters(experimentName, defaultExperiment);
                return experiment;
            }
        }
        public class RateExperimentVariant : Variant
        {
            [SerializeField] private BasePopUpRatePanel _basePopUpRatePanel;
            public BasePopUpRatePanel BasePopUpRatePanel => _basePopUpRatePanel;
            public RateExperimentVariant(BasePopUpRatePanel popUpRatePanel, float probability, string name = "default") : base(probability, name)
            {
                _basePopUpRatePanel = popUpRatePanel;
            }
        }

        [System.Serializable]
        public class RateRemoteData
        {
            public bool IsEnabled = false;
            public int MinActivateDayRangeForFreeUsers = 9;
            public int MinActivateDayRangeForPaidUsers = 9;

            public static bool LoadFromJson(string json, out RateRemoteData rateRemoteData)
            {
                rateRemoteData = null;
                try
                {
                    rateRemoteData = JsonUtility.FromJson<RateRemoteData>(json);
                    if (rateRemoteData == null)
                        Debug.LogError($"Error while parsing json {json} to {nameof(RateRemoteData)}: {nameof(rateRemoteData)} is null");

                    return rateRemoteData != null;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while parsing json {json} to {nameof(RateRemoteData)}: {e.Message}");
                    return false;
                }
            }

            public override string ToString()
            {
                return $"IsEnabled: {IsEnabled}, MinActivateDayRangeForFreeUsers: {MinActivateDayRangeForFreeUsers}, MinActivateDayRangeForPaidUsers: {MinActivateDayRangeForPaidUsers}";
            }
        }

    }
}
