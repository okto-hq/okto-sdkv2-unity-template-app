using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/*
 * BffClientRepository Class
 * 
 * This class acts as a client repository for communicating with the backend-for-frontend (BFF) API. 
 * It provides methods for retrieving and sending data related to wallets, networks, tokens, portfolios, orders, 
 * and user sessions. It uses the ApiClient class to handle HTTP requests.
 *
 * Features:
 * - Defines API routes for different GET and POST requests.
 * - Implements methods to interact with API endpoints, including:
 *   - Retrieving wallet details.
 *   - Fetching supported networks and tokens.
 *   - Fetching user portfolio data, NFT balances, and activity history.
 *   - Managing orders (retrieval and estimation).
 *   - Verifying user session authentication.
 * - Handles API responses with error checking and exception handling.
 * - Uses a singleton approach to maintain a single instance of the repository.
 *
 * Methods:
 * - GetWallets(): Retrieves the list of user wallets.
 * - GetSupportedNetworks(): Retrieves a list of supported blockchain networks.
 * - GetSupportedTokens(): Retrieves a list of supported tokens.
 * - GetPortfolio(): Fetches aggregated portfolio data.
 * - GetPortfolioActivity(): Retrieves user portfolio activity.
 * - GetPortfolioNft(): Retrieves user NFT balances.
 * - GetOrders(OrderFilterRequest filters = null): Retrieves orders with optional filters.
 * - EstimateOrder(EstimateOrderPayload payload): Estimates an order based on the provided payload.
 * - VerifySession(): Verifies the user's authentication session.
 *
 * Usage:
 * - The class should be attached to a Unity GameObject to function properly.
 * - Initialize the API client using InitializeApiClient().
 * - Use the provided methods to interact with the BFF API.
 */


namespace OktoSDK
{
    public class BffClientRepository : MonoBehaviour
    {
        private static class Routes
        {
            // GET
            public const string GetWallets = "/api/oc/v1/wallets";
            public const string GetSupportedNetworks = "/api/oc/v1/supported/networks";
            public const string GetSupportedTokens = "/api/oc/v1/supported/tokens";
            public const string GetPortfolio = "/api/oc/v1/aggregated-portfolio";
            public const string GetPortfolioActivity = "/api/oc/v1/portfolio/activity";
            public const string GetPortfolioNft = "/api/oc/v1/portfolio/nft";
            public const string GetOrders = "/api/oc/v1/orders";
            public const string GetNftOrderDetails = "/api/oc/v1/nft/order-details";

            // POST
            public const string EstimateOrder = "/api/oc/v1/estimate";
            public const string VerifySession = "/api/oc/v1/verify-session";
        }

        //public OktoAuthExample oktoAuthExample;
        private ApiClient bffClient;

        [Serializable]
        public class ApiResponse<T>
        {
            public string status;
            public string error;
            public T data;
        }

        [Serializable]
        public class ApiResponseWithCount<T>
        {
            public string status;
            public string error;
            public CountData<T> data;
        }

        [Serializable]
        public class CountData<T>
        {
            public List<T> items;
            public List<T> tokens;
            public List<T> network;
            public List<T> activity;
            public List<T> details;
        }

        private static BffClientRepository _bffClientRepository;

        private void OnEnable()
        {
            _bffClientRepository = this;
        }

        public static BffClientRepository GetBffClientRepository()
        {
            return _bffClientRepository;
        }

        public static void InitializeApiClient()
        {
            _bffClientRepository.bffClient = new ApiClient(OktoAuthExample.getOktoClient().Env.BffBaseUrl, OktoAuthExample.getOktoClient());
        }

        public async Task<List<Wallet>> GetWallets()
        {

            var response = await bffClient.Get<ApiResponse<List<Wallet>>>(Routes.GetWallets);


            if (response.status == "error")
            {
                throw new Exception("Failed to retrieve supported wallets");
            }

            if (response.data == null) 
            {
                throw new Exception("Response data is missing");
            }

            return response.data;
        }

        public async Task<List<NetworkData>> GetSupportedNetworks()
        {
            var response = await bffClient.Get<ApiResponseWithCount<NetworkData>>(Routes.GetSupportedNetworks);

            if (response.status == "error")
            {
                throw new Exception("Failed to retrieve supported networks");
            }

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data.network;
        }

        public async Task<UserSessionResponse> VerifySession()
        {
            var response = await bffClient.Post<ApiResponse<UserSessionResponse>>(Routes.VerifySession);

            if (response.status == "error")
            {
                throw new Exception("Failed to verify user session");
            }

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data;
        }

        public async Task<List<Token>> GetSupportedTokens()
        {
            var response = await bffClient.Get<ApiResponseWithCount<Token>>(Routes.GetSupportedTokens);

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data.tokens;
        }

        public async Task<UserPortfolioData> GetPortfolio()
        {
            var response = await bffClient.Get<ApiResponse<UserPortfolioData>>(Routes.GetPortfolio);

            if (response.status == "error")
            {
                throw new Exception("Failed to retrieve portfolio");
            }

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data;
        }

        public async Task<List<UserPortfolioActivity>> GetPortfolioActivity()
        {
            var response = await bffClient.Get<ApiResponseWithCount<UserPortfolioActivity>>(Routes.GetPortfolioActivity);

            if (response.status == "error")
            {
                throw new Exception("Failed to retrieve portfolio activity");
            }

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data.activity;
        }

        public async Task<List<UserNFTBalance>> GetPortfolioNft()
        {
            var response = await bffClient.Get<ApiResponseWithCount<UserNFTBalance>>(Routes.GetPortfolioNft);

            if (response.status == "error")
            {
                throw new Exception("Failed to retrieve NFT portfolio");
            }

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data.details;
        }

        public async Task<List<Order>> GetOrders(OrderFilterRequest filters = null)
        {
            string queryParams = "";
            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.intentId))
                    queryParams += $"intent_id={filters.intentId}&";
                if (!string.IsNullOrEmpty(filters.status))
                    queryParams += $"status={filters.status}&";
                if (!string.IsNullOrEmpty(filters.intentType))
                    queryParams += $"intent_type={filters.intentType}";
            }

            var response = await bffClient.Get<ApiResponseWithCount<Order>>(Routes.GetOrders + (queryParams.Length > 0 ? "?" + queryParams : ""));

            if (response.status == "error")
            {
                throw new Exception($"Failed to retrieve orders: {response.error}");
            }

            if (response.data?.items == null)
            {
                throw new Exception("No orders found or response data is missing.");
            }

            return response.data.items;
        }

        public async Task<OrderEstimateResponse> EstimateOrder(EstimateOrderPayload payload)
        {
            var response = await bffClient.Post<ApiResponse<OrderEstimateResponse>>(Routes.EstimateOrder, payload);

            if (response.status == "error")
            {
                throw new Exception("Failed to estimate order");
            }

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data;
        }
    }
}