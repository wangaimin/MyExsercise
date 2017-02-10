using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Web;
using System.Xml;
using YZ.JsonRpc;

namespace YZ.JsonRpc.ExtensionInject
{
    public class MethodDescriptor
    {
        public static string GetCSharpRepresentation(Type t)
        {
            if (t.IsGenericType)
            {
                var genericArgs = t.GetGenericArguments().ToList();

                return GetCSharpRepresentation(t, true, genericArgs);
            }

            return t.Name;
        }

        public static string GetCSharpRepresentation(Type t, bool trimArgCount)
        {
            if (t.IsGenericType)
            {
                var genericArgs = t.GetGenericArguments().ToList();

                return GetCSharpRepresentation(t, trimArgCount, genericArgs);
            }

            return t.Name;
        }

        public static string GetCSharpRepresentation(Type t, bool trimArgCount, List<Type> availableArguments)
        {
            if (t.IsGenericType)
            {
                string value = t.Name;
                if (trimArgCount && value.IndexOf("`") > -1)
                {
                    value = value.Substring(0, value.IndexOf("`"));
                }

                if (t.DeclaringType != null)
                {
                    // This is a nested type, build the nesting type first
                    value = GetCSharpRepresentation(t.DeclaringType, trimArgCount, availableArguments) + "+" + value;
                }

                // Build the type arguments (if any)
                string argString = "";
                var thisTypeArgs = t.GetGenericArguments();
                for (int i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++)
                {
                    if (i != 0) argString += ", ";

                    argString += GetCSharpRepresentation(availableArguments[0], trimArgCount);
                    availableArguments.RemoveAt(0);
                }

                // If there are type arguments, add them with < >
                if (argString.Length > 0)
                {
                    value += "<" + argString + ">";
                }

                return value;
            }

            return t.Name;
        }


        public static dynamic GetMethodComment(MethodInfo mi)
        {
            string methodSign = "M:" + mi.DeclaringType.FullName + "." + mi.Name;

            string xmlPath = AppDomain.CurrentDomain.BaseDirectory + "\\bin\\" + mi.DeclaringType.Assembly.GetName().Name + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(xmlPath))
                return null;

            xmlDoc.Load(xmlPath);

            foreach (XmlNode memberNode in xmlDoc["doc"]["members"].ChildNodes)
            {
                if (memberNode.Attributes["name"].Value == methodSign || memberNode.Attributes["name"].Value.StartsWith(methodSign + "("))
                {
                    return memberNode.Clone();
                }
            }
            return null;
        }


        public static string GetMethodDescprtionComment(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return string.Empty;

            var xmlElement = xmlNode["summary"];
            if (xmlElement != null) return xmlElement.InnerText.Trim();

            return string.Empty;
        }

        public static string GetMethodReturnsComment(XmlNode xmlNode)
        {
            if (xmlNode == null)
                return string.Empty;

            var xmlElement = xmlNode["returns"];
            if (xmlElement != null) return xmlElement.InnerText;

            return string.Empty;
        }

        public static string GetMethodParameterComment(XmlNode xmlNode, string paramName)
        {
            if (xmlNode == null)
                return string.Empty;

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.Attributes != null && (childNode.Name == "param" && childNode.Attributes["name"].Value == paramName))
                    return childNode.InnerText;
            }
            return string.Empty;
        }
    }
}