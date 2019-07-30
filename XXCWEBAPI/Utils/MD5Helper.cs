using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace XXCWEBAPI.Utils
{
    public static class MD5Helper
    {
        /// <summary>
        /// ASP MD5加密算法
        /// </summary>
        /// <param name="md5str">要加密的字符串</param>
        /// <param name="type">16还是32位加密</param>
        /// <returns>Asp md5加密结果</returns>
        public static string _md5(string str)
        {
            var md5Csp = new MD5CryptoServiceProvider();
            byte[] md5Source = Encoding.UTF8.GetBytes(str);
            byte[] md5Out = md5Csp.ComputeHash(md5Source);
            string pwd = "";
            for (int i = 0; i < md5Out.Length; i++)
            {
                pwd += md5Out[i].ToString("x2");
            }
            return pwd;
        }
    }
}