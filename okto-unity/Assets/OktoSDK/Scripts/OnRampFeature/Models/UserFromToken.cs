using Newtonsoft.Json;

namespace OktoSDK.OnRamp
{
    public class UserFromToken
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("freezed")]
        public bool Freezed { get; set; }

        [JsonProperty("freeze_reason")]
        public string FreezeReason { get; set; }
    }
}