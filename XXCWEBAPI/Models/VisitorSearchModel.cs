using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Models
{
    /// <summary>
    /// [VisitorSearchModel]表实体类
    /// 作者:linqitai
    /// 创建时间:2019-05-22 14:34:19
    /// </summary>
    [Serializable]
    public partial class VisitorSearchModel
    {
        public VisitorSearchModel()
        { }
        private string _Time;
        /// <summary>
        /// 
        /// </summary>
        public string Time
        {
            set { _Time = value; }
            get { return _Time; }
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
        private string _Top;
        /// <summary>
        /// 
        /// </summary>
        public string Top
        {
            set { _Top = value; }
            get { return _Top; }
        }
        private string _Flag;
        /// <summary>
        /// 
        /// </summary>
        public string Flag
        {
            set { _Flag = value; }
            get { return _Flag; }
        }
    }
}