using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace YZ.JsonRpc.Client.RpcInvokers
{
    internal abstract class RpcInvoker
    {
        protected static Encoding UTF8Encoding = new UTF8Encoding(false);

        protected static object idLock = new object();
        protected static int id = 0;
        public RpcOption Option { get; set; }
        internal string ServiceAddress { get; set; }

        internal JsonResponse<T> Invoke<T>(string method, params object[] args)
        {
            var req = new JsonRequest
            {
                Method = method,
                Params = args
            };

            return Invoke<T>(req);
        }

        internal JsonResponse<T> InvokeWithDeclaredParams<T>(string method, object args)
        {
            var req = new JsonRequest
            {
                Method = method,
                Params = args
            };
            return Invoke<T>(req);
        }

        internal abstract JsonResponse<T> Invoke<T>(JsonRequest jsonRpc);
        internal abstract List<JsonResponse> BatchInvoke(List<JsonRequest> jsonRpcList);

        protected static void MergeContextValues(NameValueCollection globalValues, RpcOption option)
        {
            if(option == null|| option.ContextValues == null)
                return;

            foreach (string key in option.ContextValues.Keys)
                globalValues[key] = option.ContextValues[key];
        }
    }
}