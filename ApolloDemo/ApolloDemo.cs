using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApolloDemo
{
    class ApolloDemo
    {
        public ApolloDemo()
        {
        }

        public static void Main(string[] args)
        {
            var dd = ApolloConfigHelper.GetProperty<String>("test");
            Console.WriteLine(dd);

            var d = ApolloConfigHelper.GetProperty<decimal>("d");
            Console.WriteLine(d);
            var i = ApolloConfigHelper.GetProperty<Int32>("I");
            Console.WriteLine(i);
            
            Console.WriteLine("Apollo Config Demo. Please input key to get the value. Input quit to exit.");
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == null || input.Length == 0)
                {
                    continue;
                }
                input = input.Trim();
                if (input.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                {
                    Environment.Exit(0);
                }
                var fff = ApolloConfigHelper.GetProperty<ConfigInfo>("ConfigInfo", ApolloValueTypes.JSON);
                Console.WriteLine(JsonConvert.SerializeObject(fff));
            }
        }
    }

    public class ConfigInfo
    {
        public String Name { get; set; }
        public String Value { get; set; }
    }
}
