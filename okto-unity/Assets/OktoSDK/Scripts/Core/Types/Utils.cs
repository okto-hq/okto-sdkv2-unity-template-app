using Newtonsoft.Json;
using System.Numerics;

namespace OktoSDK
{
    public class Transaction
    {
        public string from { get; set; }
        public string to { get; set; }
        public string data { get; set; }
        public string value { get; set; }
    }

    public class TokenTransferIntentParams
    {
        public string caip2Id { get; set; }
        public string recipientWalletAddress { get; set; }
        public string tokenAddress { get; set; }
        public BigInteger amount { get; set; }
    }

    public class NFTTransferIntentParams
    {
        public string caip2Id { get; set; }
        public string collectionAddress { get; set; }
        public string nftId { get; set; }
        public string recipientWalletAddress { get; set; }
        public BigInteger amount { get; set; }
        public string nftType { get; set; }
    }

    public class UserOp
    {
        public string sender { get; set; }
        public string nonce { get; set; }
        public string paymaster { get; set; }
        public string callGasLimit { get; set; } = "0x493e0";
        public string verificationGasLimit { get; set; } = "0x30d40";
        public string preVerificationGas { get; set; } = "0xc350";
        public string maxFeePerGas { get; set; } = "0x77359400";
        public string maxPriorityFeePerGas { get; set; } = "0x77359400";
        public string paymasterPostOpGasLimit { get; set; } = "0x186a0";
        public string paymasterVerificationGasLimit { get; set; } = "0x186a0";
        public string callData { get; set; }
        public string paymasterData { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string signature { get; set; }

        public UserOp Clone()
        {
            return new UserOp
            {
                sender = this.sender,
                nonce = this.nonce,
                paymaster = this.paymaster,
                callGasLimit = this.callGasLimit,
                verificationGasLimit = this.verificationGasLimit,
                preVerificationGas = this.preVerificationGas,
                maxFeePerGas = this.maxFeePerGas,
                maxPriorityFeePerGas = this.maxPriorityFeePerGas,
                paymasterPostOpGasLimit = this.paymasterPostOpGasLimit,
                paymasterVerificationGasLimit = this.paymasterVerificationGasLimit,
                callData = this.callData,
                paymasterData = this.paymasterData
                // Deliberately not copying signature
            };
        }
    }

    public class PackedUserOp
    {
        public string sender { get; set; }
        public string nonce { get; set; }
        public string initCode { get; set; } = "0x";
        public string callData { get; set; }
        public string preVerificationGas { get; set; }
        public string accountGasLimits { get; set; }
        public string gasFees { get; set; }
        public string paymasterAndData { get; set; }
    }

    public class JsonRpcRequest
    {
        public string method { get; set; }
        public string jsonrpc { get; set; }
        public string id { get; set; }
        public object[] @params { get; set; }
    }


    // Add these classes for JSON response handling
    public class JsonRpcResponse<T>
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }
        public T result { get; set; }
        public JsonRpcError error { get; set; }
    }

    public class JsonRpcError
    {
        public int code { get; set; }
        public string message { get; set; }
        public string data { get; set; }
    }

    public class ExecuteResult
    {
        public string jobId { get; set; }
    }

    public class RawTransactionIntentParams
    {
        public string from { get; set; }
        public string to { get; set; }
        public string data { get; set; } = "0x";
        public string value { get; set; }
        public string chainId { get; set; }
    }

}
