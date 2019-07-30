using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XXCWEBAPI.Utils
{
    public class HttpHelper
    {
        //post请求并调用
 
        //Dictionary<string, string> dic = new Dictionary<string, string>();
        //dic.Add("id", "4");
        //textBox1.Text = GetResponseString(CreatePostHttpResponse("https://www.baidu.com/", dic));
        //get请求并调用
 
        //textBox3.Text = GetResponseString(CreateGetHttpResponse("https://i.cnblogs.com/EditPosts.aspx?opt=1"));
        /// <summary>
        /// 发送http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebResponse CreateGetHttpResponse(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
            return request.GetResponse() as HttpWebResponse;
        }
        /// <summary>
        /// 从HttpWebResponse对象中提取响应的数据转换为字符串
        /// </summary>
        /// <param name="webresponse"></param>
        /// <returns></returns>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
        #region//接口转入统一方式为HTTP POST
        /// <summary>
        /// 接口转入统一方式为HTTP POST
        /// </summary>
        /// <param name="para_strPath">HTTP地址</param>
        /// <param name="para_strPostData">请求的数据</param>
        /// <returns>HttpPost</returns>
        public static string HttpPost(string para_strUrl, string para_strPostData)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(para_strPostData); //转化为UTF8
                HttpWebRequest webReq = null;

                webReq = (HttpWebRequest)WebRequest.Create(new Uri(para_strUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        int errorcode = (int)response.StatusCode;
                        ret = errorcode + "," + ex.Message;
                    }
                    else
                    {
                        // no http status code available
                        ret = ex.Message;
                    }
                }
                else
                {
                    // no http status code available
                    ret = ex.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }
        public static string HttpGet(string para_strUrl)
        {
            string ret = string.Empty;
            try
            {
                HttpWebRequest webReq = null;

                webReq = (HttpWebRequest)WebRequest.Create(new Uri(para_strUrl));
                webReq.Method = "GET";
                webReq.ContentType = "application/x-www-form-urlencoded";
                
                Stream newStream = webReq.GetRequestStream();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        int errorcode = (int)response.StatusCode;
                        ret = errorcode + "," + ex.Message;
                    }
                    else
                    {
                        // no http status code available
                        ret = ex.Message;
                    }
                }
                else
                {
                    // no http status code available
                    ret = ex.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }
        #endregion
    }
}