using System.Runtime.CompilerServices;
using UnityEngine.Networking;

// Extension for UnityWebRequest to support async/await pattern.  
// Provides a custom awaiter for handling asynchronous web requests.
//Note :- not getting used for now
namespace OktoSDK
{
    public static class UnityWebRequestExtensions
    {
        public struct UnityWebRequestAwaiter : INotifyCompletion
        {
            private UnityWebRequestAsyncOperation asyncOp;
            private bool isDone;

            public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
            {
                this.asyncOp = asyncOp;
                isDone = false;
            }

            public bool IsCompleted => asyncOp.isDone;

            public void OnCompleted(System.Action continuation)
            {
                asyncOp.completed += _ => continuation();
            }

            public UnityWebRequestAsyncOperation GetResult()
            {
                return asyncOp;
            }
        }

        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
} 