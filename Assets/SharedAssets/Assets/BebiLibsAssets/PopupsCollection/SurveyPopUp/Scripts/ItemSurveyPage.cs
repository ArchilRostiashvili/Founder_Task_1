using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
namespace Survey
{
    public class ItemSurveyPage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _headerDisplayText;
        [SerializeField] private ItemSurveyAnswer _itemSurveyAnswerPrefab;
        [SerializeField] private TMP_InputField _textInput;
        [SerializeField] private Transform _answersParentTR;
        [SerializeField] private List<ItemSurveyAnswer> _surveyAnswersList;
        [SerializeField] private FlowLayoutGroup _gridLayoutGroup;
        public System.Action<bool> NextButtonActivationEvent;
        private SurveyQuestion _answerRoot = new SurveyQuestion();
        private bool _isRequired;
        private bool _isSingleChoice = false;
        private int _answerCheckedCount;
        private string _questionID, _questionType;

        private void Start()
        {
            if (_textInput != null)
            {
                _textInput.lineType = TMP_InputField.LineType.MultiLineNewline;
                _textInput.onEndEdit.AddListener(OnInputValueChanged);
                Answer answer = new Answer();
                answer.Id = "1";
                answer.Text = "";
                _answerRoot.Answers.Add(answer);
            }
            if (_gridLayoutGroup != null)
            {
                // Debug.Log(_answersParentTR.GetComponent<RectTransform>());
                // Debug.Log(_gridLayoutGroup.GetComponent<RectTransform>());
                // Debug.Log(_gridLayoutGroup.gameObject.GetComponent<RectTransform>());
                if (Camera.main.aspect < 1.7f)
                {
                    _gridLayoutGroup.padding.left = 380;
                    _gridLayoutGroup.padding.top = -60;
                    if (_gridLayoutGroup.gameObject.GetComponent<RectTransform>() != null)
                    {
                        _gridLayoutGroup.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
                        _gridLayoutGroup.GetComponent<RectTransform>().offsetMax = new Vector2(-400, 0);
                    }
                }
            }
        }

        public void PrepareAnswers(int count)
        {
            if (_surveyAnswersList.Count < count)
            {
                while (_surveyAnswersList.Count != count)
                {
                    ItemSurveyAnswer answer = Instantiate(_itemSurveyAnswerPrefab, _answersParentTR) as ItemSurveyAnswer;
                    _surveyAnswersList.Add(answer);
                }
            }
            else if (_surveyAnswersList.Count > count && _surveyAnswersList.Count > 0)
            {
                while (_surveyAnswersList.Count != count)
                {
                    ItemSurveyAnswer answer = _surveyAnswersList[_surveyAnswersList.Count - 1];
                    _surveyAnswersList.RemoveAt(_surveyAnswersList.Count - 1);
                    Destroy(answer.gameObject);
                }
            }
        }

