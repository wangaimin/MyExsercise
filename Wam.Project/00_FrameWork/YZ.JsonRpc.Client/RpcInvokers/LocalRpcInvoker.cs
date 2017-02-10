using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace YZ.JsonRpc.Client.RpcInvokers
{
    internal class LocalRpcInvoker : RpcInvoker
    {
        internal override JsonResponse<T> Invoke<T>(JsonRequest jsonRpc)
        {
            int myId;

            if (jsonRpc.Id == null)
            {
                lock (idLock)
                {
                    myId = ++id;
                }

                jsonRpc.Id = myId.ToString();
            }

            var jsonReqStr = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpc);
            var contextNameValueCollection = new NameValueCollection();
            if (this.Option != null && this.Option.ForceWriteDB)
            {
                contextNameValueCollection["x-rpc-forcewritedb"] = "true";
            }
            if (Rpc.GlobalContextSet != null)
                Rpc.GlobalContextSet(contextNameValueCollection);
            MergeContextValues(contextNameValueCollection, this.Option);

            var jsonRespStr = LocalRpcRun(jsonReqStr, contextNameValueCollection);
            JsonResponse<T> rjson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonResponse<T>>(jsonRespStr);

            if (rjson == null)
            {
                if (!string.IsNullOrEmpty(jsonRespStr))
                {
                    JObject jo = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonRespStr) as JObject;
                    throw new Exception(jo["Error"].ToString());
                }
                else
                {
                    throw new Exception("Empty response");
                }
            }

            return rjson;
        }

        private static string LocalRpcRun(string jsonReqStr, NameValueCollection contextNameValueCollection)
        {
            var jsonRpcProcessor = Type.GetType("YZ.JsonRpc.JsonRpcProcessor, YZ.JsonRpc");

            if (jsonRpcProcessor == null)
                throw new TypeUnloadedException("找不到YZ.JsonRpc.JsonRpcProcessor，是否YZ.JsonRpc.dll没有被引用。");

            var task = jsonRpcProcessor.GetMethod("Process",
                new Type[] { typeof(string), typeof(object) })
                .Invoke(null, new object[] { (object)jsonReqStr, (object)contextNameValueCollection })
                as Task<string>;
            var jsonRespStr = task.Result;
            return jsonRespStr;
        }

        internal override List<JsonResponse> BatchInvoke(List<JsonRequest> jsonRpcList)
        {
            //WebRequest req = null;

            foreach (JsonRequest jsonRpc in jsonRpcList)
            {
                int myId;

                if (jsonRpc.Id == null)
                {
                    lock (idLock)
                    {
                        myId = ++id;
                    }

                    jsonRpc.Id = myId.ToString();
                }
            }


            var jsonReqStr = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpcList);
            var contextNameValueCollection = new NameValueCollection();
            if (this.Option != null && this.Option.ForceWriteDB)
            {
                contextNameValueCollection["x-rpc-forcewritedb"] = "true";
            }
            if (Rpc.GlobalContextSet != null)
                Rpc.GlobalContextSet(contextNameValueCollection);
            MergeContextValues(contextNameValueCollection, this.Option);

            var jsonRespStr = LocalRpcRun(jsonReqStr, contextNameValueCollection);
            List<JsonResponse> rjson = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonResponse>>(jsonRespStr);
            return rjson;
        }
    }
}
