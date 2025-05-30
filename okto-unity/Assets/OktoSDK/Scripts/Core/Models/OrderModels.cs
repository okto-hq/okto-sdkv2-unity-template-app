using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OktoSDK.Models.Order
{

    [Serializable]
    public class Order
    {
        [JsonProperty("downstream_transaction_hash")]
        public List<string> DownstreamTransactionHash;

        [JsonProperty("transaction_hash")]
        public List<string> TransactionHash;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("intent_id")]
        public string IntentId;

        [JsonProperty("intent_type")]
        public string IntentType;

        [JsonProperty("network_name")]
        public string NetworkName;

        [JsonProperty("caip_id")]
        public string CaipId;

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

                if (IntentType == "RAW_TRANSACTION")
                {
                    _details = JsonConvert.DeserializeObject<RawTransactionDetails>(value.ToString());
                }
                else if (IntentType == "TOKEN_TRANSFER")
                {
                    _details = JsonConvert.DeserializeObject<TokenTransferDetails>(value.ToString());
                }
                else if (IntentType == "NFT_TRANSFER")
                {
                    _details = JsonConvert.DeserializeObject<NftTransferDetails>(value.ToString());
                }
            }
        }

        [JsonProperty("reason")]
        public string reason;

        [JsonProperty("block_timestamp")]
        public long blockTimestamp;
    }

    [Serializable]
    public class BaseDetails
    {
        [JsonProperty("caip2_id", NullValueHandling = NullValueHandling.Ignore)]
        public string Caip2Id { get; set; }

        [JsonProperty("intent_type", NullValueHandling = NullValueHandling.Ignore)]
        public string IntentType { get; set; }
    }

    [Serializable]
    public class RawTransactionItem
    {
        [JsonProperty("Key")]
        public string Key { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }

    [Serializable]
    public class RawTransactionDetails : BaseDetails
    {
        [JsonProperty("caip2Id")]
        public string Caip2IdAlt { get; set; }

        [JsonProperty("transactions")]
        public List<List<RawTransactionItem>> Transactions { get; set; }
    }

    [Serializable]
    public class TokenTransferDetails : BaseDetails
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("caip2id")]
        public string Caip2IdAlt { get; set; }

        [JsonProperty("recipientWalletAddress")]
        public string RecipientWalletAddress { get; set; }

        [JsonProperty("tokenAddress")]
        public string TokenAddress { get; set; }
    }

    [Serializable]
    public class NftTransferDetails : BaseDetails
    {
        [JsonProperty("collectionaddress")]
        public string CollectionAddress { get; set; }

        [JsonProperty("nftId")]
        public string NftId { get; set; }

        [JsonProperty("recipientWalletAddress")]
        public string RecipientWalletAddress { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("nftType")]
        public string NftType { get; set; }
    }

    [Serializable]
    public class KeyValuePair
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
