using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [T_CourtInf]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class CourtInf
    {
        public CourtInf()
        { }
        private int? _CId;
        /// <summary>
        /// 
        /// </summary>
        public int? CId
        {
            set { _CId = value; }
            get { return _CId; }
        }
        private string _CNumber;
        /// <summary>
        /// 
        /// </summary>
        public string CNumber
        {
            set { _CNumber = value; }
            get { return _CNumber; }
        }
        private string _CName;
        /// <summary>
        /// 
        /// </summary>
        public string CName
        {
            set { _CName = value; }
            get { return _CName; }
        }
        private string _CLinkman;
        /// <summary>
        /// 
        /// </summary>
        public string CLinkman
        {
            set { _CLinkman = value; }
            get { return _CLinkman; }
        }
        private string _CWorkTelephone;
        /// <summary>
        /// 
        /// </summary>
        public string CWorkTelephone
        {
            set { _CWorkTelephone = value; }
            get { return _CWorkTelephone; }
        }
        private string _CAddress;
        /// <summary>
        /// 
        /// </summary>
        public string CAddress
        {
            set { _CAddress = value; }
            get { return _CAddress; }
        }
        private string _CLongitude;
        /// <summary>
        /// 
        /// </summary>
        public string CLongitude
        {
            set { _CLongitude = value; }
            get { return _CLongitude; }
        }
        private string _CLatitude;
        /// <summary>
        /// 
        /// </summary>
        public string CLatitude
        {
            set { _CLatitude = value; }
            get { return _CLatitude; }
        }
        private string _CMD5Ciphertext;
        /// <summary>
        /// 
        /// </summary>
        public string CMD5Ciphertext
        {
            set { _CMD5Ciphertext = value; }
            get { return _CMD5Ciphertext; }
        }
    }
}