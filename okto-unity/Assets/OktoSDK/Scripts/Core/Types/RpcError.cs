namespace OktoSDK
{
    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public string data { get; set; }

        public Error(int code, string message, string data)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }
    }

    public class RpcError: System.Exception
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }
        public Error error { get; set; }

        public RpcError(string id,string jsonrpc,Error error) 
        {
            this.jsonrpc = jsonrpc;
            this.id = id;
            this.error = new Error(error.code,error.message,error.data);
        }
    }

    public class RpcErrors
    {
        public string jsonrpc { get; set; }
        public string id { get; set; }
        public Error error { get; set; }

        public RpcErrors(string id, string jsonrpc, Error error)
        {
            this.jsonrpc = jsonrpc;
            this.id = id;
            this.error = new Error(error.code, error.message, error.data);
        }
    }
} 