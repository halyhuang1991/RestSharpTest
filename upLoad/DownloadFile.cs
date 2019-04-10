using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RestSharpTest.upLoad
{
    public class DownloadFile
    {
        private static string filename="test.txt";
        public static void Download(){
            string deskpath=@"C:\Users\halyhuang\Desktop";
            string path=deskpath+"\\test.txt";
            FileInfo fileInfo = new System.IO.FileInfo(path);
            if (fileInfo.Exists)
            {
                Console.WriteLine(fileInfo.Length.ToString());
            }
            File.Delete(path);
             //Download("http://download.firefox.com.cn/releases-sha2/stub/official/zh-CN/Firefox-latest.exe",deskpath);
            string url="http://localhost:18096/api/download/resumeFile";
            Downloadfile(url,deskpath);
        }
       public static bool Downloadfile(string Url, string desPath,long from=0,long to=0){
            bool flag = false;
            long btCount=1024*64;
            if(to==0)to=btCount;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AddRange(from, to);
            try
            {
                long length =0;
                ContentRange contentRange;
                //获取HTTP回应，注意HttpWebResponse继承自IDisposable
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        throw new Exception("文件不支持Range部分下载");
                    Console.WriteLine(from+"--"+to);
                    string fN=GetFileName(response);
                    string filePath=desPath+@"\"+fN;
                    //设置接收信息的缓冲器
                    var bytes = new byte[5000];
                    
                    //获取回应的Stream（字节流）
                    using (var stream = response.GetResponseStream())
                    {//FileMode.Append 我这里是从文件末尾处写入        如果是多线程 就要根据具体的请求的位置 写入文件
                       
                        using (var outStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                        {
                            const int bufferLen = 4096;
                            byte[] buffer = new byte[bufferLen];
                            int count = 0;

                            while ((count = stream.Read(buffer, 0, bufferLen)) > 0)
                            {
                                outStream.Write(buffer, 0, count);
                            }
                            outStream.Close();
                            stream.Close();
                        }
                    }
                    length = response.ContentLength;
                    contentRange=GetRange(response);
                    length=contentRange.total-1;
                    if(contentRange.end==contentRange.total-1)return true;
                }
                from=to+1;
                to=to+btCount;
                if(to>=contentRange.total){
                    to=contentRange.total-1;
                }
                Downloadfile(Url,desPath,from,to);
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误信息：{0}", ex.Message+ex.StackTrace);
                flag=false;
            }
            return flag;
       }
        private class ContentRange
        {
            public int start { get; set; }
            public int end { get; set; }
            public int total { get; set; }
        }
        private static ContentRange GetRange(WebResponse res)
        {
            ContentRange contentRange = new ContentRange();
            string Content_Range = res.Headers["Content-Range"];
            string RegexStr = @"bytes (?<start>[\d]+)-(?<end>[\d]+)/(?<total>[\d]+)";
            Match matc = Regex.Match(Content_Range, RegexStr);
            contentRange.start = int.Parse(matc.Groups["start"].ToString());
            contentRange.end = int.Parse(matc.Groups["end"].ToString());
            contentRange.total = int.Parse(matc.Groups["total"].ToString());
            return contentRange;
        }
        public static bool Download(string sourceFile, string desFile){
             bool flag = false;
            long SPosition = 0;
            FileStream FStream = null;
            Stream myStream = null;
            string fileName = sourceFile.Substring(sourceFile.LastIndexOf(@"/") + 1);
            if (desFile.EndsWith("\\"))
            {
                desFile = desFile + fileName;
            }
            else
            {
                desFile = desFile + "\\" + fileName;
            }
            try
            {
                long serverFileLength =0;
                //判断要下载的文件夹是否存在
                if (File.Exists(desFile))
                {
                    //打开上次下载的文件
                    FStream = File.OpenWrite(desFile);
                    //获取已经下载的长度
                    SPosition = FStream.Length;
                    serverFileLength = GetHttpLength(sourceFile);
                    if (SPosition == serverFileLength)
                    {//文件是完整的，直接结束下载任务
                        return true;
                    }
                    FStream.Seek(SPosition, SeekOrigin.Current);
                }
                else
                {
                    //文件不保存创建一个文件
                    FStream = new FileStream(desFile, FileMode.Create);
                    SPosition = 0;
                }
               
                //打开网络连接
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(sourceFile);
              
                if (SPosition > 0)
                {
                    myRequest.AddRange(SPosition);             //设置Range值
                }
                //向服务器请求,获得服务器的回应数据流
                myStream = myRequest.GetResponse().GetResponseStream();
               
                //定义一个字节数据
                byte[] btContent = new byte[512];
                int intSize = 0;
                intSize = myStream.Read(btContent, 0, 512);
                while (intSize > 0)
                {
                    FStream.Write(btContent, 0, intSize);
                    intSize = myStream.Read(btContent, 0, 512);
                }
                flag = true;        //返回true下载成功
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("下载文件时异常：" + ex.Message+ex.StackTrace);
            }
            finally
            {
                //关闭流
                if (myStream != null)
                {
                    myStream.Close();
                    myStream.Dispose();
                }
                if (FStream != null)
                {
                    FStream.Close();
                    FStream.Dispose();
                }
            }
            return flag;
        }
        private static bool GetAcceptRanges(WebResponse res)
        {
            if (res.Headers["Accept-Ranges"] != null)
            {
                string s = res.Headers["Accept-Ranges"];
                if (s == "none")
                {
                    return false;
                }
            }
            return true;
        }

        private static string GetEtag(WebResponse res)
        {
            if (res.Headers["ETag"] != null)
            {
                return res.Headers["ETag"];
            }
            return null;
        }
        private static string GetFileName(WebResponse res)
        {
            string disposition = res.Headers["Content-Disposition"];
            if (string.IsNullOrEmpty(disposition))
                disposition = res.ResponseUri.Segments[res.ResponseUri.Segments.Length - 1];
            else
                disposition = disposition.Remove(0, disposition.IndexOf("filename=") + 9);
            byte[] byteArray = Encoding.GetEncoding("utf-8").GetBytes(disposition);
            string filename= Encoding.GetEncoding("utf-8").GetString(byteArray);
            return filename;
        }
        static long GetHttpLength(string url)
        {
            long length = 0;
            try
            {
                var req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                req.Method = "HEAD";//获取请求实体的元信息而不需要传输实体主体
                req.Timeout = 5000;
                var res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    length = res.ContentLength;
                }
                res.Close();
                return length;
            }
            catch (WebException wex)
            {
                return 0;
            }
        }
    }
}