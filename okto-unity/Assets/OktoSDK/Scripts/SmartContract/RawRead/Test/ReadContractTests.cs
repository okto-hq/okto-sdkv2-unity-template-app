using UnityEngine;
using System.Threading.Tasks;
using System;
using OktoSDK.Auth;

namespace OktoSDK.Features.SmartContract
{
    public class ReadSmartContractTests : MonoBehaviour
    {
        private string contractAddress = "0x833589fCD6eDb6E08f4c7C32D4f71b54bdA02913";
        private string ownerAddress = "0xB7B8F759E8Bd293b91632100f53a45859832f463";
        private string testAddress = "0xdce77344d59fEF3f96587eA6244674CcEa21d2B9";
        private string network = "eip155:8453";
        private string endpoint;
        private string authToken;

        public async void OnEnable()
        {
            endpoint = EnvironmentHelper.GetBffBaseUrl() + "/api/oc/v1/readContractData";
            authToken = OktoAuthManager.GetOktoClient().GetAuthorizationToken();

            string name = await TestName();
            string symbol = await TestSymbol();
            string decimals = await TestDecimals();
            string totalSupply = await TestTotalSupply();
            string balance = await TestBalanceOf();
            string balanceSpecific = await TestBalanceOfSpecific();
            string allowance = await TestAllowance();
            string domainSeparator = await TestDomainSeparator();
            string nonces = await TestNonces();
            string eip712Domain = await TestEip712Domain();

            string cancelAuthorizationTypeHash = await TestCancelAuthorizationTypeHash();
            string permitTypeHash = await TestPermitTypeHash();
            string receiveWithAuthorizationTypeHash = await TestReceiveWithAuthorizationTypeHash();
            string transferWithAuthorizationTypeHash = await TestTransferWithAuthorizationTypeHash();

            string authorizationState = await TestAuthorizationState(); // example nonce; replace as needed
            string currency = await TestCurrency();
            string isBlacklisted = await TestIsBlacklisted();
            string isMinter = await TestIsMinter();

            string response = $"Name: {name}\n" +
                              $"Symbol: {symbol}\n" +
                              $"Decimals: {decimals}\n" +
                              $"Total Supply: {totalSupply}\n" +
                              $"Balance of {ownerAddress}: {balance}\n" +
                              $"Balance Specific Test: {balanceSpecific}\n" +
                              $"Allowance from {ownerAddress} to {testAddress}: {allowance}\n" +
                              $"Domain Separator: {domainSeparator}\n" +
                              $"Nonces for {ownerAddress}: {nonces}\n" +
                              $"EIP-712 Domain: {eip712Domain}\n" +
                              $"Cancel Authorization Type Hash: {cancelAuthorizationTypeHash}\n" +
                              $"Permit Type Hash: {permitTypeHash}\n" +
                              $"Receive With Authorization Type Hash: {receiveWithAuthorizationTypeHash}\n" +
                              $"Transfer With Authorization Type Hash: {transferWithAuthorizationTypeHash}\n" +
                              $"Authorization State (example nonce 0x0): {authorizationState}\n" +
                              $"Currency: {currency}\n" +
                              $"Is Blacklisted ({ownerAddress}): {isBlacklisted}\n" +
                              $"Is Minter ({ownerAddress}): {isMinter}";

            ResponsePanel.SetResponse(response);
        }


