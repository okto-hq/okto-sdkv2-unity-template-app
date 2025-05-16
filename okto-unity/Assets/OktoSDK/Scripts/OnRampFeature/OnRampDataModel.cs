using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OktoSDK.BFF;
using static OktoSDK.BFF.UserPortfolioData;

namespace OktoSDK.OnRamp
{
    [Serializable]
    public class OnRampToken
    {
        public WhitelistedToken WhitelistedToken { get; }
        public Token Token { get; }

        public OnRampToken(WhitelistedToken whitelistedToken, Token token = null)
        {
            WhitelistedToken = whitelistedToken;
            Token = token;
        }

        public Dictionary<string, object> AckJson()
        {
            return new Dictionary<string, object>
        {
            { "id", WhitelistedToken.Id },
            { "name", WhitelistedToken.Name },
            { "symbol", WhitelistedToken.ShortName },
            { "iconUrl", WhitelistedToken.Image },
            { "networkId", WhitelistedToken.NetworkId },
            { "networkName", WhitelistedToken.NetworkName },
            { "address", WhitelistedToken.Address },
            { "balance", Token?.Balance },
            { "precision", Token?.Precision },
            { "chainId", WhitelistedToken.ChainId }
        };
        }
    }

    [Serializable]
    public class WhitelistedToken
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("logo")]
        public string Image { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("token_id")]
        public string Id { get; set; }

        [JsonProperty("token_group_id")]
        public string GroupId { get; set; }

        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("network_id")]
        public string NetworkId { get; set; }

        [JsonProperty("network_name")]
        public string NetworkName { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("chain_id")]
        public string ChainId { get; set; }

        [JsonProperty("precision")]
        public string? Precision { get; set; }

        public static WhitelistedToken FromJson(string json) => JsonConvert.DeserializeObject<WhitelistedToken>(json);
        public string ToJson() => JsonConvert.SerializeObject(this);
    }

    public class WhiteListedTokens
    {
        public List<WhitelistedToken> Tokens { get; set; }

        public static WhiteListedTokens FromJson(string json) => JsonConvert.DeserializeObject<WhiteListedTokens>(json);
        public string ToJson() => JsonConvert.SerializeObject(this);
    }

    public class WhiteListedOnRampTokens
    {
        public List<WhitelistedToken> OnRampTokens { get; set; } = new List<WhitelistedToken>();
        public List<WhitelistedToken> OffRampTokens { get; set; } = new List<WhitelistedToken>();

        public static WhiteListedOnRampTokens FromJson(string json) => JsonConvert.DeserializeObject<WhiteListedOnRampTokens>(json);
        public string ToJson() => JsonConvert.SerializeObject(this);
    }

    public class Tokens
    {
        public List<Token> TokenList { get; set; } = new List<Token>();

        public static Tokens FromJson(string json) => JsonConvert.DeserializeObject<Tokens>(json);
        public string ToJson() => JsonConvert.SerializeObject(this);
    }

    [Serializable]
    public class Token
    {

        [JsonProperty("image")]
        public string TokenImage { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("group_id")]
        public string GroupId { get; set; }

        [JsonProperty("network_id")]
        public string NetworkId { get; set; }

        [JsonProperty("is_primary")]
        public bool? IsPrimary { get; set; }

        public string HoldingsPriceUsdt { get; set; }
        public string HoldingsPriceInr { get; set; }
        public string? Balance { get; set; }
        public string? Precision { get; set; }
        public string TokenAddress { get; set; }
        public string NetworkName { get; set; }

        public static Token FromJson(string json) => JsonConvert.DeserializeObject<Token>(json);
        public string ToJson() => JsonConvert.SerializeObject(this);

        // Add this conversion method inside the Token class
        public static Token FromTokenData(TokenData tokenData)
        {
            return new Token
            {
                Id = tokenData.id,
                Symbol = tokenData.symbol,
                ShortName = tokenData.shortName,
                TokenImage = tokenData.tokenImage,
                TokenAddress = tokenData.tokenAddress,
                NetworkId = tokenData.caip2Id,
                Precision = tokenData.precision,
                NetworkName = tokenData.networkName,
                IsPrimary = tokenData.isPrimary,
                Balance = tokenData.balance,
                HoldingsPriceUsdt = tokenData.holdingsPriceUsdt,
                HoldingsPriceInr = tokenData.holdingsPriceInr
            };
        }

    }

    //[System.Serializable]
    //public class UserData
    //{
    //    public string userId;
    //    public string email;
    //    public string walletAddress;
    //    public string username;
    //}


    //[System.Serializable]
    //public class TransactionResult
    //{
    //    public string transactionId;
    //    public string tokenId;
    //    public string tokenSymbol;
    //    public string amount;
    //    public string status;
    //    public long timestamp;
    //}

    //[System.Serializable]
    //public class TokenBalanceUpdate
    //{
    //    public string tokenId;
    //    public string newBalance;
    //    public string timestamp;
    //}

    //[System.Serializable]
    //public class PaymentResult
    //{
    //    public string status;
    //    public string message;
    //    public string transactionId;
    //    public string amount;
    //    public string currency;
    //    public string paymentMethod;
    //    public long timestamp;
    //}

    [Serializable]
    public class CombinedToken
    {
        public string name;
        public string symbol;
        public string shortName;
        public string id;
        public string groupId;
        public string holdingsPriceUsdt;
        public string holdingsPriceInr;
        public string balance;
        public string networkId;
        public bool isPrimary;
        public string tokenImage;
        public string networkName;
        public string walletAddress;
        public string chainId;
        public string email;
        public string userId;
        public Token token;

        public CombinedToken(WhitelistedToken whitelistedToken, GroupToken portfolioToken, Wallet wallet, UserFromToken userFromToken)
        {
            name = whitelistedToken.Name;
            symbol = whitelistedToken.Symbol;
            shortName = whitelistedToken.ShortName;
            id = whitelistedToken.Id;
            groupId = whitelistedToken.GroupId;
            holdingsPriceUsdt = portfolioToken?.holdingsPriceUsdt ?? "0";
            holdingsPriceInr = portfolioToken?.holdingsPriceInr ?? "0";
            balance = portfolioToken?.balance ?? "0";
            isPrimary = portfolioToken?.isPrimary ?? false;
            tokenImage = whitelistedToken.Image;
            networkName = wallet?.networkName ?? "";
            networkId = wallet?.cap2Id ?? "";
            walletAddress = wallet?.address ?? "";
            chainId = wallet.networkName;
            email = userFromToken.Email;
            userId = userFromToken.UserId;

            // Find matching token in portfolio token list
            if (portfolioToken?.tokens != null)
            {
                TokenData matchingToken = portfolioToken.tokens.FirstOrDefault(t => whitelistedToken.Id == t.id);
                if (matchingToken != null)
                {
                    token = Token.FromTokenData(matchingToken);
                }
            }
        }
    }
}