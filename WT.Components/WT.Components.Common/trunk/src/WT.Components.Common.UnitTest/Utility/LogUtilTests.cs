using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WT.Components.Common.Utility;

namespace WT.Components.Common.UnitTest.Utility
{
    [TestClass]
    public class LogUtilTests
    {
        [TestMethod]
        public void LogTest()
        {
            LogUtil.Info("info", "LogInfoTest", "info");
        }

        [TestMethod]
        public void LogWarnTest()
        {
            LogUtil.Warning("warn", "LogWarnTest", "warn");
        }

        [TestMethod]
        public void LogErrorTest()
        {
            LogUtil.Error("error", "LogErrorTest", "error");
        }

        [TestMethod]
        public void InfoFormatTest()
        {
            LogUtil.InfoFormat("a:{0},b:{1}", "a", "b");
        }

        [TestMethod]
        public void WarningFormatTest()
        {
            LogUtil.WarningFormat("a:{0},b:{1}", "a", "b");
        }

        [TestMethod]
        public void ErrorFormatTest()
        {
            LogUtil.ErrorFormat("a:{0},b:{1}", "a", "b");
        }

        [TestMethod]
        public void EventLogTest()
        {
            LogUtil.EventLog(new Exception("aa"));

            LogUtil.EventLog(new Exception("aa"), "ee");
        }
    }
}
