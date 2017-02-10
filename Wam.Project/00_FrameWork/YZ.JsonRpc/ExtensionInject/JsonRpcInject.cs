using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using YZ.JsonRpc.Reflection;
using JsonRequest = YZ.JsonRpc.JsonRequest;
using JsonResponse = YZ.JsonRpc.JsonResponse;

namespace YZ.JsonRpc.ExtensionInject
{
    public class JsonRpcInject
    {
        private static GenericInvoker rpcCallInvoker;
        private static object rpcCallInvokerSyncLocker = new object();

        public void PreProcess(JsonRequest rpc, object context)
        {
        }

        public void CompletedProcess(JsonRequest jsonRequest, JsonResponse jsonResponse)
        {
            if (jsonResponse.Error != null)
                return;

            var profiles = FindInjectProfiles(jsonRequest.Method);
            if (profiles != null && profiles.Count > 0)
            {
                foreach (InjectProfile profile in profiles)
                {
                    // 异步调用切入服务，但需要附属到主任务 （TaskCreationOptions.AttachedToParent），
                    // 否则可能出现主任务退出了，而异步任务被强制中止
                    Task.Factory.StartNew(
                        CallHandleService, new
                        {
                            JsonRequest = jsonRequest,
                            JsonResponse = jsonResponse,
                            Profile = profile
                        }, TaskCreationOptions.AttachedToParent
                        );

                }
            }
        }

        private void CallHandleService(dynamic state)
        {
            InjectProfile profile = state.Profile;
            JsonRequest jsonRequest = state.JsonRequest;
            JsonResponse jsonResponse = state.JsonResponse;


            var serviceParams = new List<object>();
            if (profile.ParamExprs != null)
            {
                foreach (var param in profile.ParamExprs)
                    serviceParams.Add(JsonRpcExpr.ConvertExprToJToken(param.Expr, jsonRequest, jsonResponse));
            }
            //var rpcInstance = Activator.CreateInstance(Type.GetType("YZ.JsonRpc.Client.Rpc, YZ.JsonRpc.Client"));
            
            var invoker = GetRpcCallInvoker();

            invoker(null, new object[] { profile.HandleMethod, serviceParams.ToArray() });
            
        }

        private static GenericInvoker GetRpcCallInvoker()
        {
            lock (rpcCallInvokerSyncLocker)
            {
                return rpcCallInvoker ?? (rpcCallInvoker = Reflection.DynamicMethods.GenericMethodInvokerMethod(
                Type.GetType("YZ.JsonRpc.Client.Rpc, YZ.JsonRpc.Client"),
                "Call",
                new Type[] {typeof (object)},
                new Type[] {typeof (string), typeof (object[])}));
            }
        }


        private List<InjectProfile> FindInjectProfiles(string method)
        {
            List<InjectProfile> profiles = new List<InjectProfile>();

            if (InjectConfig.Current != null && InjectConfig.Current.Groups != null)
            {
                foreach (InjectGroup group in InjectConfig.Current.Groups)
                {
                    profiles.AddRange(
                        group.Profiles.FindAll(
                            m => string.Equals(m.InjectMethod, method, StringComparison.InvariantCultureIgnoreCase)));
                }

            }

            return profiles;
        }
    }
}