using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs;
using BebiLibs.RegistrationSystem;
using UnityEngine.Networking;
using BebiLibs.PopupManagementSystem;
using BebiLibs.ServerConfigLoaderSystem;
using BebiLibs.RegistrationSystem.Core;
using BebiLibs.ServerConfigLoaderSystem.Core;

namespace Survey
{
    public class SurveyPopUp : PopUpBase, IBaseRequest
    {
        private static SurveyPopUp _Instance;

        private const string _SURVEY_COMPLETION_NEW = "new";
        private const string _SURVEY_COMPLETION_DONE = "done";
        private const string _SURVEY_COMPLETION_INTERRUPT = "interrupt";

        [SerializeField] private GameUserDataSO _userData;

        [SerializeField] private SurveyUrlParser _surveyUrlParser;
        [SerializeField] private SurveyAnalytics _surveyAnalytics;
        [SerializeField] private ItemSurveyPage _surveyPageInputPrefab;
        [SerializeField] private ItemSurveyPage _surveyPageRadioPrefab;
        [SerializeField] private ItemSurveyPage _surveyPageRangePrefab;
        [SerializeField] private ItemSurveyCommon _surveyPageCommon;
        [SerializeField] private Transform _surveyPagesParentTR;
        [SerializeField] private GameObject _surveyPageStartGO;
        [SerializeField] private GameObject _surveyPageEndGO;

        [SerializeField] private GameObject _loaderGameObject;

        private bool _isFinished, _startedSurvey;
        private List<ItemSurveyPage> _itemSurveyPagesList = new List<ItemSurveyPage>();
        private List<SurveyQuestion> _surveyQuestionsList = new List<SurveyQuestion>();
        private int _pageIndexCounter = 0;
        private string _surveyCompletionType;
        private float _secondsPassedSinceSurvey;
        private int _startedPage;
        private bool _isContentInitialized = false;

        private ResponseData _responseData;
        public ResponseData RequestResponseData
        {
            get => _responseData;
            set => _responseData = value;
        }

        public override void Init()
        {
            _Instance = this;
            base.Init();
        }


        public static void Activate(string configurationUrl = "", string resultSubmitUrl = "")
        {
            if (_Instance == null)
            {
                PopupManager.GetPopup<SurveyPopUp>(popup =>
               {
                   _Instance = popup;
                   ActivatePopup(configurationUrl, resultSubmitUrl);
               });
            }
            else
            {
                ActivatePopup(configurationUrl, resultSubmitUrl);
            }
        }

        private static void ActivatePopup(string configurationUrl, string resultSubmitUrl)
        {
            _Instance.InitializeContent(configurationUrl, resultSubmitUrl);
            if (_Instance.CanShowSurveyPopUp())
            {
                _Instance.TryShowSurvey();
            }
            else
            {
                _Instance.Hide(false);
            }
        }

        private void InitializeContent(string configurationUrl, string resultSubmitUrl)
        {
            if (_isContentInitialized) return;
            _isContentInitialized = true;

            _surveyUrlParser.Init(configurationUrl, resultSubmitUrl);
            _surveyPageCommon.Init();
            _surveyPageCommon.NavigationButtonsClickedEvent += OnNavigationButtonsClickReceived;
            _surveyPageCommon.SubmitButtonClickedEvent += OnSubmitButtonClickReceived;
            _surveyUrlParser.JsonRetrieveEvent += OnJsonRetrievedEvent;
            LoadSurveyExitPageValue();
            LoadSurveyCompleteValue();

        }

        private void LoadSurveyExitPageValue()
        {
            if (!PlayerPrefs.HasKey("SurveyExitPageValue"))
            {
                PlayerPrefs.SetInt("SurveyExitPageValue", 0);
                _startedPage = 0;
            }
            else
            {
                _startedPage = PlayerPrefs.GetInt("SurveyExitPageValue");
            }
        }

        private void LoadSurveyCompleteValue()
        {
            if (!PlayerPrefs.HasKey("SurveyCompletionValue"))
            {
                PlayerPrefs.SetString("SurveyCompletionValue", _SURVEY_COMPLETION_NEW);
                _surveyCompletionType = _SURVEY_COMPLETION_NEW;
            }
            else
            {
                _surveyCompletionType = PlayerPrefs.GetString("SurveyCompletionValue");
            }
        }

        private void LoadAndIncrementSurveyCountValue()
        {
            if (!PlayerPrefs.HasKey("SurveyStartedCountValue"))
            {
                PlayerPrefs.SetInt("SurveyStartedCountValue", 1);
            }
            else
            {
                int count = PlayerPrefs.GetInt("SurveyStartedCountValue");
                count++;
                PlayerPrefs.SetInt("SurveyStartedCountValue", count);
            }
        }

        public bool CanShowSurveyPopUp()
        {
            return _surveyCompletionType != _SURVEY_COMPLETION_DONE;
        }

