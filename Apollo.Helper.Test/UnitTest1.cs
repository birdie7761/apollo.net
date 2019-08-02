using Com.Ctrip.Framework.Apollo;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{

    public class Tests
    {
        private static readonly IConfig config = ApolloConfigurationManager.GetAppConfig().GetAwaiter().GetResult();
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            ApolloConfigHelper apolloConfigHelper = new ApolloConfigHelper(config);
            var map = apolloConfigHelper.GetProperty<Dictionary<UInt16, IBEETDZPrinter>>("PrinterMap", ApolloValueTypes.JSON);
            //var ddd = new Dictionary<UInt16, IBEETDZPrinter> { { 1, new IBEETDZPrinter { Printer = 2, SessionName = "ddd" } } };
            //Console.Out.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(ddd));
            //var Param = new IBEETDZParam();
            //Param.PrinterNo = "1";
            Console.Out.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(map));
        }
    }

    public class IBEETDZPrinter
    {
        public string SessionName { get; set; }
        public UInt16 Printer { get; set; }
    }
}
