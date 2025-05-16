using System;
using System.Threading.Tasks;
using UnityEngine;
using OktoSDK.UserOp;

namespace OktoSDK
{
    public class EvmRawPrefab : MonoBehaviour
    {
        [SerializeField] private EvmRawManager evmRawManager;
        public EvmRawManager EvmRawManager
        {
            get => evmRawManager;
            set => evmRawManager = value;
        }

        public async void CallEvmRawTransfer(string from, string to, string value)
        {
            try
            {
                var transaction = new Transaction
                {
                    From = from,
                    To = to,
                    Data = "0x",
                    Value = value
                };

                string txHashStr = await evmRawManager.ExecuteEvmRawTransaction(
                    transaction.From,
                    transaction.To,
                    transaction.Value,
                    transaction.Data
                );

                CustomLogger.Log("Transaction Status : " + txHashStr);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }

        public async void CallEvmRawSmartContract(string from, string to, string data)
        {
            try
            {
                var transaction = new Transaction
                {
                    From = from,
                    To = to,
                    Data = data,
                    Value = EvmRawManager.ToHex(1)
                };

                string txHashStr = await evmRawManager.ExecuteEvmRawTransaction(
                    transaction.From,
                    transaction.To,
                    transaction.Value,
                    transaction.Data
                );

                CustomLogger.Log("Transaction Status : " + txHashStr);
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error in TestTokenTransfer: {ex.Message}");
            }
        }

        public void SetFeePayer(string feePayer)
        {
            try
            {
                if (!string.IsNullOrEmpty(feePayer))
                {
                    TransactionConstants.FeePayerAddress = feePayer;
                }
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error setting FeePayer: {ex.Message}");
            }
        }

        // Calls the Contract through EvmRaw
        public async Task<string> CallContract(Transaction transaction)
        {
            try
            {
                if (evmRawManager == null)
                {
                    CustomLogger.LogError("EvmRawManager reference not set");
                    return "Error: EvmRawManager reference not set";
                }

                string txHashStr = await evmRawManager.ExecuteEvmRawTransaction(
                    transaction.From,
                    transaction.To,
                    transaction.Value,
                    transaction.Data
                );

                return txHashStr;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error calling contract: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Creates the Transaction with from, to & data
        public Transaction CreateTransaction(string from, string to, string data, string value = "0")
        {
            return new Transaction
            {
                From = from,
                To = to,
                Data = data,
                Value = value
            };
        }

        public async Task<string> CallEvmRaw(string from, string to, string value, string data)
        {
            //try
            //{
                var transaction = new Transaction
                {
                    From = from,
                    To = to,
                    Data = data,
                    Value = value
                };

                string txHashStr = await evmRawManager.ExecuteEvmRawTransaction(
                    transaction.From,
                    transaction.To,
                    transaction.Value,
                    transaction.Data
                );

                CustomLogger.Log("Transaction Status : " + txHashStr);
                return txHashStr;

            //}
            //catch (Exception ex)
            //{
            //    CustomLogger.LogError($"Error in CallEvmRaw: {ex.Message}");
            //    return ex.Message;
            //}
        }
    }
}