        public override void Show(bool anim)
        {
            base.Show(anim);
        }

        public void TryShowSurvey()
        {
            transform.gameObject.SetActive(true);
            _surveyUrlParser.TryLoadJson();
            ManagerSounds.PlayEffect("fx_page16");
            _loaderGameObject.SetActive(true);
            LoadAndIncrementSurveyCountValue();
        }

        public void OnJsonRetrievedEvent(bool success, string value)
        {
            if (success && value != null)
            {
                _surveyQuestionsList = _surveyUrlParser.TryParseJson(value);
                SetQuestions();
            }
            else
            {
                Hide(false);
                ManagerSounds.PlayEffect("fx_wrong7");
            }
            _loaderGameObject.SetActive(false);
        }

        private void SetQuestions()
        {
            for (int i = 0; i < _surveyQuestionsList.Count; i++)
            {
                ItemSurveyPage pageRef = GetQuestionType(_surveyQuestionsList[i].Type);
                if (pageRef != null)
                {
                    ItemSurveyPage page = Instantiate(pageRef, _surveyPagesParentTR) as ItemSurveyPage;
                    page.SetQuestionsContent(_surveyQuestionsList[i]);
                    page.Activate(false);
                    page.NextButtonActivationEvent += _surveyPageCommon.ActivateNextButton;
                    _itemSurveyPagesList.Add(page);
                }
            }
            ShowSurvey();
        }

        private void ShowSurvey()
        {
            _surveyPageCommon.SetProgressBarFillAmount(1.0f / (_surveyQuestionsList.Count) * 1f);
            if (_surveyCompletionType == _SURVEY_COMPLETION_NEW)
            {
                _surveyPageStartGO.SetActive(true);
                Show(true);
                _surveyPageCommon.SetControlButtonEnabled(false);
                _surveyAnalytics.SendSurveyStartAnalytic(_surveyCompletionType, PlayerPrefs.GetInt("SurveyStartedCountValue"), 0);
            }
            else if (_surveyCompletionType == _SURVEY_COMPLETION_INTERRUPT)
            {
                _surveyPageStartGO.SetActive(false);
                _startedSurvey = true;
                RetrieveAnsweredQuestions();
                Show(true);
                _surveyAnalytics.SendSurveyStartAnalytic(_surveyCompletionType, PlayerPrefs.GetInt("SurveyStartedCountValue"), 1);
                _surveyPageCommon.SetProgressBarValue(1.0f * (PlayerPrefs.GetInt("SurveyExitPageValue") + 1) / (_surveyQuestionsList.Count) * 1f);
            }
        }

        private void Update()
        {
            if (!_startedSurvey)
            {
                return;
            }
            _secondsPassedSinceSurvey += Time.deltaTime;
        }

        private void RetrieveAnsweredQuestions()
        {
            _startedPage = PlayerPrefs.GetInt("SurveyExitPageValue");
            List<SurveyQuestion> surveyQuestionsList = new List<SurveyQuestion>();
            if (PlayerPrefs.HasKey("SurveyAnsweredValues"))
                surveyQuestionsList = _surveyUrlParser.TryParseJson(PlayerPrefs.GetString("SurveyAnsweredValues"));
            for (int i = 0; i < _itemSurveyPagesList.Count; i++)
            {
                if (_itemSurveyPagesList[i].GetID() == surveyQuestionsList[i].Id)
                {
                    if (surveyQuestionsList[i].Answers.Count != 0)
                    {
                        _itemSurveyPagesList[i].SetRetrievedAnswer(surveyQuestionsList[i].Answers);
                    }
                }
            }
            _pageIndexCounter = _startedPage;
            SurveyPagesController(_startedPage);
        }

        private ItemSurveyPage GetQuestionType(string type)
        {
            switch (type)
            {
                case "radio":
                    return _surveyPageRadioPrefab;
                case "checkbox":
                    return _surveyPageRadioPrefab;
                case "text":
                    return _surveyPageInputPrefab;
                case "range":
                    return _surveyPageRangePrefab;
                default:
                    Debug.LogError("this type of question not supported");
                    return null;
            }
        }

        public void OnStartSurvey_ButtonClicked()
        {
            _startedSurvey = true;
            SurveyPagesController(0);
            _surveyPageStartGO.SetActive(false);
            _surveyPageCommon.SetControlButtonEnabled(true);
            _surveyPageCommon.SetProgressBarValue(1.0f / (_surveyQuestionsList.Count) * 1f);
        }

