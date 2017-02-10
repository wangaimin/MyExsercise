using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace YZ.JsonRpc.ExtensionInject
{
    public class JsonRpcExpr
    {
        public static int? ConvertExprToInt32(string jsonExpr, JsonRequest jsonRequest, JsonResponse jsonResponse)
        {
            var jtoken = ConvertExprToJToken(jsonExpr, jsonRequest, jsonResponse);
            if (jtoken == null || string.IsNullOrWhiteSpace(jtoken.ToString()))
                return new int?();
            else
                return Convert.ToInt32(jtoken.ToString());
        }


        public static string ConvertExprToString(string jsonExpr, JsonRequest jsonRequest, JsonResponse jsonResponse)
        {
            var jtoken = ConvertExprToJToken(jsonExpr, jsonRequest, jsonResponse);
            if (jtoken == null)
                return string.Empty;
            else
                return jtoken.ToString();
        }

        /// <summary>
        /// 转换表达式中的值,
        /// </summary>
        /// <param name="jsonExpr"></param>
        /// <param name="jsonRequest"></param>
        /// <param name="jsonResponse"></param>
        /// <returns></returns>
        public static JToken ConvertExprToJToken(string jsonExpr, JsonRequest jsonRequest, JsonResponse jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonExpr))
                return string.Empty;
            bool isGetResult = false;
            bool isJsonRequest = false;
            bool isJsonResponse = false;
            var jsonexpMethodSymbol = "$method"; // //json表达式字符串中表示取result的关键字
            var jsonexpResultSymbol = "$result"; // //json表达式字符串中表示取result的关键字
            var jsonexpJsonRequestSymbol = "$jsonRequest"; // //json表达式字符串中表示取JsonRequest的关键字
            var jsonexpJsonResponseSymbol = "$jsonResponse"; // //json表达式字符串中表示取JsonResponse的关键字

            if (jsonExpr == jsonexpMethodSymbol)
            {
                return JValue.FromObject(jsonRequest.Method);
            }
            
            if (jsonExpr.StartsWith(jsonexpResultSymbol))
            {
                isGetResult = true;
                jsonExpr = jsonExpr.Remove(0, jsonexpResultSymbol.Length);
                jsonExpr = jsonExpr.TrimStart('.');
            }
            if (jsonExpr.StartsWith(jsonexpJsonRequestSymbol))
            {
                isJsonRequest = true;
                jsonExpr = jsonExpr.Remove(0, jsonexpJsonRequestSymbol.Length);
                jsonExpr = jsonExpr.TrimStart('.');
            }
            if (jsonExpr.StartsWith(jsonexpJsonResponseSymbol))
            {
                isJsonResponse = true;
                jsonExpr = jsonExpr.Remove(0, jsonexpJsonResponseSymbol.Length);
                jsonExpr = jsonExpr.TrimStart('.');
            }

            JToken resultJtoken = null;
            if (isGetResult || isJsonRequest || isJsonResponse)
            {

                object varObj = null;
                if (isGetResult)
                {
                    varObj = jsonResponse.Result;
                }
                else if(isJsonRequest)
                {
                    varObj = jsonRequest;
                }
                else if(isJsonResponse)
                {
                    varObj = jsonResponse;
                }


                if (varObj == null)
                    return null;

                JToken jToken = null;
                if (varObj.GetType().IsValueType)
                {
                    jToken = JValue.FromObject(varObj);
                }
                else
                {
                    jToken = JObject.FromObject(varObj);
                }
                resultJtoken = jToken.SelectToken(jsonExpr);
            }
            else
            {
                var metadata = Handler.DefaultHandler.MetaData.Services[jsonRequest.Method];
                bool isJObject = jsonRequest.Params is Newtonsoft.Json.Linq.JObject;
                bool isJArray = jsonRequest.Params is Newtonsoft.Json.Linq.JArray;
                object[] parameters = null;
                bool expectsRefException = false;
                var metaDataParamCount = metadata.parameters.Count(x => x != null);

                var getCount = jsonRequest.Params as ICollection;
                var loopCt = 0;

                if (getCount != null)
                {
                    loopCt = getCount.Count;
                }

                var paramCount = loopCt;
                if (paramCount == metaDataParamCount - 1 && metadata.parameters[metaDataParamCount - 1].ObjectType.Name.Contains(typeof(JsonRpcException).Name))
                {
                    paramCount++;
                    expectsRefException = true;
                }
                parameters = new object[paramCount];

                JArray jArray;

                if (isJObject)
                {
                    jArray = new JArray();
                    var jo = jsonRequest.Params as Newtonsoft.Json.Linq.JObject;
                    var asDict = jo as IDictionary<string, Newtonsoft.Json.Linq.JToken>;
                    for (int i = 0; i < loopCt; i++)
                    {
                        if (asDict.ContainsKey(metadata.parameters[i].Name) == false)
                        {
                            throw new Exception(
                                string.Format("JsonRpc Inject Expression Named parameter '{0}' was not present.",
                                    metadata.parameters[i].Name)
                                );
                        }
                        jArray.Add(jo[metadata.parameters[i].Name]);
                    }
                }
                else
                {
                    jArray = jsonRequest.Params as JArray;
                }

                if (jArray != null)
                    resultJtoken = jArray.SelectToken(jsonExpr);
            }
            return resultJtoken;
        }

        public static string ReplaceExprText(string exprContent, JsonRequest jsonRequest, JsonResponse jsonResponse)
        {
            string result = exprContent;
            var macthes = Regex.Matches(exprContent, "{($.+?)}|{(\\[.+?)}");
            foreach (Match m in macthes)
            {
                var replaceValue = ReplaceExprText(m.Groups[1].Value, jsonRequest, jsonResponse);
                result = result.Replace(m.Groups[0].Value, replaceValue);
            }
            return exprContent;
        }
    }
}