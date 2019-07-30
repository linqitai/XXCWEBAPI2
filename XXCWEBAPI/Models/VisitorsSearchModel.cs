using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [VisitorsSearchModel]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class VisitorsSearchModel
    {
        public VisitorsSearchModel()
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
        private string _StartDate;
        /// <summary>
        /// 
        /// </summary>
        public string StartDate
        {
            set { _StartDate = value; }
            get { return _StartDate; }
        }
        private string _EndDate;
        /// <summary>
        /// 
        /// </summary>
        public string EndDate
        {
            set { _EndDate = value; }
            get { return _EndDate; }
        }
        private string _Checker;
        /// <summary>
        /// 
        /// </summary>
        public string Checker
        {
            set { _Checker = value; }
            get { return _Checker; }
        }
        private string _Name;
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
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
        private string _Type;
        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            set { _Type = value; }
            get { return _Type; }
        }
        private string _CheckStatus;
        /// <summary>
        /// 
        /// </summary>
        public string CheckStatus
        {
            set { _CheckStatus = value; }
            get { return _CheckStatus; }
        }
        private string _SMPhone;
        /// <summary>
        /// 
        /// </summary>
        public string SMPhone
        {
            set { _SMPhone = value; }
            get { return _SMPhone; }
        }
        private string _SInitialPassword;
        /// <summary>
        /// 
        /// </summary>
        public string SInitialPassword
        {
            set { _SInitialPassword = value; }
            get { return _SInitialPassword; }
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
        private string _appid;
        /// <summary>
        /// 
        /// </summary>
        public string appid
        {
            set { _appid = value; }
            get { return _appid; }
        }
        private string _secret;
        /// <summary>
        /// 
        /// </summary>
        public string secret
        {
            set { _secret = value; }
            get { return _secret; }
        }
        private string _js_code;
        /// <summary>
        /// 
        /// </summary>
        public string js_code
        {
            set { _js_code = value; }
            get { return _js_code; }
        }
        private string _grant_type;
        /// <summary>
        /// 
        /// </summary>
        public string grant_type
        {
            set { _grant_type = value; }
            get { return _grant_type; }
        }
        private string _OpenId;
        public string OpenId
        {
            set { _OpenId = value; }
            get { return _OpenId; }
        }
        private string _OpenId4Out;
        public string OpenId4Out
        {
            set { _OpenId4Out = value; }
            get { return _OpenId4Out; }
        }
        private string _OpenId4In;
        public string OpenId4In
        {
            set { _OpenId4In = value; }
            get { return _OpenId4In; }
        }
    }
}