using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RestSharpTest.HttpServer
{
    
public class MyHttpServer : HttpServer {
    public MyHttpServer(int port)
        : base(port) {
    }
    public override void OnGet(HttpRequest p) {
        Console.WriteLine("request: {0}", p.URL);
        // p.outputStream.WriteLine("<html><body><h1>test server</h1>");
        // p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
        // p.outputStream.WriteLine("url : {0}", p.URL);
 
        // p.outputStream.WriteLine("<form method=post action=/form>");
        // p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
        // p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
        // p.outputStream.WriteLine("</form>");
    }
 
    public override void OnPost(HttpRequest p) {
        Console.WriteLine("POST request: {0}", p.URL);
        string data = "";
 
        // p.handler.WriteLine("<html><body><h1>test server</h1>");
        // p.handler.WriteLine("<a href=/test>return</a><p>");
        // p.handler.WriteLine("postbody: <pre>{0}</pre>", data);
    }
    public override void OnDefault(){
        Console.Write("ok");
    }
}

    public abstract class HttpServer {

    protected int port;
    TcpListener listener;
    bool is_active = true;
   
    public HttpServer(int port) {
        this.port = port;
    }
    
    public void listen() {
        listener = new TcpListener(port);
        listener.Start();
        while (is_active) {                
            TcpClient s = listener.AcceptTcpClient();
            Task task=new Task(ProcessRequest,(object)s);

            Thread.Sleep(1);
        }
    }
    private void ProcessRequest(object handler)
{
    TcpClient s=handler as TcpClient;
    
    HttpRequest request = new HttpRequest(s.GetStream());

    //根据请求类型进行处理
    if(request.Method == "GET"){
        OnGet(request);
    }else if(request.Method == "POST"){
        OnPost(request);
    }else{
        OnDefault();
    }
}
    public abstract void OnDefault();
    public abstract void OnGet(HttpRequest h);
    public abstract void OnPost(HttpRequest h);
}
}