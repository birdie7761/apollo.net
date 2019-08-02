using System;
using System.Collections.Generic;
using System.Text;
using Tests;

namespace Com.Ctrip.Framework.Apollo
{
    class Program
    {
        private static readonly IConfig config = ApolloConfigurationManager.GetAppConfig().GetAwaiter().GetResult();
        static public void Main(string[] args)
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
}
