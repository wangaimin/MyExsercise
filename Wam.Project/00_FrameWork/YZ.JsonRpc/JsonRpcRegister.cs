using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;

namespace YZ.JsonRpc
{
    public class JsonRpcRegister
    {
        private static readonly List<object> jsonrpcInstances = new List<object>();
        private static readonly object loadLocker = new object();
        private static bool isLoadedConfig = false;
        
        public static void LoadFromConfig()
        {
            lock (loadLocker)
            {
                if (isLoadedConfig)
                    return;
                var jsonrpcConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\jsonrpc.config";
                if (File.Exists(jsonrpcConfigPath))
                {
                    DoLoadFromConfig();
                }
                isLoadedConfig = true;
            }
        }

        private static void DoLoadFromConfig()
        {
            var jsonrpcConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\jsonrpc.config";

            XmlDocument jsonrpcDoc = new XmlDocument();
            jsonrpcDoc.Load(jsonrpcConfigPath);

            foreach (XmlNode assemblyNode in jsonrpcDoc["jsonrpc"]["serviceAssemblies"].ChildNodes)
            {
                if (!"add".Equals(assemblyNode.Name, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                try
                {
                    var assemblyName = assemblyNode.Attributes["assembly"].Value;
                    var domain = assemblyNode.Attributes["domain"].Value;
                    var methodMode = assemblyNode.Attributes["methodMode"].Value;

                    var assem = Assembly.Load(assemblyName);
                    var typesWithHandlers = assem.GetTypes().Where(f => f.IsPublic);
                    foreach (Type typeWithHandlers in typesWithHandlers)
                    {
                        string sessionID = Handler.DefaultSessionId();
                        var regMethod = typeof (Handler).GetMethod("Register");

                        var item = typeWithHandlers;
                        var instance = assem.CreateInstance(typeWithHandlers.FullName);
                        List<MethodInfo> methods;
                        bool isMethodModeByAttribute = methodMode != null &&
                                                       "attribute".Equals(methodMode, StringComparison.InvariantCultureIgnoreCase);

                        if (isMethodModeByAttribute)
                        {
                            methods =
                                item.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(m => m.GetCustomAttributes(typeof (JsonRpcMethodAttribute), false).Length > 0)
                                    .ToList();
                        }
                        else
                        {
                            methods = item.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
                        }

                        if (methods.Any())
                        {
                            jsonrpcInstances.Add(instance);
                        }

                        foreach (var meth in methods)
                        {
                            if (meth.Name.StartsWith("get_") || meth.Name.StartsWith("set_"))
                                continue;

                            Dictionary<string, Type> paras = new Dictionary<string, Type>();
                            Dictionary<string, object> defaultValues = new Dictionary<string, object>();
                            // dictionary that holds default values for optional params.

                            var paramzs = meth.GetParameters();

                            List<Type> parameterTypeArray = new List<Type>();
                            for (int i = 0; i < paramzs.Length; i++)
                            {
                                // reflection attribute information for optional parameters
                                //http://stackoverflow.com/questions/2421994/invoking-methods-with-optional-parameters-through-reflection
                                paras.Add(paramzs[i].Name, paramzs[i].ParameterType);

                                if (paramzs[i].IsOptional) // if the parameter is an optional, add the default value to our default values dictionary.
                                    defaultValues.Add(paramzs[i].Name, paramzs[i].DefaultValue);
                            }

                            var resType = meth.ReturnType;
                            paras.Add("returns", resType); // add the return type to the generic parameters list.

                            if (isMethodModeByAttribute)
                            {
                                var atdata = meth.GetCustomAttributes(typeof (JsonRpcMethodAttribute), false);
                                foreach (JsonRpcMethodAttribute handlerAttribute in atdata)
                                {
                                    string methodName;
                                    if (handlerAttribute.JsonMethodName == string.Empty)
                                    {
                                        methodName = typeWithHandlers.Name + "." + meth.Name;
                                        if (!string.IsNullOrWhiteSpace(domain))
                                        {
                                            methodName = domain + "." + methodName;
                                        }
                                    }
                                    else
                                    {
                                        methodName = handlerAttribute.JsonMethodName == string.Empty ? meth.Name : handlerAttribute.JsonMethodName;
                                    }
                                    var newDel = Delegate.CreateDelegate(System.Linq.Expressions.Expression.GetDelegateType(paras.Values.ToArray()),
                                        instance /*Need to add support for other methods outside of this instance*/, meth);
                                    var handlerSession = Handler.GetSessionHandler(sessionID);
                                    regMethod.Invoke(handlerSession, new object[] {methodName, newDel});
                                    try
                                    {
                                        handlerSession.MetaData.AddService(methodName, paras, defaultValues);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Register method " + methodName + " error and skiped, " + ex.ToString());
                                    }
                                }
                            }
                            else
                            {
                                {
                                    var methodName = typeWithHandlers.Name + "." + meth.Name;
                                    if (!string.IsNullOrWhiteSpace(domain))
                                    {
                                        methodName = domain + "." + methodName;
                                    }

                                    var newDel = Delegate.CreateDelegate(System.Linq.Expressions.Expression.GetDelegateType(paras.Values.ToArray()),
                                        instance /*Need to add support for other methods outside of this instance*/, meth);
                                    var handlerSession = Handler.GetSessionHandler(sessionID);
                                    regMethod.Invoke(handlerSession, new object[] {methodName, newDel});
                                    try
                                    {
                                        handlerSession.MetaData.AddService(methodName, paras, defaultValues);
                                    }
                                    catch(Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Register method " + methodName + " error and skiped, " + ex.ToString());
                                    }
                                }
                            }
                        }
                    }

                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            isLoadedConfig = true;
        }
    }
}
