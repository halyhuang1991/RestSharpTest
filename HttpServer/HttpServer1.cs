using System.IO;
using System.Net;
using System.Threading;

namespace RestSharpTest.HttpServer
{
    public class HttpServer1
    {
        public static void run(){
            HttpListener httpListener = new HttpListener();
       
                httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                httpListener.Prefixes.Add("http://localhost:8091/");
                httpListener.Start();
               new Thread(new ThreadStart(delegate
                {
                    while (true)
                    {
                         
                            HttpListenerContext httpListenerContext = httpListener.GetContext();
                            httpListenerContext.Response.StatusCode = 200;
                             using (StreamWriter writer = new StreamWriter(httpListenerContext.Response.OutputStream))
                            {
                                writer.WriteLine("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/><title>测试服务器</title></head><body>");
                                writer.WriteLine("<div style=\"height:20px;color:blue;text-align:center;\"><p> hello</p></div>");
                                writer.WriteLine("<ul>");
                                writer.WriteLine("</ul>");
                                writer.WriteLine("</body></html>");
                                     
                           }
                        
                    }
                })).Start();
        }
    }
}