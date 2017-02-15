using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wam.Utility;
using System.Reflection;
using System.IO;
using Wam.Utility.String;
using Wam.Utility.DataAccess.Entity;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wam.Utility.Cache;
using YZ.JsonRpc.Client;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            child c = new child();
            c.setAddress("温江");
        }


        /// <summary>
        /// 测试反射
        /// </summary>
        [TestMethod]
        public void TestAssembly()
        {
            var assem = Assembly.Load("Wam.Service");
            var types = assem.GetType();
            var regMethod = typeof(child).GetMethod("Register");
            //  regMethod.Invoke("",)

            //Delegate.CreateDelegate()
        }

        /// <summary>
        /// 测试Func
        /// </summary>
        [TestMethod]
        public void TestFunc()
        {
              string fullName = Do(getFullName);
        }


        /// <summary>
        /// 测试缓存
        /// </summary>
        [TestMethod]
        public void TestCache() {
        
            LocalMemoryCache lmc = new LocalMemoryCache();
            var data = lmc.GetWithCache("SqlConfig", _LoadConfig, 3600);
            var data2 = lmc.GetWithCache("SqlConfig", _LoadConfig, 3600);
        }

        [TestMethod]
        public void TestRpc() {
          string result=  Rpc.Call<string>("WamService.UserService.getName", "Gene");

        }





        public string getFullName(string namePart1, string namePart2)
        {

            return namePart1 + namePart2;
        }

        public string Do(Func<string, string, string> handler)
        {

            return handler("王", "爱民");
        }

        private List<SQL> _LoadConfig()
        {
            Regex regex = new Regex(@"@\w*", RegexOptions.IgnoreCase);
            List<SQL> list = new List<SQL>();


            string filePath = @"C:\Users\admin\Source\Repos\NewRepo\Wam.Project\UnitTestProject2\Configuration\Data\Shop.config";
            if (File.Exists(filePath))
            {
                SQLConfig sqlConfig = SerializeHelper.LoadFromXml<SQLConfig>(filePath);
                if (sqlConfig.SQLList != null)
                {
                    foreach (SQL sql in sqlConfig.SQLList)
                    {
                        sql.ParameterNameList = new List<string>();

                        MatchCollection matches = regex.Matches(sql.Text.Trim());
                        if (matches != null && matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                if (!sql.ParameterNameList.Exists(f => f.Trim().ToLower() == match.Value.Trim().ToLower()))
                                {
                                    sql.ParameterNameList.Add(match.Value);
                                }
                            }
                        }

                        //if (sql.TimeOut == 0)
                        //{
                        //    DBConnection conn = DBConfigHelper.ConfigSetting.DBConnectionList.Find(f => f.Key.ToLower().Trim() == sql.ConnectionKey.ToLower().Trim());
                        //    if (conn != null)
                        //    {
                        //        sql.TimeOut = conn.TimeOut;
                        //    }
                        //    else
                        //    {
                        //        sql.TimeOut = 180;
                        //    }

                        //}
                    }
                    list.AddRange(sqlConfig.SQLList);
                }
            }

            return list;

        }
    }

}

