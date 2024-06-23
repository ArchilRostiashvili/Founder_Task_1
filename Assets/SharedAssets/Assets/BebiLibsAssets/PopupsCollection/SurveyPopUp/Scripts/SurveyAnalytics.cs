using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BebiLibs.Analytics.GameEventLogger;
using BebiLibs.Analytics;

namespace Survey
{
    public class SurveyAnalytics : AnalyticsHelperBase
    {
        private List<SurveyAnswer> _answersList = new List<SurveyAnswer>();

        public void GatherData(List<SurveyQuestion> answersList)
        {
            for (int i = 0; i < answersList.Count; i++)
            {
                SurveyAnswer answer = new SurveyAnswer();
                answer.Id = answersList[i].Id;
                answer.Type = answersList[i].Type;
                if (answer.Type == "text")
                {
                    answer.Text = answersList[i].Answers.Count > 0 ? answersList[i].Answers[0].Text : string.Empty;
                }
                else
                {
                    for (int j = 0; j < answersList[i].Answers.Count; j++)
                    {
                        answer.Answers.Add(answersList[i].Answers[j].Id);
                    }
                }
                _answersList.Add(answer);
            }
        }

        public List<SurveyAnswer> GetAnswersList()
        {
            return _answersList;
        }

        public void Clear()
        {
            _answersList.Clear();
        }

        public void SendSurveyStartAnalytic(string type, int count, int pageIndex)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(3);
            parameters.Add("type", type);
            parameters.Add("count", count);
            parameters.Add("start_question", pageIndex);
            AnalyticsManager.LogEvent("survey_start", parameters);
        }
        public void SendSurveyInterruptAnalytic(string type, int count, int start_question, int exit_question, int sec)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(5);
            parameters.Add("type", type);
            parameters.Add("count", count);
            parameters.Add("start_question", start_question);
            parameters.Add("exit_question", exit_question);
            parameters.Add("sec", sec);
            AnalyticsManager.LogEvent("survey_exit", parameters);
        }
        public void SendSurveyDoneAnalytic(string type, int count, int start_question, int sec)
        {
            GameParameterBuilder parameters = new GameParameterBuilder(4);
            parameters.Add("type", type);
            parameters.Add("count", count);
            parameters.Add("start_question", start_question);
            parameters.Add("sec", sec);
            AnalyticsManager.LogEvent("survey_done", parameters);
        }
    }
}
