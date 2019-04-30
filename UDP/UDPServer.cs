using System.Net.Sockets;
using System;
using System.Threading;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestSharpTest.UDP
{
    public class UDPServer
    {
        public  void run(){
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001));//绑定端口号和IP 
            Console.WriteLine("服务端已经开启"); 
            Task t = new Task(ReciveMsg,server);//开启接收消息线程 
            t.Start(); 
            Task t2 = new Task(sendMsg,server);//开启发送消息线程 
            t2.Start();

        }
        void sendMsg(object server1)
        {
            Socket server = (Socket)server1;
            EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000);
            while (true)
            {
                string msg = "  server6001 to 60000 "+DateTime.Now.ToString();//Console.ReadLine();
              
                server.SendTo(Encoding.UTF8.GetBytes(msg), point);
                Thread.Sleep(10000);
            }
        }
        void ReceiveMessage(IAsyncResult ar){
            try
            {
                var socket = ar.AsyncState as Socket;

                //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.endreceive.aspx  
                var length = socket.EndReceive(ar);
                byte[] buffer = new byte[1024];
                //读取出来消息内容  
                var message = Encoding.ASCII.GetString(buffer, 0, length);

                Console.WriteLine(message, ConsoleColor.White);
                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息了）  
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ConsoleColor.Red);
                //var socket = ar.AsyncState as Socket;
                //socket.Close();
                
            }
        }
        void ReciveMsg(object server1)
        {
            bool flg=true;
            Socket server = (Socket)server1;
            byte[] buffer = new byte[1024*8];
                  //    server.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, 
                //    new AsyncCallback(ReceiveMessage), server);
            while (flg)
            {
                 EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号 
                try
                {
                    int length = server.ReceiveFrom(buffer, ref point);//接收数据报 
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    Console.WriteLine(point.ToString() +" "+ message+" "+server.Available);
                }
                catch(Exception ex)
                {
                    flg = false;
                    //server.Close();
                    Console.WriteLine("no way to receive."+ex.Message);
                }
             
            }
        }
               


    }
}