using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using XXCWEBAPI.Utils;

namespace XXCWEBAPI.Models
{
    public class WeChat
    {
        string appid = ConfigurationManager.ConnectionStrings["appid"].ConnectionString;
        string secret = ConfigurationManager.ConnectionStrings["secret"].ConnectionString;
        public class AccessToken
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public DateTime CreateDate { get; set; }


        }
        public string SendTemplete(string access_token, string temid, string touser, string form_id, string page, object data)
        {
            var postUrl = string.Format("https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}", access_token);
            var msgData = new
            {
                touser = touser,
                template_id = temid,
                form_id = form_id,
                page = page,
                data = data
            };
            JavaScriptSerializer js = new JavaScriptSerializer();

            var j = js.Serialize(msgData);
            HttpHelper2 http = new HttpHelper2();
            string res = GetDataByPost(postUrl, j);
            return res;
        }
        private string GetDataByPost(string url, string postData = "")
        {
            string result = "";
            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(postData);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
