using System.Collections.Generic;
using Newtonsoft.Json;
namespace Survey
{
    [System.Serializable]
    public class SurveyQuestion
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("text")]
        public string Text;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("required")]
        public bool Required;
        [JsonProperty("answers")]
        public List<Answer> Answers = new List<Answer>();
        public Range Range;

        public string GetAnswer(string id)
        {
            return Answers.Find(x => x.Id == id).Text;
        }
    }
}
