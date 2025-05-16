using Newtonsoft.Json;
using System.Collections.Generic;

namespace OktoSDK.OnRamp
{
    [System.Serializable]
    public class WebEventModel
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("params", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> @params { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        [JsonProperty("response", NullValueHandling = NullValueHandling.Ignore)]
        public object response { get; set; }

        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string source { get; set; } = "web";

        public WebEventModel CopyWith(
            string newType = null,
            Dictionary<string, object> newRequest = null,
            Dictionary<string, object> newParams = null,
            string newId = null,
            object newResponse = null
        )
        {
            return new WebEventModel
            {
                type = newType ?? this.type,
                @params = newParams ?? this.@params,
                id = newId ?? this.id,
                response = newResponse ?? this.response,
                source = source
            };
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }

        public Dictionary<string, object> AckJson()
        {
            return new Dictionary<string, object>
            {
                { "type", type },
                { "response", response },
                { "source", source },
                { "id", id }
            };
        }
    }

    public static class WebEvent
    {
        public const string Analytics = "analytics";
        public const string Close = "close";
        public const string Url = "url";
        public const string RequestPermission = "requestPermission";
        public const string RequestPermissionAck = "requestPermission_ack";
        public const string Data = "data";
        public const string RequestPermissions = "requestPermissions";
        public const string OnMetaHandler = "onMetaHandler";
    }

    public static class WebKeys
    {
        public const string Key = "key";
        public const string Source = "source";
        public const string RemoteConfig = "remote-config";
        public const string TransactionId = "payToken";
        public const string TokenData = "tokenData";
        public const string ForwardToRoute = "forwardToRoute";
        public const string OrderSuccessBottomSheet = "orderSuccessBottomSheet";
        public const string OrderFailureBottomSheet = "orderFailureBottomSheet";
        public const string OnRampEnabled = "on_ramp_enabled";
        // ... (other keys as needed)
    }
}