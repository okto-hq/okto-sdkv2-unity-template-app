using System;
using System.Collections.Generic;

namespace OktoSDK.Features.SmartContract
{
    [Serializable]
    public class DecodedCallData
    {
        public string FunctionName { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        public string RawCallData { get; set; }
        public string ContractAddress { get; set; }
        public string FromAddress { get; set; }
        public string Value { get; set; }
        public string OrderType { get; set; }
        public string OrderId { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Function: {FunctionName}");
            sb.AppendLine($"Contract: {ContractAddress}");
            sb.AppendLine($"From: {FromAddress}");
            sb.AppendLine($"Value: {Value}");
            sb.AppendLine($"Order Type: {OrderType}");
            sb.AppendLine($"Order ID: {OrderId}");
            sb.AppendLine($"Status: {(IsSuccessful ? "Success" : "Failed")}");
            
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                sb.AppendLine($"Error: {ErrorMessage}");
            }
            
            if (Parameters != null && Parameters.Count > 0)
            {
                sb.AppendLine("Parameters:");
                foreach (var param in Parameters)
                {
                    sb.AppendLine($"  {param.Key}: {param.Value}");
                }
            }
            
            return sb.ToString();
        }
    }
}

namespace OktoSDK.Models.SmartContract
{
    [Serializable]
    public class CallDataDecoderModel
    {
        public string MethodName { get; set; }
        public string DecodedData { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        
        public CallDataDecoderModel()
        {
            Parameters = new Dictionary<string, string>();
        }
        
        [Serializable]
        public class DecodedCallData
        {
            public string Method { get; set; }
            public string Signature { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
            
            public DecodedCallData()
            {
                Parameters = new Dictionary<string, string>();
            }
        }
        
        public bool DecodeCallData(string callData)
        {
            try
            {
                // Implementation would depend on actual decoding logic
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    
    [Serializable]
    public class OrderPrefab
    {
        public string OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public string DecodedData { get; set; }
    }
    
    [Serializable]
    public class AccountPrefab
    {
        public string Address { get; set; }
        public string NetworkId { get; set; }
        public string Balance { get; set; }
    }
    
    [Serializable]
    public class WalletManager
    {
        public List<string> SupportedNetworks { get; set; }
        public string CurrentWalletAddress { get; set; }
        public string CurrentNetworkId { get; set; }
        
        public WalletManager()
        {
            SupportedNetworks = new List<string>();
        }
    }
    
    [Serializable]
    public class PrefabManager
    {
        public WalletManager WalletManager { get; set; }
        public CallDataDecoderModel CallDataDecoder { get; set; }
        
        public PrefabManager()
        {
            WalletManager = new WalletManager();
            CallDataDecoder = new CallDataDecoderModel();
        }
    }
} 