using System;
namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [T_LawyerInf]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class LawyerInf
    {
        public LawyerInf()
        { }
        private int? _LId ;
        /// <summary>
        /// 
        /// </summary>
        public int? LId
        {
            set { _LId = value; }
            get { return _LId; }
        }
        private string _LName ;
        /// <summary>
        /// 
        /// </summary>
        public string LName
        {
            set { _LName = value; }
            get { return _LName; }
        }
        private string _LSex ;
        /// <summary>
        /// 
        /// </summary>
        public string LSex
        {
            set { _LSex = value; }
            get { return _LSex; }
        }
        private string _LPhoto;
        /// <summary>
        /// 
        /// </summary>
        public string LPhoto
        {
            set { _LPhoto = value; }
            get { return _LPhoto; }
        }
        private string _LIdentityNumber ;
        /// <summary>
        /// 
        /// </summary>
        public string LIdentityNumber
        {
            set { _LIdentityNumber = value; }
            get { return _LIdentityNumber; }
        }
        private string _LActuator ;
        /// <summary>
        /// 
        /// </summary>
        public string LActuator
        {
            set { _LActuator = value; }
            get { return _LActuator; }
        }
        private string _LPCType ;
        /// <summary>
        /// 
        /// </summary>
        public string LPCType
        {
            set { _LPCType = value; }
            get { return _LPCType; }
        }
        private string _LPCNumber ;
        /// <summary>
        /// 
        /// </summary>
        public string LPCNumber
        {
            set { _LPCNumber = value; }
            get { return _LPCNumber; }
        }
        private string _LQualifityNumber ;
        /// <summary>
        /// 
        /// </summary>
        public string LQualifityNumber
        {
            set { _LQualifityNumber = value; }
            get { return _LQualifityNumber; }
        }
        private string _LIssuingAuthority ;
        /// <summary>
        /// 
        /// </summary>
        public string LIssuingAuthority
        {
            set { _LIssuingAuthority = value; }
            get { return _LIssuingAuthority; }
        }
        private string _LIssuingDate ;
        /// <summary>
        /// 
        /// </summary>
        public string LIssuingDate
        {
            set { _LIssuingDate = value; }
            get { return _LIssuingDate; }
        }
        private string _LInTime ;
        /// <summary>
        /// 
        /// </summary>
        public string LInTime
        {
            set { _LInTime = value; }
            get { return _LInTime; }
        }
        private string _LFromCourtId;
        /// <summary>
        /// 
        /// </summary>
        public string LFromCourtId
        {
            set { _LFromCourtId = value; }
            get { return _LFromCourtId; }
        }
        private string _LRemark ;
        /// <summary>
        /// 
        /// </summary>
        public string LRemark
        {
            set { _LRemark = value; }
            get { return _LRemark; }
        }
        private string _LMD5Ciphertext;
        /// <summary>
        /// 
        /// </summary>
        public string LMD5Ciphertext
        {
            set { _LMD5Ciphertext = value; }
            get { return _LMD5Ciphertext; }
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