        public void OnSubmitButtonClickReceived()
        {
            string surveyType = "";
            if (PlayerPrefs.GetInt("SurveyStartedCountValue") == 1)
            {
                surveyType = "new";
            }
            else
            {
                surveyType = "continue";
            }
            _isFinished = true;
            _itemSurveyPagesList[_itemSurveyPagesList.Count - 1].Activate(false);
            _surveyPageEndGO.SetActive(true);
            _surveyPageCommon.SetControlButtonEnabled(false);
            _surveyAnalytics.GatherData(GatherResponses());

            PlayerPrefs.SetString("SurveyCompletionValue", _SURVEY_COMPLETION_DONE);
            _surveyAnalytics.SendSurveyDoneAnalytic(surveyType, PlayerPrefs.GetInt("SurveyStartedCountValue"), _startedPage, (int)_secondsPassedSinceSurvey);

            string answersJsonText = _surveyUrlParser.SerializeAnswersIntoJsonString(_surveyAnalytics.GetAnswersList());
            SendAnswersDataToServer(answersJsonText);
        }

        public void SendAnswersDataToServer(string dataToSend)
        {
            StartCoroutine(SendData());

            IEnumerator SendData()
            {
                yield return BaseRequestHandler.PostDataToUrl(_surveyUrlParser.UrlToSend, dataToSend, this);

                if (_responseData.RequestStatus == RequestStatus.Success)
                {
                    Debug.LogWarning("Survey data sent currently");
                }
                else
                {
                    Debug.LogError($"FAILED to send Survey Data {_responseData.ResponseCode} | {_responseData.ResponseString}");
                }
            }
        }

        public void ModifyRequestHeader(UnityWebRequest unityWebRequest)
        {
            BaseRequestHandler.SetBearerToken(unityWebRequest, _userData);
        }

        public void OnNavigationButtonsClickReceived(bool navigateNext)
        {
            if (navigateNext)
                _pageIndexCounter++;
            else
                _pageIndexCounter--;
            SurveyPagesController(_pageIndexCounter);
        }

        public void SurveyPagesController(int pageIndex)
        {
            _surveyPageCommon.CheckForFirstPage(pageIndex);
            _surveyPageCommon.CheckForLastPage(pageIndex == _itemSurveyPagesList.Count - 1);
            for (int i = 0; i < _itemSurveyPagesList.Count; i++)
            {
                if (i == pageIndex)
                {
                    _itemSurveyPagesList[i].Activate(true);
                    _surveyPageCommon.ActivateNextButton(_itemSurveyPagesList[i].IsAnyAnswerChecked());
                }
                else
                {
                    _itemSurveyPagesList[i].Activate(false);
                }
            }
        }
        private List<SurveyQuestion> GatherResponses()
        {
            List<SurveyQuestion> answersList = new List<SurveyQuestion>();
            for (int i = 0; i < _itemSurveyPagesList.Count; i++)
            {
                answersList.Add(_itemSurveyPagesList[i].GetAnswer());
            }
            return answersList;
        }

        public override void Trigger_ButtonClick_Close()
        {
            ManagerSounds.PlayEffect("fx_page17");
            string surveyType = "";
            if (PlayerPrefs.GetInt("SurveyStartedCountValue") == 1)
            {
                surveyType = "new";

            }
            else
            {
                surveyType = "continue";
            }

            base.Trigger_ButtonClick_Close();
            if (_startedSurvey && !_isFinished)
            {
                string answerValues = _surveyUrlParser.SerializeAnswersIntoJsonString(GatherResponses());
                PlayerPrefs.SetString("SurveyAnsweredValues", answerValues);
                PlayerPrefs.SetString("SurveyCompletionValue", _SURVEY_COMPLETION_INTERRUPT);
                PlayerPrefs.SetInt("SurveyExitPageValue", _pageIndexCounter);
                _surveyAnalytics.SendSurveyInterruptAnalytic(surveyType,
                PlayerPrefs.GetInt("SurveyStartedCountValue"), _startedPage,
                PlayerPrefs.GetInt("SurveyExitPageValue"), (int)_secondsPassedSinceSurvey);
            }
            else if (_startedSurvey && _isFinished)
            {
                PlayerPrefs.SetString("SurveyCompletionValue", _SURVEY_COMPLETION_DONE);
            }
            else
            {
                PlayerPrefs.SetString("SurveyCompletionValue", _SURVEY_COMPLETION_NEW);
            }
            PlayerPrefs.Save();

            _isContentInitialized = false;

            foreach (var item in _itemSurveyPagesList)
            {
                if (item != null)
                {
                    GameObject.Destroy(item.gameObject);
                }
            }

            _itemSurveyPagesList?.Clear();
            _surveyQuestionsList?.Clear();

            _surveyPageCommon.NavigationButtonsClickedEvent -= OnNavigationButtonsClickReceived;
            _surveyPageCommon.SubmitButtonClickedEvent -= OnSubmitButtonClickReceived;
            _surveyUrlParser.JsonRetrieveEvent -= OnJsonRetrievedEvent;
            _surveyAnalytics.Clear();

            _startedSurvey = false;
            _isFinished = false;
        }


    }
}
