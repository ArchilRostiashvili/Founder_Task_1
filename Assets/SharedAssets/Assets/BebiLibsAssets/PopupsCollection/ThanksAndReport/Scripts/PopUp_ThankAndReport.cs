using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BebiLibs.PopupManagementSystem;

namespace BebiLibs
{
    public class PopUp_ThankAndReport : PopUpBase
    {

        public static PopUp_ThankAndReport instance;

        [Header("UI")]
        public TMP_Text Text_Header;
        public TMP_Text Text_Message;
        public TMP_Text Text_MessageTwo;

        [Header("Texts")]
        public string rateHeaderText;
        public string rateMessageOne;
        public string reteMessageTwo;

        public string notificationHeader;
        public string notificationMessageOne;
        public string notificationMessageTwo;

        public override void Init()
        {
            base.Init();
            instance = this;
        }

        public static void Activate(bool fromNotification)
        {
            if (instance == null)
            {
                PopupManager.GetPopup<PopUp_ThankAndReport>(popup =>
                {
                    instance = popup;
                    ActivatePopup(fromNotification);
                });
            }
            else
            {
                ActivatePopup(fromNotification);
            }
        }

        private static void ActivatePopup(bool fromNotification)
        {
            instance.InitializePopUp(fromNotification);
            instance.Show(true);
        }

        public void InitializePopUp(bool fromNotification)
        {
            if (fromNotification)
            {
                Text_Header.text = notificationHeader;
                Text_Message.text = notificationMessageOne;
                Text_MessageTwo.text = notificationMessageTwo;
            }
            else
            {
                Text_Header.text = rateHeaderText;
                Text_Message.text = rateMessageOne;
                Text_MessageTwo.text = reteMessageTwo;
            }
        }

        public override void Show(bool anim)
        {
            ManagerSounds.PlayEffect("fx_page15");
            base.Show(anim);
        }

        public override void Hide(bool anim)
        {
            base.Hide(anim);
        }

        public void Trigger_ButtonClick_OnReportClick()
        {
            ManagerSounds.PlayEffect("fx_page15");
            MailReportSystem.SendReport();
            Hide(false);
        }

        public override void Trigger_ButtonClick_Close()
        {
            ManagerSounds.PlayEffect("fx_page17");
            base.Trigger_ButtonClick_Close();
        }

    }
}
