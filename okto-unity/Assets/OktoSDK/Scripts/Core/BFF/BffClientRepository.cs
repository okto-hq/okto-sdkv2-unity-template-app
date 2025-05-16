using Newtonsoft.Json;
using OktoSDK.Auth;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderType = OktoSDK.Models.Order.Order;

namespace OktoSDK.BFF
{
    public static class BffClientRepository
    {
        public static class Routes
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

        private static ApiClient bffClient;

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
            public List<T> wallets;
            public List<T> data;
            public List<T> onramp_tokens;
            public List<T> offramp_tokens;
        }

        public static void InitializeApiClient()
        {
            bffClient = new ApiClient(EnvironmentHelper.GetBffBaseUrl(), OktoAuthManager.GetOktoClient());
        }

        public static async Task<List<Wallet>> GetWallets()
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

        public static async Task<List<NetworkData>> GetSupportedNetworks()
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

        public static async Task<UserSessionResponse> VerifySession()
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

        public static async Task<List<Token>> GetSupportedTokens()
        {
            var response = await bffClient.Get<ApiResponseWithCount<Token>>(Routes.GetSupportedTokens);

            if (response.data == null)
            {
                throw new Exception("Response data is missing");
            }

            return response.data.tokens;
        }

        public static async Task<UserPortfolioData> GetPortfolio()
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

        public static async Task<List<UserPortfolioActivity>> GetPortfolioActivity()
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

        public static async Task<List<UserNFTBalance>> GetPortfolioNft()
        {
            var response = await bffClient.Get<ApiResponseWithCount<UserNFTBalance>>(Routes.GetPortfolioNft);

            if (response.status == "error")
            {
                throw new Exception(JsonConvert.SerializeObject(response));
            }

            if (response.data == null)
            {
                throw new Exception(JsonConvert.SerializeObject(response));
            }

            return response.data.details;
        }

        public static async Task<List<OrderType>> GetOrders(OrderFilterRequest filters = null)
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

            var response = await bffClient.Get<ApiResponseWithCount<OrderType>>(Routes.GetOrders + (queryParams.Length > 0 ? "?" + queryParams : ""));

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

        public static async Task<OrderEstimateResponse> EstimateOrder(EstimateOrderPayload payload)
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
