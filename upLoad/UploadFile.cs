using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Cryptography;

namespace RestSharpTest.upLoad
{
    public class UploadFile
    {
        public static void UploadLocalFiles(){
            HttpClient client = new HttpClient();
            //client.DefaultRequestHeaders.Connection.Add("keep-alive");
            string token="123456";
            client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Basic", token);//"encrypted user/pwd"
            //new AuthenticationHeaderValue("Basic",Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}")));
            // client.DefaultRequestHeaders.Add("Authorization",$"Basic {token}");
            MultipartFormDataContent form = new MultipartFormDataContent();
            FileStream file = File.OpenRead(@"D:\4.txt");
            StreamContent fileContent = new StreamContent(file);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            fileContent.Headers.ContentDisposition.FileName = "4.txt";
            form.Add(fileContent);
            var values = new[]{
                    new KeyValuePair<string, string>("email", "mrafi127@yahoo.com"),
                    new KeyValuePair<string, string>("first_name", "Rafi"),
                    new KeyValuePair<string, string>("last_name", "Qureshi"),
                    new KeyValuePair<string, string>("transaction_name", "Miguel's TPS notarization for Leeroy"),
                    new KeyValuePair<string, string>("transaction_type", "Power of Attorney"),
                    new KeyValuePair<string, string>("message_to_signer", "Please notarize your TPS Report"),
                    new KeyValuePair<string, string>("message_signature", "Love, Leeroy."),
            };
            foreach (var keyValuePair in values)
            {
                form.Add(new StringContent(keyValuePair.Value),
                    String.Format("\"{0}\"", keyValuePair.Key));
            }
            HttpResponseMessage res = client.PostAsync(@"http://localhost:18096/api/uploadFile/PostFile", form).Result;
            var uploadModel = res.Content.ReadAsStringAsync().Result;
            Console.WriteLine(uploadModel);
        }
        public static void UploadFiles(){
            string fName = @"D:\4.txt";
            string url = @"http://localhost:18096/api/uploadFile/ResumFile";
            FileStream fStream = new FileStream(fName, FileMode.Open, FileAccess.Read);
            var mdfstr = GetStreamMd5(fStream);
            fStream.Close();
            var startpoint = isResume(mdfstr, Path.GetExtension(fName));
            string msg = UpLoadFile(fName, url, 64, startpoint, mdfstr);
            Console.WriteLine(mdfstr,msg);
        }
        /// <summary>
       /// 根据文件名获取是否是续传和续传的下次开始节点
       /// </summary>
       /// <param name="md5str"></param>
       /// <param name="fileextname"></param>
       /// <returns></returns>
        private static int isResume(string md5str, string fileextname)
        {
            System.Net.WebClient WebClientObj = new System.Net.WebClient();
            var url = "http://localhost:18096/api/uploadFile/GetResumFile?md5str="+md5str+fileextname;
            byte[] byRemoteInfo = WebClientObj.DownloadData(url);
            string result = System.Text.Encoding.UTF8.GetString(byRemoteInfo);
            if(string.IsNullOrEmpty(result))
            {
                return 0;
            }
            return Convert.ToInt32(result);
 
        }
       
        /// <summary>
        /// 上传文件（自动分割）
        /// </summary>
        /// <param name="filePath">待上传的文件全路径名称</param>
        /// <param name="hostURL">服务器的地址</param>
        /// <param name="byteCount">分割的字节大小</param>        
        /// <param name="cruuent">当前字节指针</param>
        /// <returns>成功返回"";失败则返回错误信息</returns>
        public static string UpLoadFile(string filePath, string hostURL, int byteCount, long cruuent, string mdfstr)
        {
            string tmpURL = hostURL;
            byteCount = byteCount * 1024;
 
 
            System.Net.WebClient WebClientObj = new System.Net.WebClient();
            FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
 
 
            BinaryReader bReader = new BinaryReader(fStream);
            long length = fStream.Length;
            string sMsg = "上传成功";
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
            try
            {
 
                #region 续传处理
                byte[] data;
                if (cruuent > 0)
                {
                    fStream.Seek(cruuent, SeekOrigin.Current);
                }
                #endregion
 
                #region 分割文件上传
                for (; cruuent <= length; cruuent = cruuent + byteCount)
                {
                    if (cruuent + byteCount > length)
                    {
                        data = new byte[Convert.ToInt64((length - cruuent))];
                        bReader.Read(data, 0, Convert.ToInt32((length - cruuent)));
                    }
                    else
                    {
                        data = new byte[byteCount];
                        bReader.Read(data, 0, byteCount);
                    }
 
                    try
                    {
 
 
                        //***                        bytes 21010-47021/47022
                        WebClientObj.Headers.Remove(HttpRequestHeader.ContentRange);
                        WebClientObj.Headers.Add(HttpRequestHeader.ContentRange, "bytes " + cruuent + "-" + (cruuent + byteCount) + "/" + fStream.Length);
                        WebClientObj.Headers.Add("Authorization","Basic 123456");
                        hostURL = tmpURL + "?filename=" + mdfstr + Path.GetExtension(fileName);                        
                        byte[] byRemoteInfo = WebClientObj.UploadData(hostURL, "POST", data);
                        string sRemoteInfo = System.Text.Encoding.Default.GetString(byRemoteInfo);
 
                        //  获取返回信息
                        if (sRemoteInfo.Trim() != "")
                        {
                            sMsg = sRemoteInfo;
                            break;
 
                        }
                    }
                    catch (Exception ex)
                    {
                        sMsg = ex.ToString();
                        break;
                    }
                #endregion
 
                }
            }
            catch (Exception ex)
            {
                sMsg = sMsg + ex.ToString();
            }
            try
            {
                bReader.Close();
                fStream.Close();
            }
            catch (Exception exMsg)
            {
                sMsg = exMsg.ToString();
            }
 
            GC.Collect();
            return sMsg;
        }
        public static string GetStreamMd5(Stream stream)
        {
            var oMd5Hasher = new MD5CryptoServiceProvider();
            byte[] arrbytHashValue = oMd5Hasher.ComputeHash(stream);
            //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
            string strHashData = BitConverter.ToString(arrbytHashValue);
            //替换-
            strHashData = strHashData.Replace("-", "");
            string strResult = strHashData;
            return strResult;
        }
        /// <summary>
        /// 通过HashAlgorithm的TransformBlock方法对流进行叠加运算获得MD5
        /// 实现稍微复杂，但可使用与传输文件或接收文件时同步计算MD5值
        /// 可自定义缓冲区大小，计算速度较快
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <returns>MD5Hash</returns>
        public static string getMD5ByHashAlgorithm(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));
            int bufferSize = 1024 * 16;//自定义缓冲区大小16K
            byte[] buffer = new byte[bufferSize];
            Stream inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
            int readLength = 0;//每次读取长度
            var output = new byte[bufferSize];
            while ((readLength = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                //计算MD5
                hashAlgorithm.TransformBlock(buffer, 0, readLength, output, 0);
            }
            //完成最后计算，必须调用(由于上一部循环已经完成所有运算，所以调用此方法时后面的两个参数都为0)
            hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
            string md5 = BitConverter.ToString(hashAlgorithm.Hash);            
            hashAlgorithm.Clear();
            inputStream.Close();
            md5 = md5.Replace("-", "");
            return md5;
        }
    }
}