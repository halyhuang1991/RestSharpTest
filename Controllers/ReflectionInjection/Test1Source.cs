using System;

namespace RestSharpTest.Controllers.ReflectionInjection
{
    public class Test1Source : IServiceSource
    {
         public string Description { get; private set; }
        public Test1Source()
        {
            this.Description = " Test1Source";
        }
        public string ShowInfo()
        {
            return this.Description;
        }
    }
    public class Test2Source : IServiceSource
    {
         public string Description { get; private set; }
        public Test2Source()
        {
            this.Description = " Test2Source";
        }
        public string ShowInfo()
        {
            return this.Description;
        }
    }
    public class Test3Source : IServiceSource
    {
         public string Description { get; private set; }
        public Test3Source()
        {
            this.Description = " Test3Source";
        }
        public string ShowInfo()
        {
            return this.Description;
        }
    }
    public class ClientClass
    {
        public ClientClass(){

        }
        public ClientClass(IServiceSource serviceImpl)
        {
            this._serviceImpl = serviceImpl;
            Console.WriteLine("注入点"+serviceImpl.ShowInfo());
        }
        private IServiceSource _serviceImpl;
        //客户类中的方法，初始化注入点  
        public void Set_ServiceImpl(IServiceSource serviceImpl)
        {
            this._serviceImpl = serviceImpl;
        }

        public void ShowInfo()
        {
            Console.WriteLine(_serviceImpl.ShowInfo());
        }
    }
}