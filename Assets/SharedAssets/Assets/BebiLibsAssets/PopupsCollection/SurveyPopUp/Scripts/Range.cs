using Newtonsoft.Json;
namespace Survey
{
    [System.Serializable]
    public class Range
    {
        [JsonProperty("from")]
        public string From;
        [JsonProperty("to")]
        public string To;
    }

}