        public bool IsAnyAnswerChecked()
        {
            if (_answerCheckedCount > 0 || !_isRequired)
                return true;
            return false;
        }
        public SurveyQuestion GetAnswer()
        {
            return _answerRoot;
        }
        public void Activate(bool activate)
        {
            gameObject.SetActive(activate);
        }
        public void OnInputValueChanged(string value)
        {
            _answerRoot.Answers[0].Text = value;
        }
        public string GetID()
        {
            return _questionID;
        }
        public void SetQuestionsContent(SurveyQuestion questionContent)
        {
            _answerRoot.Id = questionContent.Id;
            _answerRoot.Text = questionContent.Text;
            _answerRoot.Type = questionContent.Type;
            _questionID = questionContent.Id;
            _questionType = questionContent.Type;
            if (questionContent.Type == "radio"
            || questionContent.Type == "range")
            {
                _isSingleChoice = true;
            }
            _headerDisplayText.text = questionContent.Text;
            _isRequired = questionContent.Required;
            if (questionContent.Type == "range")
            {
                int range = Mathf.Abs(System.Int32.Parse(questionContent.Range.To) - System.Int32.Parse(questionContent.Range.From));
                PrepareAnswers(range);
                for (int i = 0; i < range; i++)
                {
                    _surveyAnswersList[i].SetID((i + 1).ToString());
                    _surveyAnswersList[i].AddText((i + 1).ToString());
                    _surveyAnswersList[i].AnswerCheckDataEvent += OnAnswerCheckDataReceived;
                    _surveyAnswersList[i].AnswerCheckedEvent += OnAnswerCheckReceivedEvent;
                }
            }
            else if (questionContent.Type == "radio" || questionContent.Type == "checkbox")
            {
                PrepareAnswers(questionContent.Answers.Count);
                for (int i = 0; i < questionContent.Answers.Count; i++)
                {
                    _surveyAnswersList[i].SetID(questionContent.Answers[i].Id);
                    _surveyAnswersList[i].SetAnswersContents(questionContent.Answers[i]);
                    _surveyAnswersList[i].AnswerCheckDataEvent += OnAnswerCheckDataReceived;
                    _surveyAnswersList[i].AnswerCheckedEvent += OnAnswerCheckReceivedEvent;
                }
            }
            else if (questionContent.Type == "input")
            {
                _textInput.lineType = TMP_InputField.LineType.MultiLineNewline;
            }
        }
        public void OnAnswerCheckDataReceived(string id, string answerText)
        {
            Answer answer = new Answer();
            answer.Id = id;
            answer.Text = answerText;
            _answerRoot.Answers.Add(answer);
        }

        public void RemoveAnswer(string id)
        {
            for (int i = 0; i < _answerRoot.Answers.Count; i++)
            {
                if (_answerRoot.Answers[i].Id == id)
                {
                    _answerRoot.Answers.RemoveAt(i);
                }
            }
        }
        public void OnAnswerCheckReceivedEvent(bool isChecked, string id)
        {
            if (_isSingleChoice)
            {
                SingleChoiceCheckButton(id);
                if (isChecked)
                {
                    _answerCheckedCount = 1;
                }
                else
                {
                    _answerCheckedCount = 0;
                    RemoveAnswer(id);
                }

                if (_isRequired)
                {
                    NextButtonActivationEvent?.Invoke(isChecked);
                }
            }
            else
            {
                if (!_isRequired)
                    return;

                if (isChecked)
                {
                    _answerCheckedCount += 1;
                }
                else
                {
                    _answerCheckedCount -= 1;
                    RemoveAnswer(id);
                }

                if (_answerCheckedCount > 0)
                    NextButtonActivationEvent?.Invoke(true);
                else
                    NextButtonActivationEvent?.Invoke(false);
            }
        }

        public void SetRetrievedAnswer(List<Answer> answersList)
        {
            if (_textInput != null && answersList[0].Text != "")
            {
                //_textInput.placeholder.GetComponent<TextMeshPro>().text = "";
                _textInput.text = answersList[0].Text;
            }
            else //if (_questionType == "radio" || _questionType == "checkbox")
            {
                for (int i = 0; i < _surveyAnswersList.Count; i++)
                {
                    for (int j = 0; j < answersList.Count; j++)
                    {
                        if (_surveyAnswersList[i].GetID() == answersList[j].Id)
                        {
                            _surveyAnswersList[i].UpdateAnswerData();
                            break;
                        }
                    }
                }
            }

        }
        public bool GetIsRequired()
        {
            return _isRequired;
        }

        private void SingleChoiceCheckButton(string id)
        {
            for (int i = 0; i < _surveyAnswersList.Count; i++)
            {
                if (_surveyAnswersList[i].GetID() != id)
                {
                    _surveyAnswersList[i].SetChecked(false);
                    _surveyAnswersList[i].SetIsCLicked(false);
                    RemoveAnswer(_surveyAnswersList[i].GetID());
                }
            }
        }
    }
}
