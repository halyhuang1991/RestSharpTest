using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RestSharpTest.HttpServer
{
    public class HttpResponse1
    {
         byte[] buffer = new byte[1024];
        public static void run(){
            Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4444));
            socketServer.Listen(int.MaxValue);
            Console.WriteLine("服务端已启动，等待连接...");
            //接收连接
            while (true)
            {
                Socket ts = socketServer.Accept();
                Console.WriteLine("客户端已连接");
                HttpResponse1 http = new HttpResponse1();
                http.Receive(ts);
            }
        }
         private void SendResponse(Socket socket){
            string h = "HTTP/1.1 200 OK\r\n";
            h += "Cache-Control: no-cache\r\n"; h += "Content-Type: application/json; charset=utf-8\r\n";
            h += "Server:SelfServer 1.0\r\n"; h += "selfhead:ok\r\n";
            h += "Date:" + DateTime.Now.ToString() + "\r\n";
            string content = "{\"a\":12,\"b\":[\"a\",\"w\",\"c\"]}";
            h += "Content-Length:" + content.Length + "\r\n";
            h += "Connection: close\r\n\r\n";
            h += content;
            byte[] send_buffer = Encoding.UTF8.GetBytes(h);
            socket.Send(send_buffer);
         }
         private void Receive(Socket ts){
             //清空数据，重新开始异步接收
            buffer = new byte[buffer.Length];
            ts.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
         }
         private void ReceiveCallback(IAsyncResult result)
        {
            Socket ts = (Socket)result.AsyncState;
            ts.EndReceive(result);
            result.AsyncWaitHandle.Close();
            string reqMsg=Encoding.UTF8.GetString(buffer,0,GetEndIndex(buffer));
            Console.WriteLine("收到消息：{0}",reqMsg);
            SendResponse(ts);
            //Receive(ts);
        }
        private int GetEndIndex(byte[] Buffer){
            byte cut = 0x00;
            int index=Array.IndexOf(Buffer,cut);
            return index;
        }
    }
}