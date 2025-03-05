using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OktoSDK
{
    public class Wallet
    {
        [JsonProperty("caip_id")]
        public string capId { get; set; }

        [JsonProperty("network_name")]
        public string networkName { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("network_id")]
        public string cap2Id { get; set; }

        [JsonProperty("network_symbol")]
        public string networkSymbol { get; set; }
    }

    [Serializable]
    public class NetworkData
    {
        [JsonProperty("caip_id")]
        public string caipId { get; set; }

        [JsonProperty("network_name")]
        public string networkName { get; set; }

        [JsonProperty("chain_id")]
        public string chainId { get; set; }

        [JsonProperty("logo")]
        public string logo { get; set; }

        [JsonProperty("sponsorship_enabled")]
        public bool sponsorshipEnabled { get; set; }

        [JsonProperty("gsn_enabled")]
        public bool gsnEnabled { get; set; }

        [JsonProperty("type")]
        public ChainType? type { get; set; }

        [JsonProperty("network_id")]
        public string networkId { get; set; }

        [JsonProperty("onramp_enabled")]
        public bool onRampEnabled { get; set; }

        [JsonProperty("whitelisted")]
        public bool whitelisted { get; set; }
    }

    public enum ChainType
    {
        EVM,
        SVM,
        APT
    }

    [Serializable]
    public class Token
    {
        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("caip_id")]
        public string caipId { get; set; }

        [JsonProperty("symbol")]
        public string symbol { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("short_name")]
        public string shortName { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("group_id")]
        public string groupId { get; set; }

        [JsonProperty("is_primary")]
        public bool isPrimary { get; set; }

        [JsonProperty("network_id")]
        public string networkId { get; set; }

        [JsonProperty("network_name")]
        public string networkName { get; set; }

        [JsonProperty("onramp_enabled")]
        public bool isOnrampEnabled { get; set; }

        [JsonProperty("whitelisted")]
        public bool whitelisted { get; set; }
    }

    [Serializable]
    public class UserPortfolioData
    {
        [Serializable]
        public class AggregatedData
        {
            [JsonProperty("holdings_count")]
            public string hldingsCount { get; set; }

            [JsonProperty("holdings_price_inr")]
            public string holdingsPriceInr { get; set; }

            [JsonProperty("holdings_price_usdt")]
            public string holdingsPriceUsdt { get; set; }

            [JsonProperty("total_holding_price_inr")]
            public string totalHoldingPriceInr { get; set; }

            [JsonProperty("total_holding_price_usdt")]
            public string totalHoldingPriceUsdt { get; set; }
        }

        [Serializable]
        public class TokenData
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("symbol")]
            public string symbol { get; set; }

            [JsonProperty("short_name")]
            public string shortName { get; set; }

            [JsonProperty("token_image")]
            public string tokenImage { get; set; }

            [JsonProperty("token_address")]
            public string tokenAddress { get; set; }

            [JsonProperty("caip2_id")]
            public string caip2Id { get; set; }

            [JsonProperty("precision")]
            public string precision { get; set; }

            [JsonProperty("network_name")]
            public string networkName { get; set; }

            [JsonProperty("is_primary")]
            public bool isPrimary { get; set; }

            [JsonProperty("balance")]
            public string balance { get; set; }

            [JsonProperty("holdings_price_usdt")]
            public string holdingsPriceUsdt { get; set; }

            [JsonProperty("holdings_price_inr")]
            public string holdingsPriceInr { get; set; }
        }

        [Serializable]
        public class GroupToken : TokenData
        {
            [JsonProperty("group_id")]
            public string groupId { get; set; }

            [JsonProperty("aggregation_type")]
            public string aggregationType { get; set; }

            [JsonProperty("tokens")]
            public List<TokenData> tokens { get; set; }
        }

        [JsonProperty("aggregated_data")]
        public AggregatedData aggregatedData { get; set; }

        [JsonProperty("group_tokens")]
        public List<GroupToken> groupTokens { get; set; }
    }


    [Serializable]
    public class UserPortfolioActivity
    {
        [JsonProperty("symbol")]
        public string symbol { get; set; }

        [JsonProperty("image")]
        public string image { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("short_name")]
        public string shortName { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("group_id")]
        public string groupId { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("quantity")]
        public string quantity { get; set; }

        [JsonProperty("amount")]
        public string amount { get; set; }

        [JsonProperty("order_type")]
        public string orderType { get; set; }

        [JsonProperty("transfer_type")]
        public string transferType { get; set; }

        [JsonProperty("status")]
        public bool status { get; set; }

        [JsonProperty("created_at")]
        public int createdAt { get; set; }

        [JsonProperty("updated_at")]
        public int updatedAt { get; set; }

        [JsonProperty("timestamp")]
        public int timestamp { get; set; }

        [JsonProperty("tx_hash")]
        public string txHash { get; set; }

        [JsonProperty("network_id")]
        public string networkId { get; set; }

        [JsonProperty("network_name")]
        public string networkName { get; set; }

        [JsonProperty("network_explorer_url")]
        public string networkExplorerUrl { get; set; }

        [JsonProperty("network_symbol")]
        public string networkSymbol { get; set; }

        [JsonProperty("caip_id")]
        public string caipId { get; set; }
    }


    [Serializable]
    public class UserNFTBalance
    {
        [JsonProperty("caip_id")]
        public string caipId;

        [JsonProperty("network_name")]
        public string networkName;

        [JsonProperty("entity_type")]
        public string entityType;

        [JsonProperty("collection_address")]
        public string collectionAddress;

        [JsonProperty("collection_name")]
        public string collectionName;

        [JsonProperty("nft_id")]
        public string nftId;

        [JsonProperty("image")]
        public string image;

        [JsonProperty("quantity")]
        public string quantity;

        [JsonProperty("token_uri")]
        public string tokenUri;

        [JsonProperty("description")]
        public string description;

        [JsonProperty("nft_name")]
        public string nftName;

        [JsonProperty("explorer_smart_contract_url")]
        public string explorerSmartContractUrl;

        [JsonProperty("collection_image")]
        public string collectionImage;
    }

    [Serializable]
    public class RawTransactionItem
    {
        [JsonProperty("Key")]
        public string Key;

        [JsonProperty("Value")]
        public string Value;
    }

    [Serializable]
    public class RawTransactionDetails : BaseDetails
    {
        [JsonProperty("caip2Id")]
        public string Caip2Id;

        [JsonProperty("transactions")]
        public List<List<RawTransactionItem>> transactions;
    }

    [Serializable]
    public class TokenTransferDetails : BaseDetails
    {
        [JsonProperty("amount")]
        public string amount;

        [JsonProperty("caip2id")]
        public string caip2id;

        [JsonProperty("recipientWalletAddress")]
        public string recipientWalletAddress;

        [JsonProperty("tokenAddress")]
        public string tokenAddress;
    }

    [Serializable]
    public class Order
    {
        [JsonProperty("downstream_transaction_hash")]
        public List<string> downstreamTransactionHash;

        [JsonProperty("transaction_hash")]
        public List<string> transactionHash;

        [JsonProperty("status")]
        public string status;

        [JsonProperty("intent_id")]
        public string intentId;

        [JsonProperty("intent_type")]
        public string intentType;

        [JsonProperty("network_name")]
        public string networkName;

        [JsonProperty("caip_id")]
        public string caipId;

        private BaseDetails _details;
              
        [JsonIgnore]
        public BaseDetails Details => _details;

        [JsonProperty("details")]
        public object RawDetails
        {
            get { return _details; }
            set
            {
                if (value == null) return;

                if (intentType == "RAW_TRANSACTION")
                {
                    _details = JsonConvert.DeserializeObject<RawTransactionDetails>(value.ToString());
                }
                else if (intentType == "TOKEN_TRANSFER")
                {
                    _details = JsonConvert.DeserializeObject<TokenTransferDetails>(value.ToString());
                }
                else if (intentType == "NFT_TRANSFER")
                {
                    _details = JsonConvert.DeserializeObject<NftTransferDetails>(value.ToString());
                }
                // Add other types as needed
            }
        }

        [JsonProperty("reason")]
        public string reason;

        [JsonProperty("block_timestamp")]
        public long blockTimestamp;
    }

    [Serializable]
    public class OrderFilterRequest
    {
        [JsonProperty("intent_id")]
        public string intentId;

        [JsonProperty("status")]
        public string status;

        [JsonProperty("intent_type")]
        public string intentType;
    }

    [Serializable]
    public class EstimateOrderPayload
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("job_id")]
        public string JobId;

        [JsonProperty("paymaster_data")]
        public Dictionary<string, string> PaymasterData;

        [Serializable]
        public class GasDetails
        {
            [JsonProperty("max_fee_per_gas")]
            public string MaxFeePerGas;

            [JsonProperty("max_priority_fee_per_gas")]
            public string MaxPriorityFeePerGas;
        }

        [JsonProperty("gas_details")]
        public GasDetails gasDetails;

        [Serializable]
        public class Details
        {
            [JsonProperty("recipient_wallet_address")]
            public string RecipientWalletAddress;

            [JsonProperty("caip2_id")]
            public string Caip2Id;

            [JsonProperty("token_address")]
            public string TokenAddress;

            [JsonProperty("amount")]
            public string Amount;
        }

        [JsonProperty("details")]
        public Details details;
    }

    [Serializable]
    public class OrderEstimateResponse
    {
        [JsonProperty("encoded_call_data")]
        public string EncodedCallData;

        [JsonProperty("encoded_paymaster")]
        public string EncodedPaymaster;

        [Serializable]
        public class GasData
        {
            [JsonProperty("call_gas_limit")]
            public string CallGasLimit;

            [JsonProperty("verification_gas_limit")]
            public string VerificationGasLimit;

            [JsonProperty("pre_verification_gas")]
            public string PreVerificationGas;

            [JsonProperty("paymaster_verification_gas_limit")]
            public string PaymasterVerificationGasLimit;

            [JsonProperty("paymaster_post_op_gas_limit")]
            public string PaymasterPostOpGasLimit;
        }

        [JsonProperty("gas_data")]
        public GasData gasData;

        [Serializable]
        public class PaymasterData
        {
            [JsonProperty("paymaster_id")]
            public string PaymasterId;

            [JsonProperty("valid_until")]
            public string ValidUntil;

            [JsonProperty("valid_after")]
            public string ValidAfter;
        }

        [JsonProperty("paymaster_data")]
        public PaymasterData paymasterData;

        [Serializable]
        public class Details
        {
            [Serializable]
            public class Estimation
            {
                [JsonProperty("amount")]
                public string Amount;
            }

            [Serializable]
            public class Fees
            {
                [JsonProperty("transaction_fees")]
                public Dictionary<string, string> TransactionFees;

                [JsonProperty("approx_transaction_fees_in_usdt")]
                public string ApproxTransactionFeesInUsdt;
            }

            [JsonProperty("estimation")]
            public Estimation estimation;

            [JsonProperty("fees")]
            public Fees fees;
        }

        [JsonProperty("details")]
        public Details details;

        [Serializable]
        public class CallData
        {
            [Serializable]
            public class Policies
            {
                [JsonProperty("gsn_enabled")]
                public bool GsnEnabled;

                [JsonProperty("sponsorship_enabled")]
                public bool SponsorshipEnabled;
            }

            [Serializable]
            public class GSN
            {
                [Serializable]
                public class GSNDetails
                {
                    [JsonProperty("required_networks")]
                    public List<string> RequiredNetworks;

                    [Serializable]
                    public class TokenInfo
                    {
                        [JsonProperty("caip2_id")]
                        public string Caip2Id;

                        [JsonProperty("address")]
                        public string Address;

                        [JsonProperty("amount")]
                        public string Amount;

                        [JsonProperty("amount_in_usdt")]
                        public string AmountInUsdt;
                    }

                    [JsonProperty("tokens")]
                    public List<TokenInfo> Tokens;
                }

                [JsonProperty("is_required")]
                public bool IsRequired;

                [JsonProperty("details")]
                public GSNDetails Details;
            }

            [JsonProperty("intent_type")]
            public string IntentType;

            [JsonProperty("job_id")]
            public string JobId;

            [JsonProperty("client_id")]
            public string ClientId;

            [JsonProperty("creator_id")]
            public string craeteId;

            [JsonProperty("policies")]
            public Policies policies;

            [JsonProperty("gsn")]
            public GSN Gsn;

            [JsonProperty("payload")]
            public EstimateOrderPayload.Details Payload;
        }

        [JsonProperty("call_data")]
        public CallData callData;
    }

    [Serializable]
    public class UserSessionResponse
    {
        // Add user session response properties with [JsonProperty] attributes
    }

    public static class OrderTypes
    {
        public enum INTENT_TYPE
        {
            RAW_TRANSACTION,
            NFT_MINT,
            TOKEN_TRANSFER,
            NFT_TRANSFER
        }

        public enum STATUS_TYPE
        {
            SUCCESSFUL,
            IN_PROGRESS,
            FAILED
        }

        public static string ToString(INTENT_TYPE? type)
        {
            return type?.ToString();
        }

        public static string ToString(STATUS_TYPE? status)
        {
            return status?.ToString();
        }

        public static INTENT_TYPE? ParseIntentType(string type)
        {
            if (string.IsNullOrEmpty(type)) return null;
            return (INTENT_TYPE)Enum.Parse(typeof(INTENT_TYPE), type);
        }

        public static STATUS_TYPE? ParseStatusType(string status)
        {
            if (string.IsNullOrEmpty(status)) return null;
            return (STATUS_TYPE)Enum.Parse(typeof(STATUS_TYPE), status);
        }
    }

    [Serializable]
    public class BaseDetails
    {
        [JsonProperty("caip2_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Caip2Id;

        [JsonProperty("intent_type", NullValueHandling = NullValueHandling.Ignore)]
        public string IntentType;
    }

    [Serializable]
    public class KeyValuePair
    {
        [JsonProperty("key")]
        public string Key;

        [JsonProperty("value")]
        public string Value;
    }

    [Serializable]
    public class NFTProperty
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("value")]
        public string Value;

        [JsonProperty("value_type")]
        public string ValueType;
    }

    [Serializable]
    public class NftMintDetails : BaseDetails
    {
        [JsonProperty("collection_name")]
        public string collectionName;

        [JsonProperty("description")]
        public string description;

        [JsonProperty("nft_name")]
        public string nftName;

        [JsonProperty("properties")]
        public List<NFTProperty> properties;

        [JsonProperty("uri")]
        public string uri;

        public NftMintDetails()
        {
            IntentType = "NFT_MINT";
        }
    }

    [Serializable]
    public class NftTransferDetails : BaseDetails
    {
        //[JsonProperty("collection_address")]
        public string collectionAddress;

        //[JsonProperty("nft_id")]
        public string nftId;

        //[JsonProperty("recipient_wallet_address")]
        public string recipientWalletAddress;

        //[JsonProperty("amount")]
        public string amount;

        //[JsonProperty("nft_type")]
        public string nftType;

        public NftTransferDetails()
        {
            IntentType = "NFT_TRANSFER";
        }
    }

    [Serializable]
    public class NFTOrderDetails
    {
        [JsonProperty("job_id")]
        public string jobId;

        [JsonProperty("status")]
        public string status;

        [JsonProperty("order_type")]
        public string orderType;

        [JsonProperty("caip2_id")]
        public string caip2Id;

        [JsonProperty("created_at")]
        public string createdAt;

        [JsonProperty("updated_at")]
        public string updatedAt;
    }
}