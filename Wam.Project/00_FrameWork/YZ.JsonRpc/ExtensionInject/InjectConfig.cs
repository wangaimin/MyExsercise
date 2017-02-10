using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace YZ.JsonRpc.ExtensionInject
{
    [XmlRoot("inject")]
    public class InjectConfig
    {
        #region single instance methods
        private static InjectConfig currentConfig;
        private static readonly object configLocker = new object();
        private static bool isLoadInjectConfig = false;
        public static InjectConfig Current
        {
            get
            {
                lock (configLocker)
                {
                    if (isLoadInjectConfig)
                        return currentConfig;

                    currentConfig = ReadConfig();
                    isLoadInjectConfig = true;
                    return currentConfig;
                }
            }
        }
        #endregion

        [XmlElement("group")]
        public List<InjectGroup> Groups { get; set; }
        
        private static InjectConfig ReadConfig()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\jsonrpc-Inject.config"))
            {
                string xmlContent = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\jsonrpc-inject.config");

                using (TextReader textReader = (TextReader) new StringReader(xmlContent))
                    currentConfig = new XmlSerializer(typeof (InjectConfig)).Deserialize(textReader) as InjectConfig;

                if (currentConfig != null && currentConfig.Groups != null)
                {
                    foreach (InjectGroup group in currentConfig.Groups)
                    {
                        if (group.Profiles != null)
                        {
                            group.Profiles.ForEach(p =>
                            {
                                p.ParentGroup = group;
                                if (string.IsNullOrWhiteSpace(p.HandleMethod))
                                    p.HandleMethod = group.HandleMethod;
                            });
                            
                        }
                    }
                }

                return currentConfig;
            }
            
            return null;
        }
    }

    public class InjectGroup
    {
         [XmlElement("add")]
        public List<InjectProfile> Profiles { get; set; }

         [XmlAttribute("handleMethod")]
         public string HandleMethod { get; set; }
    }


    public class InjectProfile
    {
        [XmlAttribute("injectMethod")]
        public string InjectMethod { get; set; }

        [XmlAttribute("handleMethod")]
        public string HandleMethod { get; set; }

        [XmlElement("param")]
        public Param[] ParamExprs { get; set; }

        [XmlIgnore]
        public InjectGroup ParentGroup { get; set; }
    }

    public class Param
    {
        [XmlAttribute("expr")]
        public string Expr { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}