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
        private string _NowTime;
        /// <summary>
        /// 
        /// </summary>
        public string NowTime
        {
            set { _NowTime = value; }
            get { return _NowTime; }
        }
        private string _Phone;
        public string Phone
        {
            set { _Phone = value; }
            get { return _Phone; }
        }
        private string _PUPPhone;
        public string PUPPhone
        {
            set { _PUPPhone = value; }
            get { return _PUPPhone; }
        }
        private string _SMPhone;
        public string SMPhone
        {
            set { _SMPhone = value; }
            get { return _SMPhone; }
        }
        private string _Role;
        public string Role
        {
            set { _Role = value; }
            get { return _Role; }
        }
        private string _appid;
        public string appid
        {
            set { _appid = value; }
            get { return _appid; }
        }
        private string _secret;
        public string secret
        {
            set { _secret = value; }
            get { return _secret; }
        }
        private string _js_code;
        public string js_code
        {
            set { _js_code = value; }
            get { return _js_code; }
        }
        private string _access_token;
        public string access_token
        {
            set { _access_token = value; }
            get { return _access_token; }
        }
        private string _openid;
        public string openid
        {
            set { _openid = value; }
            get { return _openid; }
        }
        private string _form_id;
        public string form_id
        {
            set { _form_id = value; }
            get { return _form_id; }
        }
        private string _template_id;
        public string template_id
        {
            set { _template_id = value; }
            get { return _template_id; }
        }
        private string _name;
        public string name
        {
            set { _name = value; }
            get { return _name; }
        }
        private string _sname;
        public string sname
        {
            set { _sname = value; }
            get { return _sname; }
        }
        private string _SDDetailName;
        public string SDDetailName
        {
            set { _SDDetailName = value; }
            get { return _SDDetailName; }
        }
        private string _ordercode;
        public string ordercode
        {
            set { _ordercode = value; }
            get { return _ordercode; }
        }
    }
}