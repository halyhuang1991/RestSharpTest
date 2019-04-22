using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace RestSharpTest.HttpServer
{
    public class HttpRequest1
    {
        public string _path="";
         public string _host="";
         byte[] buffer = new byte[1024*64];
          public static void test(){
            HttpRequest1 http = new HttpRequest1();
            http._host = "127.0.0.1"; http._path = "http://localhost:18096/api/Download";
            Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketClient.Connect(IPAddress.Parse("127.0.0.1"), 18096);
            http.SendRequest(socketClient);
            Console.WriteLine("客户端已启动，等待连接...");
          }
         private void SendRequest(Socket socket)
        {
            string h1 = "GET " + _path + " HTTP/1.1\r\n";
            string h2 = "Accept: */*\r\n";
            string h3 = "Accept-Language: zh-cn\r\n";
            string h4 = "Host: " + _host + "\r\n";
            string h5 = "User-Agent: Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36\r\n";
            string h7 = "Connection: close\r\n\r\n";

            byte[] send_buffer = Encoding.UTF8.GetBytes(h1 + h2 + h3 + h4 + h5 + h7);
            socket.Send(send_buffer);
            Console.WriteLine("请求发送完毕，等待Web Server回复...");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), socket);
        }
        private void OnReceive(IAsyncResult result)
        {
            Socket ts = (Socket)result.AsyncState;
            ts.EndReceive(result);
            result.AsyncWaitHandle.Close();
            string values=Encoding.UTF8.GetString(buffer,0,GetEndIndex(buffer)).TrimEnd();
            var rows = Regex.Split(values, Environment.NewLine);
            string body=GetRequestBody(rows);
            Dictionary<string, string> dic=GetRequestHeaders(rows);
            Console.WriteLine("收到消息：{0}", values);
            //清空数据，重新开始异步接收
            // buffer = new byte[buffer.Length];
            // ts.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), ts);
        }
        private string GetRequestBody(IEnumerable<string> rows)
        {
            var target = rows.Select((v, i) => new { Value = v, Index = i }).FirstOrDefault(e => e.Value.Trim() == string.Empty);
            if (target == null) return null;
            var range = Enumerable.Range(target.Index + 1, rows.Count() - target.Index - 1);
            return string.Join(Environment.NewLine, range.Select(e => rows.ElementAt(e)).ToArray());
        }
        private Dictionary<string, string> GetRequestHeaders(IEnumerable<string> rows)
        {
            if (rows == null || rows.Count() <= 0) return null;
            var target = rows.Select((v, i) => new { Value = v, Index = i }).FirstOrDefault(e => e.Value.Trim() == string.Empty);
            var length = target == null ? rows.Count() - 1 : target.Index;
            if (length <= 1) return null;
            var range = Enumerable.Range(1, length - 1);
            return range.Select(e => rows.ElementAt(e)).ToDictionary(e => e.Split(':')[0], e => e.Split(':')[1].Trim());
        }
        private int GetEndIndex(byte[] Buffer){
            byte cut = 0x00;
            int index=Array.IndexOf(Buffer,cut);
            return index;
        }
    }
}