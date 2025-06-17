using Newtonsoft.Json;
using System;

namespace OktoSDK
{
    #region Request Models

    [Serializable]
    public class SendEmailOtpRequest
    {
        [JsonProperty("data")]
        public SendEmailOtpData Data { get; set; }

        [JsonProperty("client_signature")]
        public string ClientSign { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "ethsign";

        public SendEmailOtpRequest(SendEmailOtpData data, string signature)
        {
            Data = data;
            ClientSign = signature;
        }
    }

    [Serializable]
    public class SendEmailOtpData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("client_swa")]
        public string ClientSWA { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public SendEmailOtpData(string email, string clientSWA, long timestamp)
        {
            Email = email;
            ClientSWA = clientSWA;
            Timestamp = timestamp;
        }
    }

    [Serializable]
    public class VerifyEmailOtpRequest
    {
        [JsonProperty("data")]
        public VerifyEmailOtpData Data { get; set; }

        [JsonProperty("client_signature")]
        public string ClientSign { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "ethsign";

        public VerifyEmailOtpRequest(VerifyEmailOtpData data, string signature)
        {
            Data = data;
            ClientSign = signature;
        }
    }

    [Serializable]
    public class VerifyEmailOtpData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("client_swa")]
        public string ClientSwa { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public VerifyEmailOtpData(string email, string otp, string token, string swa, long time)
        {
            Email = email;
            Token = token;
            Otp = otp;
            ClientSwa = swa;
            Timestamp = time;
        }
    }

    [Serializable]
    public class ResendEmailOtpRequest
    {
        [JsonProperty("data")]
        public ResendEmailOtpData Data { get; set; }

        [JsonProperty("client_signature")]
        public string ClientSign { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "ethsign";

        public ResendEmailOtpRequest(ResendEmailOtpData data, string signature)
        {
            Data = data;
            ClientSign = signature;
        }
    }

    [Serializable]
    public class ResendEmailOtpData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("client_swa")]
        public string ClientSWA { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public ResendEmailOtpData(string email, string clientSWA, string token, long timestamp)
        {
            Email = email;
            Token = token;
            ClientSWA = clientSWA;
            Timestamp = timestamp;
        }
    }

    #endregion

    #region Response Models

    [Serializable]
    public class EmailApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("trace_id")]
        public string TraceId { get; set; }
    }

    #endregion
}
