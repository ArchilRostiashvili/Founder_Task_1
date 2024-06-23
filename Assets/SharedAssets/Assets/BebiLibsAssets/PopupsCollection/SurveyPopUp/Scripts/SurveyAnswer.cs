using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
namespace Survey
{
    [System.Serializable]
    public class SurveyAnswer
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("answers")]
        public List<string> Answers = new List<string>();
        [JsonProperty("text")]
        public string Text;

        public override string ToString()
        {
            string text = "";
            text += $"{nameof(Id)}: {Id}\n";
            text += $"{nameof(Type)}: {Type}\n";
            text += $"{nameof(Answers)}:\n";
            foreach (var item in Answers)
            {
                text += $"    {item}\n";
            }
            text += $"{nameof(Text)}: {Text}\n";
            return text;
        }
    }
}

