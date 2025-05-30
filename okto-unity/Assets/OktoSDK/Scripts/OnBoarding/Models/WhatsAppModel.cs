using Newtonsoft.Json;
using System;

namespace OktoSDK
{
    [Serializable]
    public class OTPBffApiResponse
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("data")] public OtpApiResponse Data { get; set; }
        [JsonProperty("error")] public Error Error { get; set; }
    }

    [Serializable]
    public class OtpApiResponse
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("code")] public int Code { get; set; }
        [JsonProperty("token")] public string Token { get; set; }
        [JsonProperty("trace_id")] public string TraceId { get; set; }
    }

    [Serializable]
    public class Error
    {
        [JsonProperty("code")] public int Code { get; set; }
        [JsonProperty("errorCode")] public string ErrorCode { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("trace_id")] public string TraceId { get; set; }
        [JsonProperty("details")] public string Details { get; set; }
    }


    [Serializable]
    public class SendOtpRequest
    {
        [JsonProperty("data")]
        public SendOtpData Data { get; set; }

        [JsonProperty("client_signature")]
        public string ClientSignature { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "ethsign";

        public SendOtpRequest(string phone, string country, string swa, long time, string signature)
        {
            Data = new SendOtpData(phone, country, swa, time);
            ClientSignature = signature;
        }
    }

    [Serializable]
    public class SendOtpData
    {
        [JsonProperty("whatsapp_number")]
        public string WhatsappNumber { get; set; }

        [JsonProperty("country_short_name")]
        public string CountryShortName { get; set; }

        [JsonProperty("client_swa")]
        public string ClientSwa { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public SendOtpData(string phone, string country, string swa, long time)
        {
            WhatsappNumber = phone;
            CountryShortName = country;
            ClientSwa = swa;
            Timestamp = time;
        }
    }

    [Serializable]
    public class VerifyOtpRequest
    {
        [JsonProperty("data")]
        public VerifyOtpData Data { get; set; }

        [JsonProperty("client_signature")]
        public string ClientSignature { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "ethsign";

        public VerifyOtpRequest(string phone, string country, string otp, string token, string swa, long time, string signature)
        {
            Data = new VerifyOtpData(phone, country, otp, token, swa, time);
            ClientSignature = signature;
        }
    }

    [Serializable]
    public class VerifyOtpData
    {
        [JsonProperty("whatsapp_number")]
        public string WhatsappNumber { get; set; }

        [JsonProperty("country_short_name")]
        public string CountryShortName { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("client_swa")]
        public string ClientSwa { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public VerifyOtpData(string phone, string country, string otp, string token, string swa, long time)
        {
            WhatsappNumber = phone;
            CountryShortName = country;
            Token = token;
            Otp = otp;
            ClientSwa = swa;
            Timestamp = time;
        }
    }

    [Serializable]
    public class ResendOtpRequest
    {
        [JsonProperty("data")]
        public ResendOtpData Data { get; set; }

        [JsonProperty("client_signature")]
        public string ClientSignature { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "ethsign";

        public ResendOtpRequest(string phone, string country, string token, string swa, long time, string signature)
        {
            Data = new ResendOtpData(phone, country, token, swa, time);
            ClientSignature = signature;
        }
    }

    [Serializable]
    public class ResendOtpData
    {
        [JsonProperty("whatsapp_number")]
        public string WhatsappNumber { get; set; }

        [JsonProperty("country_short_name")]
        public string CountryShortName { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("client_swa")]
        public string ClientSwa { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public ResendOtpData(string phone, string country, string token, string swa, long time)
        {
            WhatsappNumber = phone;
            CountryShortName = country;
            Token = token;
            ClientSwa = swa;
            Timestamp = time;
        }
    }

    [Serializable]
    public class AuthResponseBff
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public AuthResponse Data { get; set; }
    }

    [Serializable]
    public class AuthResponse
    {
        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("refresh_auth_token")]
        public string RefreshAuthToken { get; set; }

        [JsonProperty("device_token")]
        public string DeviceToken { get; set; }

        [JsonProperty("trace_id")]
        public string TraceId { get; set; }
    }
}