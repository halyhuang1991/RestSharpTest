using System;
using System.Reflection;
using System.Xml;

namespace RestSharpTest.Controllers.ReflectionInjection
{
    public class ReflectionFactory
    {
        private static string _SourceType;
        private static string[] _SourceTypes;
        static ReflectionFactory()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string path=Environment.CurrentDirectory + "\\Config.xml";
            path=@"D:\C\github\RestSharpTest\Controllers\ReflectionInjection\Config.xml";
            xmlDoc.Load(path);
            XmlNode xmlNode = xmlDoc.ChildNodes[1].ChildNodes[1];
            _SourceType = xmlNode.ChildNodes[0].Value;
            _SourceTypes=new string[xmlDoc.ChildNodes[1].ChildNodes.Count];
            for (int i = 0; i < xmlDoc.ChildNodes[1].ChildNodes.Count; i++)
            {
                XmlNode node=xmlDoc.ChildNodes[1].ChildNodes[i];
                _SourceTypes[i] = node.InnerText;
            }
            
        }
        public static IServiceSource MakeSource(string SourceType)
        {
//Assembly ass=Assembly.Load("RestSharpTest");
            return Activator.CreateInstance(Type.GetType("RestSharpTest.Controllers.ReflectionInjection." + SourceType)) as IServiceSource;
        }
        public static IServiceSource MakeSource()
        {
           // Assembly ass=Assembly.Load("RestSharpTest");
            return Activator.CreateInstance(Type.GetType("RestSharpTest.Controllers.ReflectionInjection." + _SourceType)) as IServiceSource;
        }
        public static void ShowSource(string SourceType)
        {
            Type type=Type.GetType("RestSharpTest.Controllers.ReflectionInjection.ClientClass");
            ClientClass obj=Activator.CreateInstance(type) as ClientClass;
            IServiceSource Source =MakeSource(SourceType);
            obj.Set_ServiceImpl(Source);
            obj.ShowInfo();
            //---------------------ClientClass 可以继承接口
            object[] pms=new object[]{};
            ConstructorInfo[] infoArray1 = type.GetConstructors(); 
            foreach (ConstructorInfo info in infoArray1) { 
                if(!info.IsStatic){
                    ParameterInfo[] parameterInfos=info.GetParameters();
                    pms=new object[parameterInfos.Length];
                    if(parameterInfos.Length>0){
                        for(int i=0;i<parameterInfos.Length;i++){
                            pms[i]=null;
                            //可以外部维护字典key：IServiceSource value：Source 实现注入
                            if(parameterInfos[i].ParameterType==typeof(IServiceSource)){
                                pms[i]=Source;
                            }
                        }
                    }
                }
            }
            ClientClass obj1=Activator.CreateInstance(type,pms) as ClientClass;
            obj.Set_ServiceImpl(Source);
            obj.ShowInfo();
        }
        public static IServiceSource[] MakeSources()
        {
            IServiceSource[] iss=new IServiceSource[_SourceTypes.Length];
            //Assembly ass=Assembly.Load("RestSharpTest");
            int i=0;
            foreach (var item in _SourceTypes)
            {
                IServiceSource v= Activator.CreateInstance(Type.GetType("RestSharpTest.Controllers.ReflectionInjection."+item)) as IServiceSource;
                iss[i]=v;
                i++;

            }
            return iss;
        }
    
    }
}