using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wam.Utility;

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
    }
}
