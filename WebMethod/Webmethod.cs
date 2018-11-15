using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebMethod
{
    public class WebHelper
    {
        /// <summary>
        /// get请求
        /// ContentType = "text/html;charset=UTF-8";
        /// token为Authorization中的授权验证码
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string token = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            if (token != null)
            {
                request.Headers.Add("Authorization", token);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

        }
        public static string HttpGet(string Url, string token = null, Dictionary<string, string> param = null)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Url);
            if (param.Count > 0 && Url.IndexOf("?") == -1)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in param)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(builder.ToString());
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            if (token != null)
            {
                request.Headers.Add("Authorization", token);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

        }

        public static string HttpPost(string Url, string token = null, string postDataStr = null,CookieContainer cookie=null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            //request.ContentType = "text/html;charset=UTF-8";
            request.ContentType = "application/x-www-form-urlencoded";

            if (token != null)
            {
                request.Headers.Add("Authorization", token);
            }
            if (postDataStr != null)
            {
                byte[] postData = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = postData.Length;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8"));
                myRequestStream.Write(postData, 0, postData.Length);
                myRequestStream.Close();
            }
            if(cookie!=null){
                request.CookieContainer=cookie;
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;

        }
        public static string HttpPost(string Url, string token = null, Dictionary<string, string> param = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (KeyValuePair<string, string> kv in param)
                {
                    if (stringBuilder.Length == 0)
                    {
                        stringBuilder.AppendFormat("{0}={1}", kv.Key, kv.Value);
                    }
                    else
                    {
                        stringBuilder.AppendFormat("&{0}={1}", kv.Key, kv.Value);
                    }
                }
            }
            return HttpPost(Url, token, stringBuilder.ToString());
        }
        
       
        public static CookieContainer GetCookie(string Url, string postDataStr = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.AllowAutoRedirect = false;
            request.ContentType = "application/x-www-form-urlencoded";
            // request.Headers.Add("Cookie", name + "=" + value);
            if (postDataStr != null)
            {
                byte[] postData = Encoding.UTF8.GetBytes(postDataStr);
                request.ContentLength = postData.Length;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("utf-8"));
                myRequestStream.Write(postData, 0, postData.Length);
                myRequestStream.Close();
            }
            CookieContainer m_Cookie = new CookieContainer();
            request.CookieContainer = m_Cookie;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            m_Cookie = request.CookieContainer;
            string cookie = response.Headers.Get("Set-Cookie");
            return m_Cookie;
        }
    }
}