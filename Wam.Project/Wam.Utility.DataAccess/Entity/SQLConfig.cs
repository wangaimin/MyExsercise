using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Wam.Utility.DataAccess.Entity
{
    [Serializable]
    [XmlRoot]
    public class SQLConfig
    {
        [XmlArrayItem("SQL")]
        public List<SQL> SQLList { get; set; }

    }

    [Serializable]
    public class SQL
    {
        [XmlAttribute]
        public string SQLKey { get; set; }

        [XmlAttribute]
        public string ConnectionKey { get; set; }

        [XmlElement]
        public string Text { get; set; }

        [XmlAttribute]
        public int TimeOut { get; set; }

        [XmlIgnore]
        public List<string> ParameterNameList { get; set; }

        [XmlAttribute]
        public string ForceWriteDB { get; set; }
    }
}
