using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Wam.Utility.DataAccess.Entity
{
    [Serializable]
    public class DBConfig
    {        
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public List<DBConnection> DBConnectionList { get; set; }

        /// <summary>
        /// SQL语句的文件列表
        /// </summary>
        [XmlArrayItem("SQLFile")]
        public List<string> SQLFileList { get; set; }

        /// <summary>
        /// 数据库连接路由
        /// </summary>
        public DBConnectionRoute DBConnectionRoute { get; set; }
    }

    [Serializable]
    public class DBConnection
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlElement]
        public string ConnectionString { get; set; }

        /// <summary>
        /// 数据库类型：SqlServer，Access，MySQL 
        /// </summary>
        [XmlAttribute]
        public string DBType { get; set; }

        /// <summary>
        /// 当前的数据提供者对象
        /// </summary>
        [XmlIgnore]
        public ProviderType DBProviderType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DBType))
                {
                    return ProviderType.OleDb;
                }
                else if (DBType.Trim().ToUpper() == "SQLSERVER")
                {
                    return ProviderType.SqlServer;
                }
                else if (DBType.Trim().ToUpper() == "MYSQL")
                {
                    return ProviderType.MySql;
                }              
                else
                {
                    return ProviderType.OleDb;
                }
            }
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        [XmlAttribute]
        public int TimeOut { get; set; }

        /// <summary>
        /// 是否加密，只有为Y或者YES时，才表示要加密
        /// </summary>
        [XmlAttribute]
        public string IsEncrypt { get; set; }

        [XmlAttribute]
        public string GroupID { get; set; }

        [XmlAttribute]
        public string IsWrite { get; set; }

        [XmlAttribute]
        public string ExcludeTransaction { get; set; }
    }



    [Serializable]
    public class DBConnectionRoute
    {
        public DBConnectionRoute()
        {
            RouteConnections = new List<RouteConnection>();
        }

        [XmlElement("DBConnection")]
        public List<RouteConnection> RouteConnections { get; set; }

        [Serializable]
        public class RouteConnection
        {
            public RouteConnection()
            {
                Nodes = new List<Node>();
            }

            [XmlAttribute]
            public string Key { get; set; }

            [XmlElement("Node")]
            public List<Node> Nodes { get; set; }

            [XmlAttribute]
            public string IsWrite { get; set; }
        }

        [Serializable]
        public class Node : DBConnection
        {
            public Node()
            {
                //BalanceType= BalanceType.Master;
                BalanceWeight = 1;
            }

            //[XmlAttribute]
            //public string Key { get; set; }

            [XmlAttribute]
            public string SplitDbCode { get; set; }

            //TODO：暂不支持处理
            //[XmlAttribute]
            //public BalanceType BalanceType { get; set; }

            [XmlAttribute]
            public double BalanceWeight { get; set; }

            //[XmlText]
            //public string ConnectionString { get; set; }

            ///// <summary>
            ///// 是否加密，只有为Y或者YES时，才表示要加密
            ///// </summary>
            //[XmlAttribute]
            //public string IsEncrypt { get; set; }
        }
    }
    public enum ProviderType
    {
        SqlServer,
        MySql, 
        OleDb
    }

    public enum BalanceType
    {
        Master,
        Slave,
        Backup
    }
}
