using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RestSharpTest.HttpServer
{
    public class HttpRequest
    {
        
        /// <summary>
        /// URL参数
        /// </summary>
        public Dictionary<string, string> Params { get; private set; }

        /// <summary>
        /// HTTP请求方式
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// HTTP(S)地址
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// HTTP协议版本
        /// </summary>
        public string ProtocolVersion { get; set; }

        /// <summary>
        /// 定义缓冲区
        /// </summary>
        private const int MAX_SIZE = 1024 * 1024 * 2;
        private byte[] bytes = new byte[MAX_SIZE];

        public Stream handler;
        private  Dictionary<string, string> Headers;
         private string Body;
     private Dictionary<string, string> RequestHeaders;

        public HttpRequest(Stream stream)
        {
            this.handler = stream;
            var data = GetRequestData(handler);
            var rows = Regex.Split(data, Environment.NewLine);

            //Request URL & Method & Version
            var first = Regex.Split(rows[0], @"(\s+)")
                .Where(e => e.Trim() != string.Empty)
                .ToArray();
            if (first.Length > 0) this.Method = first[0];
            if (first.Length > 1) this.URL = Uri.UnescapeDataString(first[1]).Split('?')[0];
            if (first.Length > 2) this.ProtocolVersion = first[2];

            //Request Headers
            this.Headers = GetRequestHeaders(rows);

            //Request "GET"
            if (this.Method == "GET")
            {
                this.Body = GetRequestBody(rows);
                var isUrlencoded = this.URL.Contains('?');
                if (isUrlencoded) this.Params = GetRequestParameters(URL.Split('?')[1]);
            }

            //Request "POST"
            if (this.Method == "POST")
            {
                this.Body = GetRequestBody(rows);
                var contentType = "text/html";
                var isUrlencoded = contentType == @"application/x-www-form-urlencoded";
                if (isUrlencoded) this.Params = GetRequestParameters(this.Body);
            }
        }

        public Stream GetRequestStream()
        {
            return this.handler;
        }

        public void SendMsg(string msg)
        {
            Stream stream = GetRequestStream();
            // BinaryWriter bw = new BinaryWriter(stream);
            // bw.Write(msg);
            // bw.Close();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }
        public void SendResponse(string msg){
            string h = "HTTP/1.1 200 OK\r\n";
            h+="Access-Control-Allow-Origin:*\r\n";
            h+="Access-Control-Allow-Methods:GET, POST*\r\n";
            h += "Cache-Control: no-cache\r\n"; h += "Content-Type: application/json; charset=utf-8\r\n";
            h += "Server:SelfServer 1.0\r\n"; h += "selfhead:ok\r\n";
            h += "Date:" + DateTime.Now.ToString() + "\r\n";
            string content = msg;
            h += "Content-Length:" + content.Length + "\r\n";
            h += "Connection: close\r\n\r\n";
            h += content;
            SendMsg(h);
        }
        public void SendRequest(string url,string method,string msg){
            Uri URI = new Uri(url);
            StringBuilder RequestHeaders = new StringBuilder();
            RequestHeaders.Append(method.ToUpper() + " " + URI.PathAndQuery + " HTTP/1.1\r\n");
            if (method.ToUpper() == "GET")
            {
                RequestHeaders.Append("Content-Type:application/json\r\n");
            }
            else
            {
                RequestHeaders.Append("Content-Type:application/x-www-form-urlencoded\r\n");
            }
            RequestHeaders.Append("User-Agent:Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11\r\n");
            //RequestHeaders.Append("Cookie:" + _cookie + "\r\n");
            RequestHeaders.Append("Accept:*/*\r\n");
            RequestHeaders.Append("Host:" + URI.Host + "\r\n");
            RequestHeaders.Append("Content-Length:" + msg.Length + "\r\n");
            RequestHeaders.Append("Connection:close\r\n\r\n");
            RequestHeaders.Append(msg);
            SendMsg(RequestHeaders.ToString());
        }
        private string GetRequestData(Stream stream)
        {
            var length = 0;
            var data = string.Empty;

            do
            {
                length = stream.Read(bytes, 0, MAX_SIZE - 1);
                data += Encoding.UTF8.GetString(bytes, 0, length);
            } while (length > 0 && !data.Contains("\r\n\r\n"));

            return data;
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

        private Dictionary<string, string> GetRequestParameters(string row)
        {
            if (string.IsNullOrEmpty(row)) return null;
            var kvs = Regex.Split(row, "&");
            if (kvs == null || kvs.Count() <= 0) return null;

            return kvs.ToDictionary(e => Regex.Split(e, "=")[0], e => Regex.Split(e, "=")[1]);
}
    }
}