using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YZ.JsonRpc.AspNet
{
    public class SampleBuilder
    {
        private readonly Hashtable types = new Hashtable();

        public object BuildSampleObject(Type t)
        {
            if (t == typeof(void))
            {
                return null;
            }
            if (t == typeof(int) || t == typeof(long))
            {
                return 1; // "int";
            }
            else if (t == typeof(float) || t == typeof(double) || t == typeof(decimal))
            {
                return (float)1.9; // "float";
            }
            else if (t == typeof(bool))
            {
                return new bool(); // "bool";
            }
            else if (t == typeof(DateTime))
            {
                return DateTime.Now; // "datetime";
            }
            else if (t == typeof(TimeSpan))
            {
                return new TimeSpan(); // "timespan";
            }
            else if (t == typeof(string))
            {
                return string.Empty;
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var genericArgType = t.GetGenericArguments()[0];
                return BuildSampleObject(genericArgType);
            }
            else if (t.IsEnum)
            {
                return t.GetEnumValues().GetValue(0);
            }
            else if (t.IsValueType)
            {
                return null;
            }
            else if (t.IsInterface)
            {
                return null;
            }
            else if (t.IsArray)
            {
                string arrTypeName = t.FullName.Remove(t.FullName.Length - 2);
                if (arrTypeName.Contains("[]"))
                    return null;
                var arrType = Type.GetType(arrTypeName);
                return new[] { BuildSampleObject(arrType) };
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                var genericArgType = t.GetGenericArguments()[0];
                var list =  Activator.CreateInstance(t);
                t.InvokeMember("Add", BindingFlags.InvokeMethod, null, list, new object[] { BuildSampleObject(genericArgType) });
                return list;
            }
            else
            {
                try
                {
                    var complexObject = Activator.CreateInstance(t);
                    return FillSampleObject(ref complexObject);
                }
                catch
                {
                    return null;
                }
            }
        }

        public object FillSampleObject(ref object complexObject)
        {
            if (complexObject == null)
                return null;

            string complexTypeFullName = complexObject.GetType().FullName;

            string memberTypeFullName = string.Empty;
            var props = complexObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = complexObject.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                try
                {
                    if (types.Keys.Cast<string>().Any(key => key.Contains("*" + memberTypeFullName)))
                    {
                        continue;
                    }
                    //if (types.Contains(prop.PropertyType.FullName + "*" + complexTypeFullName))
                    //    return null;

                    if (prop.CanWrite)
                    {
                        memberTypeFullName = prop.PropertyType.FullName;
                        if (!string.IsNullOrWhiteSpace(memberTypeFullName))
                        {
                            string typeHashKey = complexTypeFullName + "*" + memberTypeFullName;
                            if (!types.Contains(typeHashKey))
                                types.Add(typeHashKey, "");
                        }

                        if (prop.Name == "PageIndex" && prop.PropertyType == typeof(int))
                        {
                            prop.SetValue(complexObject, 0, null);
                        }
                        else if (prop.Name == "PageSize" && prop.PropertyType == typeof(int))
                        {
                            prop.SetValue(complexObject, 20, null);
                        }
                        else if (prop.Name == "SortFields" && prop.PropertyType == typeof(string))
                        {
                            prop.SetValue(complexObject, string.Empty, null);
                        }
                        else
                        {
                            prop.SetValue(complexObject, BuildSampleObject(prop.PropertyType), null);
                        }
                    }
                }
                catch
                {
                }
            }
            foreach (var field in fields)
            {
                try
                {
                    if (types.Contains(field.FieldType.FullName + "*" + complexTypeFullName))
                        return null;


                    memberTypeFullName = field.FieldType.FullName;
                    if (!string.IsNullOrWhiteSpace(memberTypeFullName))
                    {
                        string typeHashKey = complexTypeFullName + "*" + memberTypeFullName;
                        if (!types.Contains(typeHashKey))
                            types.Add(typeHashKey, "");
                    }

                    field.SetValue(complexObject, BuildSampleObject(field.FieldType));
                }
                catch
                {
                }
            }

            return complexObject;
        }
    }
}