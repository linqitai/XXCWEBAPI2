using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace XXCWEBAPI.Utils
{
    public static class DataHelper
    {
        /// <summary>
        /// 返回Token
        /// </summary>
        public static string getToken()
        {
            return AESHelper.AesEncrypt(DateTime.Now.ToString("yyyy-MM-dd"));
        }
        /// <summary>
        /// 判断该字段是否为空，空则返回""
        /// </summary>
        /// <param name="str">str</param>
        /// <param name="dealSpace">是否要把空格转换成+号</param>
        /// <returns></returns>
        public static string IsNullReturnLine(string str,bool dealSpace=false)
        {
            if (string.IsNullOrEmpty(str)||str=="null")
            {
                return "";
            }
            else {
                if (dealSpace) {
                    return str.Trim().Replace(" ", "+");
                }
                else
                {
                    return str.Trim();   
                }
                
            }
        }
        //private static string skey = "$IロQ＇I`A^";//12345`78876543.1123456788`654abc
        private static string skey = "12345678876543211234567887654abc";//12345678876543211234567887654abc
        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesEncrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return "--";
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(skey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public static string AesDecrypt(string str)
        {
            if (string.IsNullOrEmpty(str)) return "--";
            Byte[] toEncryptArray = Convert.FromBase64String(str);

            RijndaelManaged rm = new RijndaelManaged
            {
                Key = Encoding.UTF8.GetBytes(skey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
    }
}