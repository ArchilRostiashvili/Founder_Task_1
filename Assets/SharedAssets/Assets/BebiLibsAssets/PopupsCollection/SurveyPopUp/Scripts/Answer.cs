using Newtonsoft.Json;
namespace Survey
{
    [System.Serializable]
    public class Answer
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("text")]
        public string Text;
    }

}
