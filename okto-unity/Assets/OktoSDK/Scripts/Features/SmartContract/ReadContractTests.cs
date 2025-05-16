using UnityEngine;
using System.Threading.Tasks;
using System;

//test cases for readcontract
namespace OktoSDK.Features.SmartContract
{
    public class ReadContractTests : MonoBehaviour
    {
        public ReadContract readContract;
        private string contractAddress = "0x554B47F324bf8Dc0e9cCF82B16c2DdA21befFE86"; // Replace with your ERC20 token address
        private string ownerAddress = "0xDe312dE985216CD2407cD499152D89fad2Aa662B"; // Replace with token owner address
        private string testAddress = "0xdce77344d59fEF3f96587eA6244674CcEa21d2B9"; // Test address for allowances, etc.

        public async void OnEnable()
        {
            // Calling all test cases and storing results
            string name = await TestName();
            string symbol = await TestSymbol();
            string decimals = await TestDecimals();
            string totalSupply = await TestTotalSupply();
            string balance = await TestBalanceOf();
            string allowance = await TestAllowance();
            string domainSeparator = await TestDomainSeparator();
            string nonces = await TestNonces();
            string eip712Domain = await TestEip712Domain();

            // Displaying all responses in the ResponsePanel
            string response = $"Name: {name}\n" +
                              $"Symbol: {symbol}\n" +
                              $"Decimals: {decimals}\n" +
                              $"Total Supply: {totalSupply}\n" +
                              $"Balance of {ownerAddress}: {balance}\n" +
                              $"Allowance from {ownerAddress} to {testAddress}: {allowance}\n" +
                              $"Domain Separator: {domainSeparator}\n" +
                              $"Nonces for {ownerAddress}: {nonces}\n" +
                              $"EIP-712 Domain: {eip712Domain}";

            ResponsePanel.SetResponse(response);
        }

        public async Task<string> TestName()
        {
            try
            {
                string abi = @"[{""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "name";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress);
            }
            catch (Exception ex)
            {
                return $"Error in TestName: {ex.Message}";
            }
        }

        public async Task<string> TestSymbol()
        {
            try
            {
                string abi = @"[{""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "symbol";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress);
            }
            catch (Exception ex)
            {
                return $"Error in TestSymbol: {ex.Message}";
            }
        }

        public async Task<string> TestDecimals()
        {
            try
            {
                string abi = @"[{""inputs"":[],""name"":""decimals"",""outputs"":[{""internalType"":""uint8"",""name"":"""",""type"":""uint8""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "decimals";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress);
            }
            catch (Exception ex)
            {
                return $"Error in TestDecimals: {ex.Message}";
            }
        }

        public async Task<string> TestTotalSupply()
        {
            try
            {
                string abi = @"[{""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "totalSupply";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress);
            }
            catch (Exception ex)
            {
                return $"Error in TestTotalSupply: {ex.Message}";
            }
        }

        public async Task<string> TestBalanceOf()
        {
            try
            {
                string abi = @"[{""inputs"":[{""internalType"":""address"",""name"":""account"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "balanceOf";
                string parameters = ownerAddress;

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress, parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestBalanceOf: {ex.Message}";
            }
        }

        public async Task<string> TestAllowance()
        {
            try
            {
                string abi = @"[{""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""},{""internalType"":""address"",""name"":""spender"",""type"":""address""}],""name"":""allowance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "allowance";
                string parameters = $"{ownerAddress},{testAddress}";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress, parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestAllowance: {ex.Message}";
            }
        }

        public async Task<string> TestDomainSeparator()
        {
            try
            {
                string abi = @"[{""inputs"":[],""name"":""DOMAIN_SEPARATOR"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "DOMAIN_SEPARATOR";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress);
            }
            catch (Exception ex)
            {
                return $"Error in TestDomainSeparator: {ex.Message}";
            }
        }

        public async Task<string> TestNonces()
        {
            try
            {
                string abi = @"[{""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""}],""name"":""nonces"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "nonces";
                string parameters = ownerAddress;

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress, parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestNonces: {ex.Message}";
            }
        }

        public async Task<string> TestEip712Domain()
        {
            try
            {
                string abi = @"[{""inputs"":[],""name"":""eip712Domain"",""outputs"":[{""internalType"":""bytes1"",""name"":""fields"",""type"":""bytes1""},{""internalType"":""string"",""name"":""name"",""type"":""string""},{""internalType"":""string"",""name"":""version"",""type"":""string""},{""internalType"":""uint256"",""name"":""chainId"",""type"":""uint256""},{""internalType"":""address"",""name"":""verifyingContract"",""type"":""address""},{""internalType"":""bytes32"",""name"":""salt"",""type"":""bytes32""},{""internalType"":""uint256[]"",""name"":""extensions"",""type"":""uint256[]""}],""stateMutability"":""view"",""type"":""function""}]";
                string functionName = "eip712Domain";

                return await readContract.ReadSmartContractAsync(abi, functionName, contractAddress);
            }
            catch (Exception ex)
            {
                return $"Error in TestEip712Domain: {ex.Message}";
            }
        }


    }

}
