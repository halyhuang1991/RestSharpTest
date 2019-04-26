using System;
using System.IO;
using System.Linq;
using System.Net;
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
        StringBuilder stringBuilder=new StringBuilder();
        stringBuilder.Append("<html><body><h1>test server</h1>");
        stringBuilder.Append("Current Time: " + DateTime.Now.ToString());
        stringBuilder.Append(string.Format("url : {0}", p.URL));
 
        stringBuilder.Append("<form method=post action=/form>");
        stringBuilder.Append("<input type=text name=foo value=foovalue>");
        stringBuilder.Append("<input type=submit name=bar value=barvalue>");
        stringBuilder.Append("</form></body></html>");
        p.SendResponse("{\"a\":12,\"q\":[12,2,4,6]}");
        //fetch("http://localhost:8150/",{method:'get',mode: 'cors'}).then(function(data){return data.json();}).then(function(data){console.log(data)})
    }
 
    public override void OnPost(HttpRequest p) {
        Console.WriteLine("POST request: {0}", p.URL);
        string data = "";
        data+="<html><body><h1>test server</h1>";
        data+="<a href=/test>return</a><p>";
        data+="postbody: <pre>ok</pre>";
        data+="</body></html>";
        p.SendResponse("{\"a\":12,\"q\":[12,2,4,6]}");
        //fetch("http://localhost:8150/",{method:'post',mode:'cors'}).then(function(data){return data.text();}).then(function(data){console.log(data)})
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
        listener = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.port));
        listener.Start();
        while (is_active) {                
            TcpClient s = listener.AcceptTcpClient();
            Task task=new Task(ProcessRequest,(object)s);
            task.Start();
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