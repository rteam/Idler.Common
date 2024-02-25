using Idler.Common.Core.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Idler.Common.Core.Test.Config
{
    [TestClass]
    public class LocalConfigurationAccessHelperTest
    {
        [TestMethod]
        public void Test_LocalConfigurationAccessHelper_Default_Constructor()
        {
            var helper = new LocalConfigurationAccessHelper<TestConfig>();
            Assert.IsNotNull(helper);
            Assert.AreEqual(typeof(TestConfig).Name, helper.ConfigName);
        }

        [TestMethod]
        public void Test_LocalConfigurationAccessHelper_With_Config_Name()
        {
            var helper = new LocalConfigurationAccessHelper<TestConfig>("TestName");
            Assert.IsNotNull(helper);
            Assert.AreEqual("TestName", helper.ConfigName);
        }
        //
        // [TestMethod]
        // public void Test_Save_Config()
        // {
        //     var helper = new LocalConfigurationAccessHelper<TestConfig>("TestSave");
        //     helper.ConfigEntity.TestField = "TestValue";
        //     helper.Save();
        //     Assert.IsTrue(File.Exists(helper.ConfigName));
        // }
        //
        // [TestMethod]
        // public void Test_Replace_Config()
        // {
        //     var helper = new LocalConfigurationAccessHelper<TestConfig>("TestReplace");
        //     var config = new TestConfig { TestField = "TestValue" }.ToConfig();
        //     helper.Replace(config);
        //     Assert.IsTrue(File.Exists(helper.ConfigName));
        //     Assert.AreEqual(config, File.ReadAllText(helper.ConfigPath));
        // }
        //
        // [TestMethod]
        // public void Test_Load_Config()
        // {
        //     var helper = new LocalConfigurationAccessHelper<TestConfig>("TestLoad");
        //     var config = new TestConfig { TestField = "TestValue" }.ToConfig();
        //     File.WriteAllText(helper.ConfigPath, config);
        //     helper.Load();
        //     Assert.AreEqual("TestValue", helper.ConfigEntity.TestField);
        // }

        public class TestConfig : AutoBaseConfig<TestConfig>
        {
            public string TestField { get; set; }
        }
    }
}