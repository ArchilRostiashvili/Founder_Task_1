using BebiLibs.Localization;
using BebiLibs.RegistrationSystem.PopUp.Components;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace BebiLibs.RegistrationSystem
{
    public class RegistrationSuccessfullPanel : RegistrationPanel
    {
        public UnityAction ExitButtonClickEvent;
        public UnityAction ContinueButtonClickEvent;

        [SerializeField] private ButtonScale _exitButton;
        [SerializeField] private ButtonScale _continueButton;

        [SerializeField] private string _signInSuccessKey = "";
        [SerializeField] private string _signUpSuccessKey = "";

        [SerializeField] private TMP_Text _logText;

        public override void Show(bool activatedFromButton, bool refreshUI, bool isSignIn)
        {
            base.Show(activatedFromButton, refreshUI, isSignIn);
            string key = isSignIn ? _signInSuccessKey : _signUpSuccessKey;
            if (LocalizationManager.TryGetTranslation(key, out string text))
            {
                _logText.text = text;
            }
            else
            {
                _logText.text = "You've successfully signed " + (isSignIn ? "in" : "up");
            }
        }

        private void OnEnable()
        {
            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(ExitButtonClickEvent);

            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(ContinueButtonClickEvent);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveAllListeners();
            _continueButton.onClick.RemoveAllListeners();
        }


        public override void SetLoaderActive(bool value)
        {

        }
    }
}
