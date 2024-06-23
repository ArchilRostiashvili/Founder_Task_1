using BebiLibs.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BebiLibs.RegistrationSystem.PopUp.Components.Sign
{
    public class ManualRegistrationPanel : MonoBehaviour
    {

        public UnityAction<UserCredential> OnNextButtonClickEvent;
        public UnityAction<UserCredential> OnSignInButtonClickEvent;
        public UnityAction<UserCredential> OnForgetPasswordButtonClickEvent;

        [SerializeField] private ManualSignInPostDataSO _manualSignInPostDataSO;
        [SerializeField] private bool _hasPasswordField = false;

        [SerializeField] private TMP_Text _errorText;

        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_InputField _passwordInputField;

        [SerializeField] private ButtonScale _nextButton;
        [SerializeField] private ButtonScale _signInButton;
        [SerializeField] private Button _forgetPasswordButton;

        [Header("Customization:")]
        [SerializeField] private Sprite _defaultInputFieldSprite;
        [SerializeField] private Sprite _errorInputFieldSprite;

        [SerializeField] private ButtonSkinChanger _nextButtonSkinChanger;
        [SerializeField] private ButtonSkinChanger _signInButtonSkinChanger;

        [SerializeField] private string _errorFieldKey = "TEXT_THE_FIELD_IS_REQUIRED";

        private UserCredential _userCredential = new UserCredential("", "", false);

        private void OnEnable()
        {
            // #if UNITY_EDITOR
            //             _emailInputField.text = "t.gakharia@fndr.ge";
            //             _passwordInputField.text = "Mefsia!234";
            // #endif

            _nextButton.onClick.RemoveAllListeners();
            _nextButton.onClick.AddListener(OnNextButtonClick);

            _signInButton.onClick.RemoveAllListeners();
            _signInButton.onClick.AddListener(OnSignInButtonClick);

            _forgetPasswordButton.onClick.RemoveAllListeners();
            _forgetPasswordButton.onClick.AddListener(OnForgetPasswordButtonClick);

            _emailInputField.onValueChanged.RemoveAllListeners();
            _emailInputField.onValueChanged.AddListener(OnEmailInputFieldValueChanged);

            _passwordInputField.onValueChanged.RemoveAllListeners();
            _passwordInputField.onValueChanged.AddListener(OnPasswordInputFieldValueChanged);

            _userCredential = new UserCredential(_emailInputField.text, _passwordInputField.text, _hasPasswordField);
            CheckButtonActiveState();
        }

        public void ResetFields()
        {
            _emailInputField.text = "";
            _passwordInputField.text = "";
        }

        private void OnPasswordInputFieldValueChanged(string value)
        {
            ClearError(true);
            CheckButtonActiveState();
        }

        private void OnEmailInputFieldValueChanged(string value)
        {
            ClearError(true);
            CheckButtonActiveState();
        }

        private void CheckButtonActiveState()
        {
            _userCredential.SetEmailAndPassword(_emailInputField.text, _passwordInputField.text);

            if (_hasPasswordField)
            {
                _nextButtonSkinChanger.SetActiveState(false);
                _signInButtonSkinChanger.SetActiveState(_userCredential.IsValid());
            }
            else
            {
                _signInButtonSkinChanger.SetActiveState(false);
                _nextButtonSkinChanger.SetActiveState(_userCredential.IsEmailValid());
            }
        }

        private void OnForgetPasswordButtonClick()
        {
            OnForgetPasswordButtonClickEvent?.Invoke(_userCredential);
        }

        private void OnSignInButtonClick()
        {
            bool isMailValid = _userCredential.IsEmailValid();
            bool isPasswordValid = _userCredential.IsPasswordValid();

            UpdateFieldImage(isMailValid, _emailInputField);
            UpdateFieldImage(isPasswordValid, _passwordInputField);
            UpdateErrorText(isMailValid && isPasswordValid, _errorText);

            OnSignInButtonClickEvent?.Invoke(_userCredential);
        }

        private void OnNextButtonClick()
        {
            bool isMailValid = _userCredential.IsEmailValid();
            UpdateFieldImage(isMailValid, _emailInputField);
            UpdateErrorText(isMailValid, _errorText);
            OnNextButtonClickEvent?.Invoke(_userCredential);
        }

        private void UpdateFieldImage(bool isValid, TMP_InputField fieldImage)
        {
            fieldImage.image.sprite = isValid ? _defaultInputFieldSprite : _errorInputFieldSprite;
        }

        private void UpdateErrorText(bool isValid, TMP_Text errorText)
        {
            errorText.gameObject.SetActive(!isValid);
            errorText.text = !isValid ? GetFieldErrorString() : "";
        }

        private string GetFieldErrorString()
        {
            return LocalizationManager.TryGetTranslation(_errorFieldKey, out string errorString) ? errorString : "The field is required";
        }

        internal void DisplayError()
        {
            ManualSignInPostDataSO.ErrorsDict ErrorsObject = _manualSignInPostDataSO.Errors;
            ClearError(true);

            _errorText.gameObject.SetActive(true);
            if (!string.IsNullOrEmpty(ErrorsObject.UsernameRelatedError))
            {
                _emailInputField.image.sprite = _errorInputFieldSprite;
                _errorText.text = ErrorsObject.UsernameRelatedError;
            }
            else if (!string.IsNullOrEmpty(ErrorsObject.PasswordRelatedError))
            {
                _passwordInputField.image.sprite = _errorInputFieldSprite;
                _errorText.text = ErrorsObject.UsernameRelatedError;
            }
            else if (!string.IsNullOrEmpty(ErrorsObject.GeneralError))
            {
                _emailInputField.image.sprite = _errorInputFieldSprite;
                _passwordInputField.image.sprite = _errorInputFieldSprite;
                _errorText.text = ErrorsObject.GeneralError;
            }
            else
            {
                Debug.LogWarning("ManualRegistrationPanel: DisplayError: No error to display");
                ClearError(true);
            }
        }

        private void ClearError(bool clearWarning = true)
        {
            if (clearWarning)
            {
                _errorText.gameObject.SetActive(false);
                _errorText.text = "";
            }

            _emailInputField.image.sprite = _defaultInputFieldSprite;
            _passwordInputField.image.sprite = _defaultInputFieldSprite;
        }
    }
}