        public async Task<string> TestName()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "name";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "symbol";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "decimals";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "totalSupply";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string parameters = "{\"account\":\"" + ownerAddress + "\"}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken, parameters);
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
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "allowance";
                string parameters = "{\"owner\":\"" + ownerAddress + "\",\"spender\":\"" + testAddress + "\"}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[],\"name\":\"DOMAIN_SEPARATOR\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "DOMAIN_SEPARATOR";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"nonces\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "nonces";
                string parameters = "{\"owner\":\"" + ownerAddress + "\"}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
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
                string abi = "{\"inputs\":[],\"name\":\"eip712Domain\",\"outputs\":[{\"internalType\":\"bytes1\",\"name\":\"fields\",\"type\":\"bytes1\"},{\"internalType\":\"string\",\"name\":\"name\",\"type\":\"string\"},{\"internalType\":\"string\",\"name\":\"version\",\"type\":\"string\"},{\"internalType\":\"uint256\",\"name\":\"chainId\",\"type\":\"uint256\"},{\"internalType\":\"address\",\"name\":\"verifyingContract\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"salt\",\"type\":\"bytes32\"},{\"internalType\":\"uint256[]\",\"name\":\"extensions\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "eip712Domain";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestEip712Domain: {ex.Message}";
            }
        }

        public async Task<string> TestBalanceOfSpecific()
        {
            try
            {
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "balanceOf";
                string parameters = "{\"account\":\"0xB7B8F759E8Bd293b91632100f53a45859832f463\"}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestBalanceOfSpecific: {ex.Message}";
            }
        }

        public async Task<string> TestCancelAuthorizationTypeHash()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"CANCEL_AUTHORIZATION_TYPEHASH\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "CANCEL_AUTHORIZATION_TYPEHASH";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestCancelAuthorizationTypeHash: {ex.Message}";
            }
        }

        public async Task<string> TestPermitTypeHash()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"PERMIT_TYPEHASH\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "PERMIT_TYPEHASH";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestPermitTypeHash: {ex.Message}";
            }
        }

        public async Task<string> TestReceiveWithAuthorizationTypeHash()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"RECEIVE_WITH_AUTHORIZATION_TYPEHASH\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "RECEIVE_WITH_AUTHORIZATION_TYPEHASH";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestReceiveWithAuthorizationTypeHash: {ex.Message}";
            }
        }

        public async Task<string> TestTransferWithAuthorizationTypeHash()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"TRANSFER_WITH_AUTHORIZATION_TYPEHASH\",\"outputs\":[{\"internalType\":\"bytes32\",\"name\":\"\",\"type\":\"bytes32\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "TRANSFER_WITH_AUTHORIZATION_TYPEHASH";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestTransferWithAuthorizationTypeHash: {ex.Message}";
            }
        }

        public async Task<string> TestAuthorizationState()
        {
            try
            {
                string authorizer = "0xB7B8F759E8Bd293b91632100f53a45859832f463"; // assign inside method
                string nonce = "0x1234567890abcdef1234567890abcdef1234567890abcdef1234567890abcdef"; // example bytes32 hex string
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"authorizer\",\"type\":\"address\"},{\"internalType\":\"bytes32\",\"name\":\"nonce\",\"type\":\"bytes32\"}],\"name\":\"authorizationState\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "authorizationState";
                string parameters = $"{{\"authorizer\":\"{authorizer}\",\"nonce\":\"{nonce}\"}}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken, parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestAuthorizationState: {ex.Message}";
            }
        }

        public async Task<string> TestCurrency()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"currency\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "currency";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestCurrency: {ex.Message}";
            }
        }

        public async Task<string> TestIsBlacklisted()
        {
            try
            {
                string account = "0xB7B8F759E8Bd293b91632100f53a45859832f463"; // assign inside method
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_account\",\"type\":\"address\"}],\"name\":\"isBlacklisted\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "isBlacklisted";
                string parameters = $"{{\"_account\":\"{account}\"}}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken, parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestIsBlacklisted: {ex.Message}";
            }
        }

        public async Task<string> TestIsMinter()
        {
            try
            {
                string account = "0xB7B8F759E8Bd293b91632100f53a45859832f463"; // assign inside method
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"isMinter\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "isMinter";
                string parameters = $"{{\"account\":\"{account}\"}}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken, parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestIsMinter: {ex.Message}";
            }
        }

        public async Task<string> TestMasterMinter()
        {
            try
            {
                string abi = "{\"inputs\":[],\"name\":\"masterMinter\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "masterMinter";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken);
            }
            catch (Exception ex)
            {
                return $"Error in TestMasterMinter: {ex.Message}";
            }
        }

        public async Task<string> TestMinterAllowance()
        {
            try
            {
                string minter = "0xabcdefabcdefabcdefabcdefabcdefabcdef"; // assign inside method
                string abi = "{\"inputs\":[{\"internalType\":\"address\",\"name\":\"minter\",\"type\":\"address\"}],\"name\":\"minterAllowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}";
                string functionName = "minterAllowance";
                string parameters = $"{{\"minter\":\"{minter}\"}}";
                return await ReadSmartContract.ReadContractAsync(network, abi, contractAddress, endpoint, authToken,parameters);
            }
            catch (Exception ex)
            {
                return $"Error in TestMinterAllowance: {ex.Message}";
            }
        }

    }
} 