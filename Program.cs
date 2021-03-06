﻿using System;
using System.IO;
using System.Collections.Generic;
using RestSharp;
using RestSharp.Extensions;
using RestSharpTest.upLoad;
using RestSharpTest.HttpServer;
using RestSharpTest.UDP;
using System.Text.RegularExpressions;
using RestSharpTest.Controllers.ReflectionInjection;
using System.Xml.Linq;
using System.Linq;

namespace RestSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
        private void testexml(){
            string path=@"D:\C\github\RestSharpTest\Controllers\ReflectionInjection\Config.xml";
            XElement element=XElement.Load(path);
            var config=element.Elements();
            element.SetAttributeValue("id",12);
            var es=from i in config select i;
            foreach(var item in es){
                  Console.WriteLine(item.Value,item.Name);
            }
            element.Save(path);
        }
        public static void testRoutes(){
            Controllers.Routes.Route.register();
            Controllers.Routes.Route.Go("index");
            Controllers.Routes.Route.Go("index", "Index");
            Controllers.Routes.Route.Go("Index", "Get", 1);
            Controllers.Routes.Route.Go("test", "Test");
            Controllers.Routes.Route.Go("test", "Test1",2,"A12");
            var sarr = ReflectionFactory.MakeSources();
            foreach (var s in sarr)
            {
                Console.WriteLine("创建 " + s.ShowInfo());
            }
            ReflectionFactory.ShowSource("Test1Source");
        }
        public static void UDPtest(){
            UDPServer my = new UDPServer();
            my.run();
            UDPClient my1 = new UDPClient();
            my1.run();
        }
        private static void test(){
            Console.WriteLine("Hello World!");
            //uploadfile();//上传文件
            //ToObject();//返回对象
            //download();//下载文件
         
            var client = new RestClient("http://127.0.0.1:8089");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("listUsers/{id}", Method.GET);
            request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource
            //http://127.0.0.1:8089/listUsers/123?name=value

            // add parameters for all properties on an object
            //request.AddObject(object);

            // or just whitelisted properties
            //request.AddObject(object, "PersonId", "Name");

            // easily add HTTP Headers
            request.AddHeader("header", "value");

            // add files to upload (works with compatible verbs)
            //request.AddFile("file", path);

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string
            Console.WriteLine(content);
            // or automatically deserialize result
            // return content type is sniffed but can be explicitly set via RestClient.AddHandler();

            var request1 = new RestRequest("listUsers", Method.GET);
            request1.AddParameter("num", "23"); 
            List<Person> ls=new List<Person>();
            IRestResponse<Person> response2 = client.Execute<Person>(request1);
            Console.WriteLine(response2.Data.id.ToString());
            Console.WriteLine(response2.Content.ToString());
            
           

            // or download and save file to disk
            //client.DownloadData(request).SaveAs(path);

            // easy async support
            //await client.ExecuteAsync(request);

            // async with deserialization
            var asyncHandle = client.ExecuteAsync<Person>(request1, res =>
            {
                Console.WriteLine(res.Content.ToString());
            });

            // // abort the request on demand
            asyncHandle.Abort();
        }
        private static void uploadfile(){
            var client = new RestClient("http://localhost:9000/");
            var request = new RestRequest("api/File/GetFile", Method.POST);
            string path = @"D:\4.txt";
            //request.AddHeader("Content-Type", "application/zip");
            request.AddFile("file", path);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content.ToString() + response.StatusDescription);
        }
        private static void ToObject(){
            var client = new RestClient("http://localhost:9000/");
            var request = new RestRequest("api/account/{id}", Method.GET);
            request.AddUrlSegment("id", "st"); 
            request.AddParameter("name", "val");
            request.AddParameter("age", "22");
            IRestResponse<student> response = client.Execute<student>(request);
            Console.WriteLine(response.Content.ToString());
        }
        private  static void download(){
            var client = new RestClient("http://localhost:9000/");
            var request = new RestRequest("api/Down/DownFile", Method.POST);
            //client.DownloadData(request).SaveAs("D:\\44.txt");//RestSharp.Extensions
            byte[] bytes=client.DownloadData(request);
            //System.IO.File.WriteAllBytes("D:\\44.txt", bytes);
            string  str  = System.Text.Encoding.UTF8.GetString(bytes); 
            Console.WriteLine(str);
            // IRestResponse response = client.Execute(request);
            // Console.WriteLine(response.Content.ToString() + response.StatusDescription);

        }
        public class Person{
            public string name{get;set;}
            public int id{get;set;}
            public string password{get;set;}
            public string profession{get;set;}
        }
        public class student
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
