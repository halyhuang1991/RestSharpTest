using System;
using RestSharpTest.Controllers.Routes;

namespace RestSharpTest.Controllers
{
    public class IndexController:BaseController
    {
        private static int count=0;
        public void index(){
            count++;
            Console.WriteLine("IndexController"+count);
        }
        public void Get(string i){
            Console.WriteLine("IndexController-Get"+i);
        }
    }
}