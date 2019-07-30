using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
namespace XXCWEBAPI.Models
{
	[Serializable]
	public partial class Visitors
	{
        public Visitors()
		{
			// 在此点下面插入创建对象所需的代码。
		}
		private int? _Id;
        public int? Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private string _Name;
        public string Name
        {
            set { _Name = value; }
            get { return _Name; }
        }
		private string _Sex;
		public string Sex
        {
            set { _Sex = value; }
            get { return _Sex; }
        }
		private string _Phone;
		public string Phone
        {
            set { _Phone = value; }
            get { return _Phone; }
        }
		private string _IdentityNumber;
		public string IdentityNumber
        {
            set { _IdentityNumber = value; }
            get { return _IdentityNumber; }
        }
		private string _Reason;
		public string Reason
        {
            set { _Reason = value; }
            get { return _Reason; }
        }
		private string _Number;
		public string Number
        {
            set { _Number = value; }
            get { return _Number; }
        }
		private string _PlateNumber;
		public string PlateNumber
        {
            set { _PlateNumber = value; }
            get { return _PlateNumber; }
        }
		private string _Unit;
		public string Unit
        {
            set { _Unit = value; }
            get { return _Unit; }
        }
		private string _Date;
		public string Date
        {
            set { _Date = value; }
            get { return _Date; }
        }
		private string _StartTime;
		public string StartTime
        {
            set { _StartTime = value; }
            get { return _StartTime; }
        }
		private string _EndTime;
		public string EndTime
        { 
            set { _EndTime = value; }
            get { return _EndTime; }
        }
		private string _Remark;
		public string Remark
        { 
            set { _Remark = value; }
            get { return _Remark; }
        }
		private string _Type;
		public string Type
        { 
            set { _Type = value; }
            get { return _Type; }
        }
		private string _CreateTime;
		public string CreateTime
        { 
            set { _CreateTime = value; }
            get { return _CreateTime; }
        }
		private string _DataTime;
		public string DataTime
        { 
            set { _DataTime = value; }
            get { return _DataTime; }
        }
		private string _Checker;
		public string Checker
        { 
            set { _Checker = value; }
            get { return _Checker; }
        }
		private string _CheckDate;
		public string CheckDate
        { 
            set { _CheckDate = value; }
            get { return _CheckDate; }
        }
		private string _CheckStatus;
		public string CheckStatus
        { 
            set { _CheckStatus = value; }
            get { return _CheckStatus; }
        }
        private string _SNo;
        public string SNo
        {
            set { _SNo = value; }
            get { return _SNo; }
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
        private string _EnterCode;
        public string EnterCode
        {
            set { _EnterCode = value; }
            get { return _EnterCode; }
        }
        private string _IsUseEC;
        public string IsUseEC
        {
            set { _IsUseEC = value; }
            get { return _IsUseEC; }
        }
        private string _RefuseReason;
        public string RefuseReason
        {
            set { _RefuseReason = value; }
            get { return _RefuseReason; }
        }
	}
}