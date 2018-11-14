using System;
using System.Collections.Generic;
using WebMethod;

namespace RestSharpTest
{
    public class WebTest
    {
        public static void TestPost(){
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("action", "save");
            string ret = WebHelper.HttpPost("http://localhost:18096/Handlers/Handler.ashx", null, dic);
            Console.WriteLine(ret);
        }
    }
}