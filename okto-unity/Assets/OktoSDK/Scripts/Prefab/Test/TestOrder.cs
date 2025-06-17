using OktoSDK.BFF;
using OktoSDK.Features.Order;
using OktoSDK.Models.Order;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace OktoSDK
{
    public class TestOrder : MonoBehaviour
    {
        [SerializeField]
        private string intentId;

        [SerializeField]
        private float checkIntervalInSeconds = 10f;

        public OrderPrefab orderPrefab;

        private Coroutine pollingCoroutine;

        public void CheckOrderStatus()
        {
            StartCheckingOrderStatus();
        }

        public void StartCheckingOrderStatus()
        {
            if (pollingCoroutine == null)
            {
                pollingCoroutine = StartCoroutine(CheckOrderStatusUntilSuccess());
            }
        }

        public void StopCheckingOrderStatus()
        {
            if (pollingCoroutine != null)
            {
                StopCoroutine(pollingCoroutine);
                pollingCoroutine = null;
            }
        }

        private IEnumerator CheckOrderStatusUntilSuccess()
        {
            while (true)
            {
                // This is where we await the async method, but within the coroutine.
                var task = CheckOrderStatusCoroutine();
                yield return new WaitUntil(() => task.IsCompleted);

                if (task.IsCompletedSuccessfully)
                {
                    var (order, downstreamHash) = task.Result;

                    if (order.Status != "SUCCESS" || order.Status != "FAILURE")
                    {
                        // Handle the details based on the intent type
                        if (order.Details is RawTransactionDetails raw)
                        {
                            HandleRawTransaction(raw, downstreamHash);
                            break; // Stop checking when the status is successful
                        }
                        else if (order.Details is TokenTransferDetails token)
                        {
                            HandleTokenTransfer(token, downstreamHash);
                            break; // Stop checking when the status is successful
                        }
                        else if (order.Details is NftTransferDetails nft)
                        {
                            HandleNftTransfer(nft, downstreamHash);
                            break; // Stop checking when the status is successful
                        }
                        else
                        {
                            CustomLogger.Log("Status not successful yet, retrying...");
                        }
                    }
                }

                // Wait before checking again
                yield return new WaitForSeconds(checkIntervalInSeconds);
            }
        }

        private async Task<(Order details, string downstreamHash)> CheckOrderStatusCoroutine()
        {
            // Await the async operation to get the details
            var (details, downstreamHash) = await orderPrefab.GetOrderDetailsByIntentId(intentId);
            return (details, downstreamHash); // Return the result for further processing
        }

        public async void TestOrderApi()
        {
            if (orderPrefab == null)
            {
                CustomLogger.LogError("OrderPrefab reference is not set in TestOrder!");
                return;
            }

            var (order, downstreamHash) = await orderPrefab.GetOrderDetailsByIntentId(intentId);

            if (order.Details is RawTransactionDetails raw)
            {
                HandleRawTransaction(raw, downstreamHash);
            }
            else if (order.Details is TokenTransferDetails token)
            {
                HandleTokenTransfer(token, downstreamHash);
            }
            else if (order.Details is NftTransferDetails nft)
            {
                HandleNftTransfer(nft, downstreamHash);
            }
        }

        private void HandleRawTransaction(RawTransactionDetails raw, string downstreamHash)
        {
            CustomLogger.Log($"[RAW] CAIP2: {raw.Caip2Id}, Transactions Count: {raw.Transactions?.Count}");
            CustomLogger.Log($"Downstream Transaction Hash: {downstreamHash}"); // Print downstream hash

            for (int i = 0; i < raw.Transactions?.Count; i++)
            {
                var transactionList = raw.Transactions[i];

                // Iterate through each RawTransactionItem in the transaction list
                for (int j = 0; j < transactionList.Count; j++)
                {
                    var item = transactionList[j]; // This is a RawTransactionItem

                    CustomLogger.Log($"Transaction {i + 1}, Item {j + 1}: Key: {item.Key}, Value: {item.Value}");
                }
            }
        }


        private void HandleTokenTransfer(TokenTransferDetails token, string downstreamHash)
        {
            CustomLogger.Log("[TOKEN]");
            CustomLogger.Log($"Amount: {token.Amount}");
            CustomLogger.Log($"CAIP2 ID: {token.Caip2IdAlt}");
            CustomLogger.Log($"Recipient: {token.RecipientWalletAddress}");
            CustomLogger.Log($"Token Address: {token.TokenAddress}");
            CustomLogger.Log($"Downstream Transaction Hash: {downstreamHash}"); // Print downstream hash
        }

        private void HandleNftTransfer(NftTransferDetails nft, string downstreamHash)
        {
            CustomLogger.Log("[NFT]");
            CustomLogger.Log($"Collection Address: {nft.CollectionAddress}");
            CustomLogger.Log($"NFT ID: {nft.NftId}");
            CustomLogger.Log($"Recipient Wallet: {nft.RecipientWalletAddress}");
            CustomLogger.Log($"Amount: {nft.Amount}");
            CustomLogger.Log($"NFT Type: {nft.NftType}");
            CustomLogger.Log($"Downstream Transaction Hash: {downstreamHash}"); // Print downstream hash
        }
    }
}
