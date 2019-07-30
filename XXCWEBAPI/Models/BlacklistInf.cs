using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [T_BlacklistInf]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class BlacklistInf
    {
        public BlacklistInf()
        { }
        private int? _BId;
        /// <summary>
        /// 
        /// </summary>
        public int? BId
        {
            set { _BId = value; }
            get { return _BId; }
        }
        private string _BName;
        /// <summary>
        /// 
        /// </summary>
        public string BName
        {
            set { _BName = value; }
            get { return _BName; }
        }
        private string _BSex;
        /// <summary>
        /// 
        /// </summary>
        public string BSex
        {
            set { _BSex = value; }
            get { return _BSex; }
        }
        private string _BNation;
        /// <summary>
        /// 
        /// </summary>
        public string BNation
        {
            set { _BNation = value; }
            get { return _BNation; }
        }
        private string _BBirthDate;
        /// <summary>
        /// 
        /// </summary>
        public string BBirthDate
        {
            set { _BBirthDate = value; }
            get { return _BBirthDate; }
        }
        private string _BAddress;
        /// <summary>
        /// 
        /// </summary>
        public string BAddress
        {
            set { _BAddress = value; }
            get { return _BAddress; }
        }
        private string _BIssuingAuthority;
        /// <summary>
        /// 
        /// </summary>
        public string BIssuingAuthority
        {
            set { _BIssuingAuthority = value; }
            get { return _BIssuingAuthority; }
        }
        private string _BExpiryDate;
        /// <summary>
        /// 
        /// </summary>
        public string BExpiryDate
        {
            set { _BExpiryDate = value; }
            get { return _BExpiryDate; }
        }
        private string _BCertificatePhoto;
        /// <summary>
        /// 
        /// </summary>
        public string BCertificatePhoto
        {
            set { _BCertificatePhoto = value; }
            get { return _BCertificatePhoto; }
        }
        private string _BLocalePhoto;
        /// <summary>
        /// 
        /// </summary>
        public string BLocalePhoto
        {
            set { _BLocalePhoto = value; }
            get { return _BLocalePhoto; }
        }
        private string _BCertificateType;
        /// <summary>
        /// 
        /// </summary>
        public string BCertificateType
        {
            set { _BCertificateType = value; }
            get { return _BCertificateType; }
        }
        private string _BCertificateNumber;
        /// <summary>
        /// 
        /// </summary>
        public string BCertificateNumber
        {
            set { _BCertificateNumber = value; }
            get { return _BCertificateNumber; }
        }
        private string _BCreateTime;
        /// <summary>
        /// 
        /// </summary>
        public string BCreateTime
        {
            set { _BCreateTime = value; }
            get { return _BCreateTime; }
        }
        private string _BFromCourtId;
        /// <summary>
        /// 
        /// </summary>
        public string BFromCourtId
        {
            set { _BFromCourtId = value; }
            get { return _BFromCourtId; }
        }
        private string _BLevel;
        /// <summary>
        /// 
        /// </summary>
        public string BLevel
        {
            set { _BLevel = value; }
            get { return _BLevel; }
        }
        private string _BRemark;
        /// <summary>
        /// 
        /// </summary>
        public string BRemark
        {
            set { _BRemark = value; }
            get { return _BRemark; }
        }
        private string _BMD5Ciphertext;
        /// <summary>
        /// 
        /// </summary>
        public string BMD5Ciphertext
        {
            set { _BMD5Ciphertext = value; }
            get { return _BMD5Ciphertext; }
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
    }
}