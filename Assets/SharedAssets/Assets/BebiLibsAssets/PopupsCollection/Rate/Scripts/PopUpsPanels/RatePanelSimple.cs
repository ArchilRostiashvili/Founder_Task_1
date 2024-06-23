using BebiLibs.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public class RatePanelSimple : BasePopUpRatePanel
    {
        public override void Activate(RatePopup popUp_Rate)
        {


            ManagerApp.GoOutFromWhere = "r";
            ManagerApp.GoOutTimeSnap = TimeUtils.UnixTimeInSeconds();


            AnalyticsManager.LogEvent("click_rate", "bid", popUp_Rate.activationSuffix);
            AnalyticsManager.LogEvent("click_rate_" + popUp_Rate.activationSuffix, "bid", popUp_Rate.activationOrigin);
            popUp_Rate.GoToRate();
        }

        public override void Hide(RatePopup popUp_Rate)
        {

        }

        public override void Show(RatePopup popUp_Rate)
        {
            this.gameObject.SetActive(true);
            ManagerApp.GoOutTimeSnap = 0;
        }
    }
}
