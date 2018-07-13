using System;
using System.Collections.Generic;
using RestSharp;
namespace RestSharpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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
        public class Person{
            public string name{get;set;}
            public int id{get;set;}
            public string password{get;set;}
            public string profession{get;set;}
        }
    }
}
