using OktoSDK.Models.Order;

namespace OktoSDK.BFF
{
    /// <summary>
    /// Extracts transaction data from RawTransactionDetails objects.
    /// This class abstracts the extraction process to improve maintainability.
    /// </summary>
    public static class TransactionDataExtractor
    {
        /// <summary>
        /// Represents extracted transaction data
        /// </summary>
        public class TransactionData
        {
            public string CallData { get; set; } = "";
            public string ContractAddress { get; set; } = "";
            public string FromAddress { get; set; } = "";
            public string Value { get; set; } = "0";
        }

        /// <summary>
        /// Extracts transaction data from a RawTransactionDetails object
        /// </summary>
        /// <param name="rawTxDetails">The RawTransactionDetails object to extract from</param>
        /// <returns>TransactionData containing the extracted values</returns>
        public static TransactionData ExtractTransactionData(RawTransactionDetails rawTxDetails)
        {
            var result = new TransactionData();

            if (rawTxDetails == null ||
                rawTxDetails.Transactions == null ||
                rawTxDetails.Transactions.Count == 0 ||
                rawTxDetails.Transactions[0] == null)
            {
                return result;
            }

            // Assuming each element in rawTxDetails.Transactions is a list of RawTransactionItem
            foreach (var transactionList in rawTxDetails.Transactions)
            {
                if (transactionList == null)
                    continue;

                // Iterate through the Key-Value pairs in each transaction list
                foreach (var item in transactionList)
                {
                    switch (item.Key.ToLowerInvariant())
                    {
                        case "data":
                            result.CallData = item.Value;
                            break;
                        case "to":
                            result.ContractAddress = item.Value;
                            break;
                        case "from":
                            result.FromAddress = item.Value;
                            break;
                        case "value":
                            result.Value = item.Value;
                            break;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Extracts transaction data from an Order object
        /// </summary>
        /// <param name="order">The Order object to extract from</param>
        /// <returns>TransactionData containing the extracted values</returns>
        public static TransactionData ExtractTransactionData(Order order)
        {
            if (order?.Details is RawTransactionDetails rawTxDetails)
            {
                return ExtractTransactionData(rawTxDetails);
            }
            
            return new TransactionData();
        }
    }
} 
