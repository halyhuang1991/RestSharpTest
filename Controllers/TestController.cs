using System;
using RestSharpTest.Controllers.Routes;

namespace RestSharpTest.Controllers
{
    public class TestController:BaseController
    {
        public void Test(){
            Console.WriteLine("TestCotroller");
        }
        public void Test1(int i,string b){
            Console.WriteLine("TestCotroller -Test1 -"+i+b);
        }
    }
}