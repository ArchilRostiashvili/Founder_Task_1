using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace Survey
{
    [CreateAssetMenu(fileName = "SurveyResultParser", menuName = "BebiLibs/Survey/SurveyResultParser", order = 0)]
    public class SurveyResultParser : ScriptableObject
    {
        public const string RADIO = "radio";
        public const string CHECKBOX = "checkbox";
        public const string TEXT = "text";
        public const string RANGE = "range";

        [SerializeField] private TextAsset _surveyResultTextAsset;
        [SerializeField] private TextAsset _surveyQuestions;

        [SerializeField] private List<SurveyAnswer> _answerDataList;
        [SerializeField] private List<SurveyQuestion> _questionsDataList;


        public void Parse()
        {
            GetQuestionData(_surveyQuestions.text);

            string surveyAnswers = _surveyResultTextAsset.text;
            List<string> answerLines = GetAnswerLines(surveyAnswers);
            answerLines = answerLines.Select(x => x = RemoveNonJsonElements(x)).ToList();
            answerLines = answerLines.Select(x => x = RemoveDoubleQuotes(x)).ToList();
            Debug.Log("Answer Count: " + answerLines.Count);
            DataTable table = new DataTable();

            StringBuilder csv = new StringBuilder();
            string header = GetCSVLine(_questionsDataList.Select(x => x.Text).ToArray());

            csv.AppendLine(header);

            foreach (var item in answerLines)
            {
                List<SurveyResult> surveyResults = GetAnswerResults(item);
                var results = surveyResults.Select(x => x.answerList).ToArray();
                string answerLine = GetCSVLine(results);
                Debug.Log(results.Length + " | " + answerLine);
                csv.AppendLine(answerLine);
            }

            //ToCSV(table, "F:/Bebi Academy/Assets/PopupsCollection/SurveyPopUp/Scripts/Editor/Result.csv");
            File.WriteAllText("F:/Bebi Academy/Assets/PopupsCollection/SurveyPopUp/Scripts/Editor/Result.csv", csv.ToString());
        }

        public string GetCSVLine(string[] lineTexts)
        {
            return string.Join(",", lineTexts);
        }


        private List<SurveyResult> GetAnswerResults(string jsonText)
        {
            List<SurveyAnswer> answerData = JsonConvert.DeserializeObject<List<SurveyAnswer>>(jsonText);
            List<SurveyResult> results = new List<SurveyResult>();
            foreach (SurveyAnswer answer in answerData)
            {
                SurveyResult result = null;

                SurveyQuestion question = GetQuestion(answer.Id);

                if (answer.Type == TEXT)
                {
                    result = new SurveyResult(question.Id, question.Text, new List<string>() { answer.Text });
                }
                else if (answer.Type == RADIO)
                {
                    Answer a = question.Answers.Find(x => x.Id == answer.Answers[0]);
                    result = new SurveyResult(question.Id, question.Text, new List<string>() { a.Text });
                }
                else if (answer.Type == CHECKBOX)
                {
                    List<string> a = answer.Answers.Select(x => question.GetAnswer(x)).ToList();
                    result = new SurveyResult(question.Id, question.Text, a);
                }
                else if (answer.Type == RANGE)
                {
                    string a = answer.Answers.Count > 0 ? answer.Answers[0] : "NULL";
                    result = new SurveyResult(question.Id, question.Text, new List<string>() { a });
                }
                results.Add(result);
            }
            return results;
        }

        private void GetQuestionData(string jsonText)
        {
            List<SurveyQuestion> questionsData = JsonConvert.DeserializeObject<List<SurveyQuestion>>(jsonText);
            _questionsDataList = questionsData;
        }

        private SurveyQuestion GetQuestion(string id)
        {
            return _questionsDataList.Find(x => x.Id == id);
        }

        private string RemoveDoubleQuotes(string answerLine)
        {
            string doubleQuotes = "\"\"";
            return answerLine.Replace(doubleQuotes, "\"");
        }

        private string RemoveNonJsonElements(string answerLine)
        {
            int index = answerLine.IndexOf("[");
            answerLine = answerLine.Substring(index);
            answerLine = answerLine.Substring(0, answerLine.Length - 1);
            return answerLine;
        }


        private List<string> GetAnswerLines(string surveyAnswers)
        {
            List<string> answerLine = new List<string>(surveyAnswers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            return answerLine;
        }


        public void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

    }

    public class SurveyResult
    {
        public string Id;
        public string Question;
        public List<string> AnswerList = new List<string>();

        public string answerList => "\"" + string.Join(";", AnswerList) + "\"";

        public SurveyResult(string id, string question, List<string> answer)
        {
            Id = id;
            Question = question;
            AnswerList = answer.Select(x => x.Replace("\n", "").Replace("\r", "")).ToList();
        }

        public override string ToString()
        {
            string text = "";
            text += $"{nameof(Question)}: {Question}\n";
            text += $"{nameof(AnswerList)}:\n";
            foreach (var item in AnswerList)
            {
                text += $"    {item}\n";
            }
            return text;
        }
    }

}
