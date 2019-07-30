using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [T_UserInf]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class UserInf
    {
        public UserInf()
        { }
        private int? _UId;
        /// <summary>
        /// 
        /// </summary>
        public int? UId
        {
            set { _UId = value; }
            get { return _UId; }
        }
        private string _UserName;
        /// <summary>
        /// 
        /// </summary>
        public string UserName
        {
            set { _UserName = value; }
            get { return _UserName; }
        }
        private string _UPassword;
        /// <summary>
        /// 
        /// </summary>
        public string UPassword
        {
            set { _UPassword = value; }
            get { return _UPassword; }
        }
        private string _URole;
        /// <summary>
        /// 
        /// </summary>
        public string URole
        {
            set { _URole = value; }
            get { return _URole; }
        }
        private string _UPhone;
        /// <summary>
        /// 
        /// </summary>
        public string UPhone
        {
            set { _UPhone = value; }
            get { return _UPhone; }
        }
        private string _URealName;
        /// <summary>
        /// 
        /// </summary>
        public string URealName
        {
            set { _URealName = value; }
            get { return _URealName; }
        }
        private string _MD5Ciphertext;
        /// <summary>
        /// 
        /// </summary>
        public string MD5Ciphertext
        {
            set { _MD5Ciphertext = value; }
            get { return _MD5Ciphertext; }
        }
        private string _token;
        /// <summary>
        /// 
        /// </summary>
        public string Token
        {
            set { _token = value; }
            get { return _token; }
        }
        private string _OldPassword;
        /// <summary>
        /// 
        /// </summary>
        public string OldPassword
        {
            set { _OldPassword = value; }
            get { return _OldPassword; }
        }
        private string _NewPassword;
        /// <summary>
        /// 
        /// </summary>
        public string NewPassword
        {
            set { _NewPassword = value; }
            get { return _NewPassword; }
        }
    }
}