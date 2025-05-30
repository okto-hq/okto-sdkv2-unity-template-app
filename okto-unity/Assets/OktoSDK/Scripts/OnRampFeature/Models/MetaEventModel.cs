namespace OktoSDK.OnRamp
{
    [System.Serializable]
    public class MetaEventModel
    {
        public string type;
        public MetaDetail detail;
    }

    [System.Serializable]
    public class MetaDetail
    {
        public string orderId;
        public string eventCategory;
        public string paymentStatus;
        public string cryptoSwap;
        public string paymentType;
        public OrderData order;
    }

    [System.Serializable]
    public class OrderData
    {
        public float fiat;
        public string currency;
        public string utr;
        public string receiverWalletAddress;
        public string buyTokenSymbol;
        public string buyTokenAddress;
        public string orderId;
        public string createdAt;
        public int chainId;
        public CustomerData customer;
        public string paymentMode;
    }

    [System.Serializable]
    public class CustomerData
    {
        public string email;
    }
}