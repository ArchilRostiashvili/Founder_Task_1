using BebiLibs.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BebiLibs
{
    public class RatePanelInteractive : BasePopUpRatePanel
    {
        private int _starCount = 0;

        [Header("Report Button")]
        public ButtonScale ReportButton;
        public Image Image_ReportButtonCover;


        public override void Activate(RatePopup popUp_Rate)
        {
            ManagerApp.GoOutFromWhere = "r";
            ManagerApp.GoOutTimeSnap = TimeUtils.UnixTimeInSeconds();


            AnalyticsManager.LogEvent("start_game_rate");
            AnalyticsManager.LogEvent("rate", "starts", _starCount);


            if (_starCount > 3)
            {
                popUp_Rate.GoToRate();
                popUp_Rate.SetUserReview(1);
            }
            else
            {
                popUp_Rate.GoToReport();
                popUp_Rate.SetUserReview(-1);
            }
        }

        public void UpdateReportButtonState()
        {
            if (_starCount > 0)
            {
                this.SetReportButtonActive(true);
            }
            else
            {
                this.SetReportButtonActive(false);
            }
        }

        private void SetReportButtonActive(bool value)
        {
            this.ReportButton.buttonEnabled = value;
            this.Image_ReportButtonCover.gameObject.SetActive(!value);
        }


        public override void Hide(RatePopup popUp_Rate)
        {

        }

        public override void Show(RatePopup popUp_Rate)
        {
            this.gameObject.SetActive(true);
            ManagerApp.GoOutTimeSnap = 0;
            StarSelector.CallbackOnStarSelect = this.OnStarSelect;
            _starCount = 0;
            this.UpdateReportButtonState();
        }

        public void OnStarSelect(int starCount)
        {
            _starCount = starCount;
            this.UpdateReportButtonState();
        }
    }
}
