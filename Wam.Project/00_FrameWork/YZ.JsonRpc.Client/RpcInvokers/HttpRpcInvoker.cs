using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace YZ.JsonRpc.Client.RpcInvokers
{
    internal class HttpRpcInvoker : RpcInvoker
    {

        public static bool EnabledGzip = true;

        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 4096;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }


        internal override JsonResponse<T> Invoke<T>(JsonRequest jsonRpc)
        {
            HttpWebRequest req = null;

            int myId;

            if (jsonRpc.Id == null)
            {
                lock (idLock)
                {
                    myId = ++id;
                }

                jsonRpc.Id = myId.ToString();
            }

            req = WebRequest.Create(new Uri(ServiceAddress + "?callid=" + jsonRpc.Id.ToString() + "&method=" + jsonRpc.Method)) as HttpWebRequest;
            req.KeepAlive = false;
            req.Proxy = null;
            req.Method = "Post";
            req.ContentType = "application/json-rpc";
            if (Rpc.GlobalContextSet != null)
                Rpc.GlobalContextSet(req.Headers);
            MergeContextValues(req.Headers, this.Option);

            if (this.Option != null && this.Option.ForceWriteDB)
            {
                req.Headers["x-rpc-forcewritedb"] = "true";
            }
            if (this.Option != null && this.Option.Timeout > 0)
            {
                req.Timeout = this.Option.Timeout;
            }
            else
            {
                req.Timeout = 400000;
            }
            req.ReadWriteTimeout = req.Timeout;
            if (EnabledGzip)
            {
                req.Headers["Accept-Encoding"] = "gzip";
            }

            var stream = new StreamWriter(req.GetRequestStream());
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpc);
            stream.Write(json);
            stream.Close();

            var resp = req.GetResponse();
            string sstream;

            string contentEncoding = resp.Headers["Content-Encoding"];

            if (contentEncoding != null && contentEncoding.Contains("gzip"))
            {
                var mstream = CopyAndClose(resp.GetResponseStream());

                using (var gstream = new GZipStream(mstream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gstream, UTF8Encoding))
                    {
                        sstream = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (var rstream = new StreamReader(CopyAndClose(resp.GetResponseStream())))
                {
                    sstream = rstream.ReadToEnd();
                }
            }
            resp.Close();
            JsonResponse<T> rjson = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonResponse<T>>(sstream);

            if (rjson == null)
            {
                if (!string.IsNullOrEmpty(sstream))
                {
                    JObject jo = Newtonsoft.Json.JsonConvert.DeserializeObject(sstream) as JObject;
                    throw new Exception(jo["Error"].ToString());
                }
                else
                {
                    throw new Exception("Empty response");
                }
            }

            return rjson;
        }

        internal override List<JsonResponse> BatchInvoke(List<JsonRequest> jsonRpcList)
        {
            HttpWebRequest req = null;

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

            req = WebRequest.Create(new Uri(ServiceAddress)) as HttpWebRequest;
            req.KeepAlive = false;
            req.Proxy = null;
            req.Method = "Post";
            req.ContentType = "application/json-rpc";
            if (Rpc.GlobalContextSet != null)
                Rpc.GlobalContextSet(req.Headers);
            MergeContextValues(req.Headers, this.Option);

            if (this.Option != null && this.Option.ForceWriteDB)
            {
                req.Headers["x-rpc-forcewritedb"] = "true";
            }
            if (this.Option != null && this.Option.Timeout > 0)
            {
                req.Timeout = this.Option.Timeout;
            }
            else
            {
                req.Timeout = 400000;
            }
            req.ReadWriteTimeout = req.Timeout;
            if (EnabledGzip)
            {
                req.Headers["Accept-Encoding"] = "gzip";
            }

            var stream = new StreamWriter(req.GetRequestStream());
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonRpcList);
            stream.Write(json);
            stream.Close();

            var resp = req.GetResponse();
            string sstream;

            string contentEncoding = resp.Headers["Content-Encoding"];

            if (contentEncoding != null && contentEncoding.Contains("gzip"))
            {
                var mstream = CopyAndClose(resp.GetResponseStream());

                using (var gstream = new GZipStream(mstream, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gstream, UTF8Encoding))
                    {
                        sstream = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (var rstream = new StreamReader(CopyAndClose(resp.GetResponseStream())))
                {
                    sstream = rstream.ReadToEnd();
                }
            }
            resp.Close();

            List<JsonResponse> rjson = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JsonResponse>>(sstream);

            if (rjson == null)
            {
                if (!string.IsNullOrEmpty(sstream))
                {
                    JObject jo = Newtonsoft.Json.JsonConvert.DeserializeObject(sstream) as JObject;
                    throw new Exception(jo["Error"].ToString());
                }
                else
                {
                    throw new Exception("Empty response");
                }
            }

            return rjson;
        }

    }
}
