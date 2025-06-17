using System;
using System.Numerics;
using Newtonsoft.Json;

namespace OktoSDK.UserOp
{
    [Serializable]
    public class UserOp
    {
        public string sender { get; set; }
        public string nonce { get; set; }
        public string paymaster { get; set; }
        public string callGasLimit { get; set; } = "0x927c0";                    // 600000
        public string verificationGasLimit { get; set; } = "0x61a80";            // 400000
        public string preVerificationGas { get; set; } = "0x186a0";              // 100000
        public string maxFeePerGas { get; set; } = "0xee6b2800";                 // 4000000000
        public string maxPriorityFeePerGas { get; set; } = "0xee6b2800";         // 4000000000
        public string paymasterPostOpGasLimit { get; set; } = "0x30d40";         // 200000
        public string paymasterVerificationGasLimit { get; set; } = "0x30d40";   // 200000
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

    [Serializable]
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

    [Serializable]
    public class Transaction
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Data { get; set; }
        public string Value { get; set; }
    }

    [Serializable]
    public class ExecuteResult
    {
        public string TransactionHash { get; set; }
        public string Status { get; set; }
    }

    //[Serializable]
    //public class JsonRpcResponse<T>
    //{
    //    public string Jsonrpc { get; set; }
    //    public string Id { get; set; }
    //    public T Result { get; set; }
    //    public RpcError Error { get; set; }
    //}

    [Serializable]
    public class JsonRpcResponse<T>
    {
        public T data { get; set; }
        public OktoSDK.BFF.Error Error { get; set; }
    }

    [Serializable]
    public class RpcError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class RpcErrors
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }
        public RpcErrorType error { get; set; }

        public RpcErrors(string jsonrpc, string id, RpcErrorType error)
        {
            this.jsonrpc = jsonrpc;
            this.id = id;
            this.error = error;
        }
    }

    [Serializable]
    public class RpcErrorType
    {
        public int code { get; set; }
        public string message { get; set; }
    }

    [Serializable]
    public class UserOperationGasPriceResult
    {
        public string maxFeePerGas { get; set; }
        public string maxPriorityFeePerGas { get; set; }
    }

    [Serializable]
    public class TokenTransferIntentParams
    {
        public string recipientWalletAddress { get; set; }
        public string tokenAddress { get; set; }
        public BigInteger amount { get; set; }
        public string caip2Id { get; set; }
    }

    [Serializable]
    public class NFTTransferIntentParams
    {
        public string recipientWalletAddress { get; set; }
        public string collectionAddress { get; set; }
        public string nftId { get; set; }
        public string amount { get; set; }
        public string caip2Id { get; set; }
        public string nftType { get; set; }
    }
} 