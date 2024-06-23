using BebiLibs.Analytics;
using BebiLibs.PopupManagementSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.RegistrationSystem.PopUp
{
    public class RegistrationInfoPopup : PopUpBase
    {
        private static RegistrationInfoPopup _Instance;
        private static string _Sender;

        [SerializeField] private RegistrationInfoType _defaultInfoType;
        [SerializeField] private RegistrationInfoType _infoType;
        private RegistrationInfoType _lastInfoType;

        [SerializeField] private List<RegistrationInfoPanel> _registrationInfoPanels = new List<RegistrationInfoPanel>();
        [SerializeField] private RegistrationInfoPanel _activePanel;

        internal Action OkButtonClickEvent { get; private set; }
        internal Action CancelButtonClickEvent { get; private set; }
        internal Action CloseButtonClickEvent { get; private set; }

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < _registrationInfoPanels.Count; i++)
            {
                _registrationInfoPanels[i].Init(this);
            }
            _Instance = this;
        }

        private void OnValidate()
        {
            if (_infoType != _lastInfoType)
            {
                OpenInfoPanel(_infoType);
                _lastInfoType = _infoType;
            }
        }

        public static void LoadPanelInstance(System.Action callback)
        {
            if (_Instance == null)
            {
                PopupManager.GetPopup<RegistrationInfoPopup>(popup =>
                {
                    _Instance = popup;
                    callback?.Invoke();
                });
            }
            else
            {
                callback?.Invoke();
            }
        }

        public static void Activate(RegistrationInfoType errorMessageType, bool animate = false, string sender = "auto", Action onOkButtonClick = null, Action onCancelButtonClick = null, Action onCloseButtonClick = null)
        {
            LoadPanelInstance(() =>
            {
                ActivatePopup(errorMessageType, animate, sender, onOkButtonClick, onCancelButtonClick, onCloseButtonClick);
            });
        }

        private static void ActivatePopup(RegistrationInfoType errorMessageType, bool animate = false, string sender = "auto", Action onOkButtonClick = null, Action onCancelButtonClick = null, Action onCloseButtonClick = null)
        {
            _Sender = sender;

            _Instance.OkButtonClickEvent = onOkButtonClick;
            _Instance.CancelButtonClickEvent = onCancelButtonClick;
            _Instance.CloseButtonClickEvent = onCloseButtonClick;

            _Instance.OpenInfoPanel(errorMessageType);
            _Instance.Show(animate);
        }


        private void OpenInfoPanel(RegistrationInfoType errorMessageType)
        {
            RegistrationInfoPanel RegistrationInfoPanel = _registrationInfoPanels.Find(x => x.PanelType == errorMessageType);
            base.TR_Content = RegistrationInfoPanel.transform;
            for (int i = 0; i < _registrationInfoPanels.Count; i++)
            {
                _registrationInfoPanels[i].Hide();
            }

            _activePanel = RegistrationInfoPanel;
            _activePanel.Show();
            AnalyticsManager.LogEvent(_activePanel.PanelName + "_show", "bid", _Sender);
        }

        public override void Hide(bool anim)
        {
            AnalyticsManager.LogEvent(_activePanel.PanelName + "_close", "bid", _Sender);
            base.Hide(false);
        }

        internal static void HidePopup(bool animate = false)
        {
            if (_Instance == null)
            {
                return;
            }

            _Instance.Hide(animate);
        }
    }
}
