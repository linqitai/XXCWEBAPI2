using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [WXUserInfo]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class WXUserInfo
    {
        public WXUserInfo()
        { }
        private int _Id;
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private string _NickName;
        /// <summary>
        /// 
        /// </summary>
        public string NickName
        {
            set { _NickName = value; }
            get { return _NickName; }
        }
        private string _Gender;
        /// <summary>
        /// 
        /// </summary>
        public string Gender
        {
            set { _Gender = value; }
            get { return _Gender; }
        }
        private string _Province;
        /// <summary>
        /// 
        /// </summary>
        public string Province
        {
            set { _Province = value; }
            get { return _Province; }
        }
        private string _City;
        /// <summary>
        /// 
        /// </summary>
        public string City
        {
            set { _City = value; }
            get { return _City; }
        }
        private string _AvatarUrl;
        /// <summary>
        /// 
        /// </summary>
        public string AvatarUrl
        {
            set { _AvatarUrl = value; }
            get { return _AvatarUrl; }
        }
        private string _OpenId;
        /// <summary>
        /// 
        /// </summary>
        public string OpenId
        {
            set { _OpenId = value; }
            get { return _OpenId; }
        }
        private string _CreateTime;
        /// <summary>
        /// 
        /// </summary>
        public string CreateTime
        {
            set { _CreateTime = value; }
            get { return _CreateTime; }
        }
        private string _Phone;
        /// <summary>
        /// 
        /// </summary>
        public string Phone
        {
            set { _Phone = value; }
            get { return _Phone; }
        }
    }
}