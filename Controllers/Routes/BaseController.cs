using System;

namespace RestSharpTest.Controllers.Routes
{
    public class BaseController:IDisposable
    {
        public dynamic ViewBag { get; }
        public BaseController(){

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}