using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestSharpTest.UDP
{
    public class UDPClient
    {
         public void run(){
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6000));//绑定端口号和IP 
            Console.WriteLine("客户端已经开启"); 
            Task t = new Task(ReciveMsg,server);//开启接收消息线程 
            t.Start(); 
            Task t2 = new Task(sendMsg,server);//开启发送消息线程 
            t2.Start();

        }
        void sendMsg(object server1)
        {
            Socket server = (Socket)server1;
            EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001);
            while (true)
            {
                string msg = " server6000 to 60001 "+DateTime.Now.ToString();//Console.ReadLine();
                // for(int i=0;i<516;i++){
                //     msg+="test"+i;
                // }
                server.SendTo(Encoding.UTF8.GetBytes(msg), point);
                Thread.Sleep(10000);
            }
        }
        void ReciveMsg(object server1)
        {
            Socket server = (Socket)server1;
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号 
                byte[] buffer = new byte[1024];
                int length = server.ReceiveFrom(buffer, ref point);//接收数据报 
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Console.BackgroundColor=ConsoleColor.DarkYellow;
                Console.WriteLine(point.ToString() + message);
            }
        }
    }
}