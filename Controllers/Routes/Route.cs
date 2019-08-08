using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RestSharpTest.Controllers.Routes
{
    public class RouteValueDictionary : Dictionary<string, object>
    {
        public RouteValueDictionary Load(object values)
        {
            if (values != null)
            {
                foreach (var prop in values.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    this[prop.Name.ToLower()] = prop.GetValue(values);
                }
            }

            return this;
        }
    }
    public class Route
    {
        private static Dictionary<string,string> controllers=new Dictionary<string, string>();
        private static Dictionary<string,string[]> routes=new Dictionary<string, string[]>();
        //key:name+controller by controller  value:[controller,action]
        private static T getClass<T>(object[] obj) where T : new()
        {
            return (T)Activator.CreateInstance(typeof(T), obj);
        }
        public static void register(){
            Type type1 = typeof(BaseController);
            Func<Type, bool> IsExtend = (t) =>
            {
                bool r = false;
                while ((t = t.BaseType) != typeof(object))
                {
                    if(t==type1){
                        r=true;break;
                    }
                }
                return r;
            };
            var subTypeQuery = from t in Assembly.GetExecutingAssembly().GetTypes()
                                 where IsExtend(t) select t;
            foreach (var type in subTypeQuery)
            {
                Console.WriteLine(type.ToString()+"\t"+type.Name);
                controllers.TryAdd(type.Name.ToLower(),type.ToString());
            }
            MapRoute(name: "default",
                    template: "{controller=Index}/{action=Index}/{id?}");
        }
        public static void MapRoute(string name, string template){
            string pattern="{controller=([a-zA-Z0-9_]+)}/{action=([a-zA-Z0-9_]+)}";
            bool ret=Regex.IsMatch(template,pattern);
            if(!ret)return;
            MatchCollection matches=Regex.Matches(template,pattern);
            var t=from p in matches[0].Groups.AsEnumerable() where p.Index>0 select p.Value;
            if(t.Count()>0){
                routes.TryAdd(name,t.ToArray());
            }
        }
        public static void Go(params object[] args){
            if(args.Length==0)return;
            string controller; string action="";
            Object[] pms;string default1="";
            if(args.Length==1){
                default1=routes["default"][0].ToLower().Trim();
                if(routes["default"]==null)return;
                action=args[0].ToString();
                pms=new Object[]{};
            }else{
                default1=args[0].ToString().ToLower().Trim();
                action=args[1].ToString();
                pms=new object[args.Length-2];
                for(int i=2;i<args.Length;i++){
                   pms[i-2]=args[i];
                }
            }
            
         
            try
            {
                if(!default1.Contains("controller"))default1=default1+"controller";
                controllers.TryGetValue(default1,out controller);
                if(controller==null){
                    throw new Exception("没有找到继承BaseController的"+default1+"controller");
                }
                Type t=Type.GetType(controller);
                object o=Activator.CreateInstance(t);
                MethodInfo[] info = t.GetMethods();
                bool ret=false;
                for (int i = 0; i < info.Length; i++)
                {
                    var md = info[i];
                    //方法名
                    string mothodName = md.Name;
                    if (mothodName.ToLower() == action.ToLower()){
                        //参数集合
                        ParameterInfo[] paramInfos = md.GetParameters();
                        Object[] param;
                        // if (paramInfos.Length <= pms.Length)
                        // {
                        //     pms = pms.Take(paramInfos.Length).ToArray();
                        // }
                        param = new Object[paramInfos.Length];
                        for (int pt = 0; pt < param.Length; pt++)
                        {
                            if (pt == pms.Length)
                            {
                                param[pt] = null;
                                continue;
                            }
                            param[pt] = pms[pt];
                            if (paramInfos[pt].ParameterType == typeof(string))
                            {
                                param[pt] = pms[pt].ToString();
                            }
                            if (paramInfos[pt].ParameterType == typeof(int))
                            {
                                param[pt] = Convert.ToInt16(pms[pt]);
                            }
                            if (paramInfos[pt].ParameterType == typeof(decimal))
                            {
                                param[pt] = Convert.ToDecimal(pms[pt]);
                            }
                        }
                        ret=true;
                        md.Invoke(o, param);
                        break;
                    }
                    
                }
                if(!ret){
                    throw new Exception("没有找到方法"+action);
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
        }
        public static void MapRoute(string name, object template){
            if(template==null)return;
            RouteValueDictionary routesdic=new RouteValueDictionary().Load(template);
            if(String.IsNullOrEmpty((string)routesdic["controller"])||String.IsNullOrEmpty((string)routesdic["action"]))return;
            routes.TryAdd(name,new string[]{routesdic["controller"].ToString(),routesdic["action"].ToString()});
        }

    }

    
}