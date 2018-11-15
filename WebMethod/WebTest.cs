using System;
using System.Collections.Generic;
using System.Net;
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
          public static void TestCookie(){
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("action", "save");
            CookieContainer cookieContainer = WebHelper.GetCookie("http://localhost:18096/Handlers/Handler.ashx", "action=save");//使session生效
            string ret = WebHelper.HttpPost("http://localhost:18096/Handlers/Handler.ashx", null, "action=save",cookieContainer);//取得session
            Console.WriteLine(ret);
        }
    }
}