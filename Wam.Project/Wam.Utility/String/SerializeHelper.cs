using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Wam.Utility.String
{
    public static class SerializeHelper
    {

        #region Xml序列化

        public static T LoadFromXml<T>(string filePath)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return (T)serializer.Deserialize(fs);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static void SaveToXml(string filePath, object data)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(data.GetType());
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(fs, data);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
        }

        public static string ToXmlString(this object serialObject, bool removeDataRootXmlNode = false)
        {
            return XmlSerialize(serialObject, removeDataRootXmlNode);
        }

        public static string XmlSerialize(object serialObject, bool removeDataRootXmlNode = false)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer ser = new XmlSerializer(serialObject.GetType());
            using (TextWriter writer = new StringWriter(sb))
            {
                ser.Serialize(writer, serialObject);
                string xmlData = writer.ToString();
                if (removeDataRootXmlNode)
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(xmlData);
                    xmlData = doc.LastChild.InnerXml;
                }
                return xmlData;
            }
        }

        public static object XmlDeserialize(string str, Type type)
        {
            XmlSerializer mySerializer = new XmlSerializer(type);
            using (TextReader reader = new StringReader(str))
            {
                return mySerializer.Deserialize(reader);
            }
        }

        public static T XmlDeserialize<T>(string str)
        {
            return (T)XmlDeserialize(str, typeof(T));
        }

        #endregion Xml序列化

        #region 二进制序列化

        public static string ToBinaryBase64String(this object serialObject)
        {
            return BinarySerialize(serialObject);
        }

        public static string BinarySerialize(object serialObject)
        {
            if (serialObject == null)
            {
                return string.Empty;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, serialObject);
                ms.Position = 0;
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static object BinaryDeserialize(string str)
        {
            if (str == null || str.Trim().Length <= 0)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(str)))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        public static T BinaryDeserialize<T>(string str)
        {
            return (T)BinaryDeserialize(str);
        }

        #endregion 二进制序列化

      




        #region Json序列化 - DataContractJsonSerializer，不支持对匿名类的序列化

        /// <summary>
        /// 通过DataContractJsonSerializer，不支持对匿名类的序列化
        /// </summary>
        /// <param name="serialObject"></param>
        /// <returns></returns>
        private static string ToJsonString1(this object serialObject)
        {
            return JsonSerialize1(serialObject);
        }

        /// <summary>
        /// 通过DataContractJsonSerializer，不支持对匿名类的序列化
        /// </summary>
        /// <param name="serialObject"></param>
        /// <returns></returns>
        private static string JsonSerialize1(object serialObject)
        {
            if (serialObject == null)
            {
                return string.Empty;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(serialObject.GetType());
                serializer.WriteObject(stream, serialObject);
                byte[] s = stream.ToArray();
                return Encoding.UTF8.GetString(s, 0, s.Length);
            }
        }


        #endregion Json序列化

        #region Json序列化 - JavaScriptSerializer，可支持对匿名类的序列化

        /// <summary>
        /// 通过JavaScriptSerializer，可支持对匿名类的序列化
        /// </summary>
        /// <param name="serialObject"></param>
        /// <returns></returns>
        private static string ToJsonString2(this object serialObject)
        {
            return JsonSerialize2(serialObject);
        }

        /// <summary>
        /// 通过JavaScriptSerializer，可支持对匿名类的序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete]
        public static string JsonSerialize2(object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            return serializer.Serialize(obj);
        }


        #endregion

        public static T DeepClone<T>(T t)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, t);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        #region Json Serialize/Deserialize by JSON.NET

        public static string JsonSerializeFixed(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            });
        }

        public static T JsonDeserializeFixed<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object JsonDeserializeFixed(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        #endregion
    }
}
