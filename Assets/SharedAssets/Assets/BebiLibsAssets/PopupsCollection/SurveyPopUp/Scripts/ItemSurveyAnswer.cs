using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BebiLibs;
namespace Survey
{
    public class ItemSurveyAnswer : MonoBehaviour
    {
        [SerializeField] private ButtonScale _answerButton;
        [SerializeField] private TextMeshProUGUI _answerDisplayText;
        [SerializeField] private TextMeshProUGUI _buttonDisplayText;
        [SerializeField] private Image _checkMarkRenderer;
        [SerializeField] private Image _answerButtonRenderer;
        public System.Action<bool, string> AnswerCheckedEvent;
        public System.Action<string, string> AnswerCheckDataEvent;
        private bool _isClicked;
        private string _answerID;

        public void AddText(string text)
        {
            _buttonDisplayText.text = text;
            _buttonDisplayText.gameObject.SetActive(true);
        }
        public void SetID(string id)
        {
            _answerID = id;
        }
        public string GetID()
        {
            return _answerID;
        }
        public void SetAnswersContents(Answer answers)
        {
            _answerDisplayText.text = answers.Text;
        }


        public void OnAnswerButtonClick()
        {
            ManagerSounds.PlayEffect("fx_page16");
            UpdateAnswerData();
        }

        public void UpdateAnswerData()
        {
            _isClicked = !_isClicked;
            SetChecked(_isClicked);
            AnswerCheckedEvent?.Invoke(_isClicked, _answerID);
        }


        public void SetIsCLicked(bool value)
        {
            _isClicked = value;
        }
        public void SetChecked(bool active)
        {
            if (active)
            {
                string answerValue = "";
                if (_answerDisplayText != null)
                {
                    answerValue = _answerDisplayText.text;
                }
                else if (_buttonDisplayText != null)
                    answerValue = _buttonDisplayText.text;
                AnswerCheckDataEvent?.Invoke(_answerID, answerValue);
                Color color = Color.green;
                ColorUtility.TryParseHtmlString("#5BC71A", out color);
                _answerButtonRenderer.color = color;
                if (_checkMarkRenderer != null)
                {
                    _checkMarkRenderer.gameObject.SetActive(true);
                }
                else
                {
                    _buttonDisplayText.color = Color.white;
                }
            }
            else
            {
                _answerButtonRenderer.color = Color.white;
                if (_checkMarkRenderer != null)
                {
                    _checkMarkRenderer.gameObject.SetActive(false);
                }
                else
                {
                    Color colorText = Color.gray;
                    ColorUtility.TryParseHtmlString("#3E4A56", out colorText);
                    _buttonDisplayText.color = colorText;
                }
            }
        }
    }
}
