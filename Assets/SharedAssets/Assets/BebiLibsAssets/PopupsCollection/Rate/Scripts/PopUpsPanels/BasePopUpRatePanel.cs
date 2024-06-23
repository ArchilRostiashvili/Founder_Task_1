using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs
{
    public abstract class BasePopUpRatePanel : MonoBehaviour
    {
        [SerializeField] private string _popupID;
        [SerializeField] private RatePopup _popUp_Rate;
        public string PopupID => _popupID;

        public abstract void Activate(RatePopup popUp_Rate);
        public abstract void Hide(RatePopup popUp_Rate);
        public abstract void Show(RatePopup popUp_Rate);

        public virtual void OnRate_ButtonClick()
        {
            _popUp_Rate.Rate();
        }

        public virtual void OnClose_ButtonClick()
        {
            _popUp_Rate.ClosePopup();
        }

    }
}
