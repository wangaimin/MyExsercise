using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace YZ.JsonRpc.Client
{
    public delegate void RequestHandler(ref WebRequest request);

    public class Rpc
    {
        private static object idLock = new object();
        private static int id = 0;
        public Uri ServiceEndpoint = null;
        public static bool EnabledGzip = true;
        
        private static Encoding UTF8Encoding = new UTF8Encoding(false);
        private static RequestHandler requestHandler;

        /// <summary>
        /// The length of time, in milliseconds, default value is 100000 milliseconds(100 seconds).
        /// </summary>
        public int Timeout { get; set; }

        public static void SetRequestHandler(RequestHandler handler)
        {
            requestHandler += handler;
        }

        public Rpc()
        {
            
        }

        public Rpc(string serviceEndpoint)
        {
            ServiceEndpoint = new Uri(serviceEndpoint);
        }


        public Rpc(string serviceEndpoint, int timeout)
        {
            ServiceEndpoint = new Uri(serviceEndpoint);
            Timeout = timeout;
        }

        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 256;
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

        public JsonResponse<T> Invoke<T>(string method, params object[] args)
        {
            var req = new JsonRequest()
            {
                Method = method,
                Params = args
            };
            return Invoke<T>(req);
        }


        public JsonResponse<T> InvokeWithDeclaredParams<T>(string method, object args)
        {
            var req = new JsonRequest()
            {
                Method = method,
                Params = args
            };
            return Invoke<T>(req);
        }

        public JsonResponse<T> Invoke<T>(JsonRequest jsonRpc)
        {
            WebRequest req = null;
            try
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

                req = WebRequest.Create(new Uri(ServiceEndpoint, "?callid=" + jsonRpc.Id.ToString()));
                req.Proxy = null;
                req.Method = "Post";
                req.ContentType = "application/json-rpc";
                if (requestHandler != null)
                    requestHandler(ref req);
                if (Timeout > 0)
                {
                    req.Timeout = Timeout;
                }
                if (EnabledGzip)
                {
                    req.Headers["Accept-Encoding"] = "gzip";
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

        #region get service url from config

        private static string GetServiceUrlDomainFromConfig(string method)
        {
            var domain = ExtractDomainName(method);
            if (string.IsNullOrEmpty(domain))
                return null;
            return System.Configuration.ConfigurationManager.AppSettings["JsonRpcServiceUrl." + domain];
        }

        private static string ExtractDomainName(string method)
        {
            var elements = method.Split('.');
            return elements.Length >= 2 ? elements[0] : null;
        }

        private static string GetServiceUrlFromConfig<T>(string method)
        {
            var serviceUrlByDomain = GetServiceUrlDomainFromConfig(method);
            string serviceUrl = System.Configuration.ConfigurationManager.AppSettings["JsonRpcServiceUrl"];
            if (!string.IsNullOrWhiteSpace(serviceUrlByDomain))
                serviceUrl = serviceUrlByDomain;
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new Exception("Config setting JsonRpcServiceUrl is empty.");
            return serviceUrl;
        }

        #endregion

        /// <summary>
        /// 调用JSON-RPC服务（服务地址需要配置在App.config中的Setting下的JsonRpcServiceUrl的Key值）。
        /// </summary>
        /// <typeparam name="T">JSON-RPC方法返回的Result的对象类型。</typeparam>
        /// <param name="method">JSON-RPC方法名。</param>
        /// <param name="args">JSON-RPC方法接收的参数，此参数为可变数组</param>
        /// <returns></returns>
        public static T Call<T>(string method, params object[] args)
        {
            var rpc = new Rpc();
            return rpc.CallInvoke<T>(method, args);
        }

        /// <summary>
        /// 调用JSON-RPC服务（服务地址需要配置在App.config中的Setting下的JsonRpcServiceUrl的Key值）。
        /// </summary>
        /// <typeparam name="T">JSON-RPC方法返回的Result的对象类型。</typeparam>
        /// <param name="method">JSON-RPC方法名。</param>
        /// <param name="args">JSON-RPC方法接收的参数，此参数为可变数组</param>
        /// <returns></returns>
        public T CallInvoke<T>(string method, params object[] args)
        {
            if (ServiceEndpoint == null)
            {
                ServiceEndpoint = new Uri(GetServiceUrlFromConfig<T>(method));
            }
            var jresp = Invoke<T>(method, args);
            if (jresp.Error != null)
                throw jresp.Error;

            return jresp.Result;
        }

        /// <summary>
        /// 调用JSON-RPC服务（服务地址需要配置在App.config中的Setting下的JsonRpcServiceUrl的Key值）。
        /// </summary>
        /// <typeparam name="T">JSON-RPC方法返回的Result的对象类型。</typeparam>
        /// <param name="method">JSON-RPC方法名。</param>
        /// <param name="args">JSON-RPC方法接收的参数，此参数为可变数组</param>
        /// <returns></returns>
        public T CallInvokeWithDeclaredParams<T>(string method, object args)
        {
            if (ServiceEndpoint == null)
            {
                ServiceEndpoint = new Uri(GetServiceUrlFromConfig<T>(method));
            }
            JsonResponse<T> jresp = InvokeWithDeclaredParams<T>(method, args);

            if (jresp.Error != null)
                throw jresp.Error;

            return jresp.Result;
        }

        /// <summary>
        /// 调用JSON-RPC服务。
        /// </summary>
        /// <typeparam name="T">JSON-RPC方法返回的Result的对象类型。</typeparam>
        /// <param name="url">JSON-RPC服务地址</param>
        /// <param name="method">JSON-RPC方法名。</param>
        /// <param name="args">JSON-RPC方法接收的参数，此参数为可变数组</param>
        /// <returns></returns>
        public static T CallUrl<T>(string url, string method, params object[] args)
        {
            var rpc = new Rpc(url);
            return rpc.CallInvoke<T>(method, args);
        }

        /// <summary>
        /// 调用JSON-RPC服务（服务地址需要配置在App.config中的Setting下的JsonRpcServiceUrl的Key值）。
        /// </summary>
        /// <typeparam name="T">JSON-RPC方法返回的Result的对象类型。</typeparam>
        /// <param name="method">JSON-RPC方法名。</param>
        /// <param name="args">JSON-RPC方法接收的参数，此参数为可变数组</param>
        /// <returns></returns>
        public static T CallWithDeclaredParams<T>(string method, object args)
        {
            var rpc = new Rpc();
            return rpc.CallInvokeWithDeclaredParams<T>(method, args);
        }
    }
}