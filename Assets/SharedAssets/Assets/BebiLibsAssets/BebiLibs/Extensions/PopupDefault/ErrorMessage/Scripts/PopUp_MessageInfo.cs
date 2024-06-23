using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using TMPro;
using BebiLibs.Analytics.GameEventLogger;
using BebiLibs.Analytics;
using BebiLibs.PopupManagementSystem;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BebiLibs
{
    public class PopUp_MessageInfo : PopUpBase
    {
        private const string _DEFAULT_TEXT_ID = "TEXT_SOMETHING_WENT_WRONG";

        private static PopUp_MessageInfo _Instance;
        private static System.Action _ButtonClickEvent;
        private static string _Sender;


        [SerializeField] private MessageInfoType _infoMessageType;
        private MessageInfoType _lastInfoMessage;

        [SerializeField] private List<ErrorMessagePanel> _errorMessagePanels = new List<ErrorMessagePanel>();
        [SerializeField] private ErrorMessagePanel activePanel;


        public MessageInfoType testMessageInfoType = MessageInfoType.Default;
        private static string _Error;

        public override void Init()
        {
            base.Init();
            for (int i = 0; i < _errorMessagePanels.Count; i++)
            {
                _errorMessagePanels[i].Init(this);
            }
            _Instance = this;
        }

        private void OnValidate()
        {
            if (_infoMessageType != _lastInfoMessage)
            {
                OpenInfoPanel(_infoMessageType);
                _lastInfoMessage = _infoMessageType;
            }
        }

        public static void Activate(MessageInfoType errorMessageType, string sender, System.Action callback_OnButtonAction = null, string error = null)
        {
            _Error = error;
            if (_Instance == null)
            {
                PopupManager.GetPopup<PopUp_MessageInfo>(popup =>
                {
                    _Instance = popup;
                    ActivatePopup(errorMessageType, sender, callback_OnButtonAction);
                });
            }
            else
            {
                ActivatePopup(errorMessageType, sender, callback_OnButtonAction);
            }
        }

        public static void Activate(string infoText = null, System.Action callback_OnButtonAction = null)
        {
            if (_Instance == null)
            {
                PopupManager.GetPopup<PopUp_MessageInfo>(popup =>
                {
                    _Instance = popup;
                    ActivatePopup(infoText, callback_OnButtonAction);
                });
            }
            else
            {
                ActivatePopup(infoText, callback_OnButtonAction);
            }
        }

        private static void ActivatePopup(MessageInfoType errorMessageType, string sender, Action callback_OnButtonAction)
        {
            _Sender = sender;
            _ButtonClickEvent = callback_OnButtonAction;
            _Instance.OpenInfoPanel(errorMessageType);
            _Instance.Show(true);
        }

        private static void ActivatePopup(string infoText, Action callback_OnButtonAction)
        {
            _ButtonClickEvent = callback_OnButtonAction;
            _Instance.OpenInfoPanel(MessageInfoType.Default, infoText);
            _Instance.Show(false);
        }

        public void TriggerEndAction()
        {
            _ButtonClickEvent?.Invoke();
        }

        private void OpenInfoPanel(MessageInfoType errorMessageType, string infoText = null)
        {
            ErrorMessagePanel errorMessagePanel = _errorMessagePanels.Find(x => x.messageInfoType == errorMessageType);
            base.TR_Content = errorMessagePanel.transform;
            for (int i = 0; i < _errorMessagePanels.Count; i++)
            {
                _errorMessagePanels[i].Hide();
            }

            if (infoText != null)
            {
                activePanel.SetInfoText(infoText);
            }

            activePanel = errorMessagePanel;
            activePanel.Show(_Error);
            AnalyticsManager.LogEvent(activePanel.panel_name + "_show", "bid", _Sender);
        }

        private string TranslateErrorMessage(string errorMessage)
        {
            string message = errorMessage;
            if (errorMessage != null)
            {
                if (!LocalizationManager.TryGetTranslation(errorMessage, out message))
                {
                    message = errorMessage;
                }
            }
            return message;
        }

        public override void Trigger_ButtonClick_Close()
        {
            ManagerSounds.PlayEffect("fx_page17");
            Hide(false);
        }

        public override void Hide(bool anim)
        {
            AnalyticsManager.LogEvent(activePanel.panel_name + "_close", "bid", _Sender);
            base.Hide(false);
        }
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(PopUp_MessageInfo))]
    public class PopUp_MessageInfoEditor : Editor
    {
        private PopUp_MessageInfo _popUp_MessageInfo;

        private void OnEnable()
        {
            _popUp_MessageInfo = (PopUp_MessageInfo)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Start Test PopUp"))
            {
                PopUp_MessageInfo.Activate(_popUp_MessageInfo.testMessageInfoType, "test", () =>
                {
                    Debug.Log("Test Ended");
                });
            }
        }
    }
#endif
}
