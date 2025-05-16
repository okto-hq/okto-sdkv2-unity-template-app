using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WalletType = OktoSDK.BFF.Wallet;
using OktoSDK.Features.Wallet.Features.Wallet;

namespace OktoSDK.Features.Wallet
{
    public class AccountPrefab : MonoBehaviour
    {
        // Fetches the order matching the intentId
        public async Task<List<WalletType>> CallAccountApi()
        {
            List<WalletType> wallets = await WalletService.GetWallets();
            string json = JsonConvert.SerializeObject(wallets, Formatting.Indented);
            CustomLogger.Log("Wallets: " + json);
            return wallets;
        }
        
        /// <summary>
        /// Gets the list of accounts/wallets available to the user
        /// </summary>
        /// <returns>List of wallet accounts</returns>
        public async Task<List<WalletType>> GetAccounts()
        {
            return await WalletService.GetWallets();
        }
    }
}
