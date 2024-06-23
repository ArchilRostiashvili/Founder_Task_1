using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using UnityEngine.UI;
namespace Survey
{
    public class ItemSurveyCommon : MonoBehaviour
    {
        [SerializeField] private ButtonScale _buttonNext, _buttonSubmit;
        [SerializeField] private ButtonScale _buttonPrevious;
        [SerializeField] private Sprite _buttonInactiveSprite, _buttonActiveSprite;
        [SerializeField] private GameObject _commonContentParentGO;
        [SerializeField] private Image _progressBarRenderer;
        public System.Action<bool> NavigationButtonsClickedEvent;
        public System.Action SubmitButtonClickedEvent;
        private float _progressBarFillAmount;
        public void Init()
        {
            _buttonNext.Init();
            _buttonNext.onClick.RemoveAllListeners();
            _buttonNext.onClick.AddListener(() =>
            {
                ManagerSounds.PlayEffect("fx_page16");
                OnNavigationButtonClicked(true);
            });

            _buttonPrevious.Init();
            _buttonPrevious.onClick.RemoveAllListeners();
            _buttonPrevious.onClick.AddListener(() =>
            {
                ManagerSounds.PlayEffect("fx_page15");
                OnNavigationButtonClicked(false);
            });
            _progressBarRenderer.fillAmount = 0;
            _buttonSubmit.Init();
            _buttonSubmit.onClick.RemoveAllListeners();
            _buttonSubmit.onClick.AddListener(() =>
            {
                ManagerSounds.PlayEffect("fx_page16");
                SubmitButtonClickedEvent?.Invoke();
            });
        }
        public void SetControlButtonEnabled(bool activate)
        {
            _commonContentParentGO.SetActive(activate);
        }

        public void ActivateNextButton(bool activate)
        {
            if (activate)
            {
                _buttonNext.buttonEnabled = true;
                _buttonNext.SetImage(_buttonActiveSprite);
            }
            else
            {
                _buttonNext.buttonEnabled = false;
                _buttonNext.SetImage(_buttonInactiveSprite);
            }
        }
        public void SetProgressBarFillAmount(float amount)
        {
            _progressBarFillAmount = amount;
        }
        public void SetProgressBarValue(float value)
        {
            _progressBarRenderer.fillAmount += value;
        }
        public void OnNavigationButtonClicked(bool navigateNext)
        {
            NavigationButtonsClickedEvent?.Invoke(navigateNext);
            if (navigateNext)
                SetProgressBarValue(_progressBarFillAmount);
            else
                SetProgressBarValue(-_progressBarFillAmount);
        }
        public void CheckForLastPage(bool isLast)
        {
            if (isLast)
            {
                //_buttonPrevious.gameObject.SetActive(false);
                _buttonNext.gameObject.SetActive(false);
                _buttonSubmit.gameObject.SetActive(true);
            }
            else
            {
                _buttonNext.gameObject.SetActive(true);
                _buttonSubmit.gameObject.SetActive(false);
            }
        }
        public void CheckForFirstPage(int index)
        {
            if (index == 0)
                _buttonPrevious.gameObject.SetActive(false);
            else
                _buttonPrevious.gameObject.SetActive(true);
        }

    }
}