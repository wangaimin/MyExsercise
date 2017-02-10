using Newtonsoft.Json;

namespace YZ.JsonRpc.Client
{
    /// <summary>
    /// Represents a JsonRpc request
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonRequest
    {
        public JsonRequest()
        {
        }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public object Params { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }
    }
}
