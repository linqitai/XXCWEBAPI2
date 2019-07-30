using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [T_VisitorAccessInf]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class VisitorAccessInf
    {
        public VisitorAccessInf()
        { }
        private int? _VId;
        /// <summary>
        /// 
        /// </summary>
        public int? VId
        {
            set { _VId = value; }
            get { return _VId; }
        }
        private string _VName;
        /// <summary>
        /// 
        /// </summary>
        public string VName
        {
            set { _VName = value; }
            get { return _VName; }
        }
        private string _VSex;
        /// <summary>
        /// 
        /// </summary>
        public string VSex
        {
            set { _VSex = value; }
            get { return _VSex; }
        }
        private string _VNation;
        /// <summary>
        /// 
        /// </summary>
        public string VNation
        {
            set { _VNation = value; }
            get { return _VNation; }
        }
        private string _VBirthDate;
        /// <summary>
        /// 
        /// </summary>
        public string VBirthDate
        {
            set { _VBirthDate = value; }
            get { return _VBirthDate; }
        }
        private string _VAddress;
        /// <summary>
        /// 
        /// </summary>
        public string VAddress
        {
            set { _VAddress = value; }
            get { return _VAddress; }
        }
        private string _VIssuingAuthority;
        /// <summary>
        /// 
        /// </summary>
        public string VIssuingAuthority
        {
            set { _VIssuingAuthority = value; }
            get { return _VIssuingAuthority; }
        }
        private string _VExpiryDate;
        /// <summary>
        /// 
        /// </summary>
        public string VExpiryDate
        {
            set { _VExpiryDate = value; }
            get { return _VExpiryDate; }
        }
        private string _VCertificatePhoto;
        /// <summary>
        /// 
        /// </summary>
        public string VCertificatePhoto
        {
            set { _VCertificatePhoto = value; }
            get { return _VCertificatePhoto; }
        }
        private string _VLocalePhoto;
        /// <summary>
        /// 
        /// </summary>
        public string VLocalePhoto
        {
            set { _VLocalePhoto = value; }
            get { return _VLocalePhoto; }
        }
        private string _VCertificateType;
        /// <summary>
        /// 
        /// </summary>
        public string VCertificateType
        {
            set { _VCertificateType = value; }
            get { return _VCertificateType; }
        }
        private string _VCertificateNumber;
        /// <summary>
        /// 
        /// </summary>
        public string VCertificateNumber
        {
            set { _VCertificateNumber = value; }
            get { return _VCertificateNumber; }
        }
        private string _VType;
        /// <summary>
        /// 
        /// </summary>
        public string VType
        {
            set { _VType = value; }
            get { return _VType; }
        }
        private string _VFromCourtId;
        /// <summary>
        /// 
        /// </summary>
        public string VFromCourtId
        {
            set { _VFromCourtId = value; }
            get { return _VFromCourtId; }
        }
        private string _VInTime;
        /// <summary>
        /// 
        /// </summary>
        public string VInTime
        {
            set { _VInTime = value; }
            get { return _VInTime; }
        }
        private string _VOutTime;
        /// <summary>
        /// 
        /// </summary>
        public string VOutTime
        {
            set { _VOutTime = value; }
            get { return _VOutTime; }
        }
        private string _VInPost;
        /// <summary>
        /// 
        /// </summary>
        public string VInPost
        {
            set { _VInPost = value; }
            get { return _VInPost; }
        }
        private string _VOutPost;
        /// <summary>
        /// 
        /// </summary>
        public string VOutPost
        {
            set { _VOutPost = value; }
            get { return _VOutPost; }
        }
        private string _VInDoorkeeper;
        /// <summary>
        /// 
        /// </summary>
        public string VInDoorkeeper
        {
            set { _VInDoorkeeper = value; }
            get { return _VInDoorkeeper; }
        }
        private string _VOutDoorkeeper;
        /// <summary>
        /// 
        /// </summary>
        public string VOutDoorkeeper
        {
            set { _VOutDoorkeeper = value; }
            get { return _VOutDoorkeeper; }
        }
        private string _VVisitingReason;
        /// <summary>
        /// 
        /// </summary>
        public string VVisitingReason
        {
            set { _VVisitingReason = value; }
            get { return _VVisitingReason; }
        }
        private string _VIntervieweeDept;
        /// <summary>
        /// 
        /// </summary>
        public string VIntervieweeDept
        {
            set { _VIntervieweeDept = value; }
            get { return _VIntervieweeDept; }
        }
        private string _VInterviewee;
        /// <summary>
        /// 
        /// </summary>
        public string VInterviewee
        {
            set { _VInterviewee = value; }
            get { return _VInterviewee; }
        }
        private string _VOffice;
        /// <summary>
        /// 
        /// </summary>
        public string VOffice
        {
            set { _VOffice = value; }
            get { return _VOffice; }
        }
        private string _VOfficePhone;
        /// <summary>
        /// 
        /// </summary>
        public string VOfficePhone
        {
            set { _VOfficePhone = value; }
            get { return _VOfficePhone; }
        }
        private string _VExtensionPhone;
        /// <summary>
        /// 
        /// </summary>
        public string VExtensionPhone
        {
            set { _VExtensionPhone = value; }
            get { return _VExtensionPhone; }
        }
        private string _VMobilePhone;
        /// <summary>
        /// 
        /// </summary>
        public string VMobilePhone
        {
            set { _VMobilePhone = value; }
            get { return _VMobilePhone; }
        }
        private string _VRemark;
        /// <summary>
        /// 
        /// </summary>
        public string VRemark
        {
            set { _VRemark = value; }
            get { return _VRemark; }
        }
        private string _VMD5Ciphertext;
        /// <summary>
        /// 
        /// </summary>
        public string VMD5Ciphertext
        {
            set { _VMD5Ciphertext = value; }
            get { return _VMD5Ciphertext; }
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