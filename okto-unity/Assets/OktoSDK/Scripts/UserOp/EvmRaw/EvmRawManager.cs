using System;
using System.Numerics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using OktoSDK.Features.Transaction;
using UserOpType = OktoSDK.UserOp.UserOp;
using ExecuteResultType = OktoSDK.BFF.ExecuteResult;
using OktoSDK.Auth;
using OktoSDK.BFF;

namespace OktoSDK
{
    public class EvmRawManager : MonoBehaviour
    {
        public EVMRawController evmRawController;

        private string network;

        public void SetNetwork(string network)
        {
            this.network = network;
        }

        public string GetNetwork()
        {
            return this.network;
        }

        public async Task<UserOpType> CreateUserOp(OktoSDK.UserOp.Transaction transaction)
        {
            var nonce = Guid.NewGuid();
            string guidString = nonce.ToString("N");

            var userOp = await evmRawController.CreateUserOp(
                userSWA: OktoAuthManager.GetOktoClient()._sessionConfig.UserSWA,
                clientSWA: OktoAuthManager.GetOktoClient().ClientSWA,
                nonce: "0x" + new string('0', 24) + guidString,
                paymasterData: await PaymasterDataGenerator.Generate(
                    clientSWA: OktoAuthManager.GetOktoClient().ClientSWA,
                    clientPrivateKey: OktoAuthManager.GetOktoClientConfig().ClientPrivateKey,
                    nonce: nonce.ToString(),
                    validUntil: DateTime.UtcNow.AddHours(6)
                ),
                transaction: transaction
            );

            string userOpJson = JsonConvert.SerializeObject(userOp, Formatting.Indented);

            return userOp;
        }

        public UserOpType SignUserOp(UserOpType userOp)
        {
            var userOpForSigning = userOp.Clone();

            var signedUserOp = UserOpSign.SignUserOp(
                userOp: userOpForSigning,
                privateKey: OktoAuthManager.GetSession().SessionPrivKey,
                entryPointAddress: OktoAuthManager.GetOktoClient().Env.EntryPointContractAdress,
                chainId: OktoAuthManager.GetOktoClient().Env.ChainId
            );

            userOp.signature = signedUserOp.signature;

            string userOpJson = JsonConvert.SerializeObject(userOp, Formatting.Indented);

            return signedUserOp;
        }

        public async Task<JsonRpcResponse<ExecuteResult>> ExecuteUserOp(UserOpType signedUserOp, string signature)
        {
            JsonRpcResponse<ExecuteResultType> txHash = await UserOpExecute.ExecuteUserOp(signedUserOp, signature);
            return txHash;
        }

        public static string ToHex(object value)
        {
            if (value is int intValue)
            {
                return $"0x{intValue:X}";
            }
            else if (value is BigInteger bigIntValue)
            {
                return $"0x{bigIntValue.ToString("X")}";
            }
            else if (value is string strValue)
            {
                if (BigInteger.TryParse(strValue, out BigInteger bigIntParsed))
                {
                    return $"0x{bigIntParsed.ToString("X")}";
                }
                else
                {
                    throw new ArgumentException($"Invalid string input: {strValue}. It must be a valid number.");
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported type: {value.GetType()}. Use int, BigInteger, or a valid numeric string.");
            }
        }

        public async Task<string> ExecuteEvmRawTransaction(string sender, string receipent, string value, string data)
        {
            string hexValue;

            try
            {
                hexValue = string.IsNullOrEmpty(value) ? "0x" : ToHex(value);
            }
            catch
            {
                return "Invalid Amount!";
            }

            var transaction = evmRawController.CreateTransaction(
                from: sender,
                to: receipent,
                data: string.IsNullOrEmpty(data) ? "0x" : data,
                value: hexValue
            );

            var userOp = await CreateUserOp(transaction);


            var signedUserOp = SignUserOp(userOp);

            BFF.JsonRpcResponse<ExecuteResult> txHash = await ExecuteUserOp(signedUserOp, signedUserOp.signature);
            string txHashStr = JsonConvert.SerializeObject(txHash, Formatting.Indented);


            return txHashStr;
        }

        // Optional test helper method
        public async void TestTokenTransfer()
        {
            try
            {
                var transaction = new OktoSDK.UserOp.Transaction
                {
                    From = "0xc3AC3F050CCa482CF6F53070541A7B61A71C4abd",
                    To = "0xEE54970770DFC6cA138D12e0D9Ccc7D20b899089",
                    Data = "0x",
                    Value = ToHex(1)
                };

                string txHashStr = await ExecuteEvmRawTransaction(
                    transaction.From,
                    transaction.To,
                    transaction.Value,
                    transaction.Data
                );

                ResponsePanel.SetResponse(txHashStr);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }
    }
}
