using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
namespace XXCWEBAPI.Models
{
	[Serializable]
	public partial class SHFY
	{
        public SHFY()
		{
			// 在此点下面插入创建对象所需的代码。
		}
		private int? _Id;
        public int? Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private int? _ApiType;
        public int? ApiType
        {
            set { _ApiType = value; }
            get { return _ApiType; }
        }
		private string _OrderNo;
        public string OrderNo
        {
            set { _OrderNo = value; }
            get { return _OrderNo; }
        }
		private string _OrderCode;
        public string OrderCode
        {
            set { _OrderCode = value; }
            get { return _OrderCode; }
        }
        private string _VisitorName;
        public string VisitorName
        {
            set { _VisitorName = value; }
            get { return _VisitorName; }
        }
        private string _VisitorSex;
        public string VisitorSex
        {
            set { _VisitorSex = value; }
            get { return _VisitorSex; }
        }
        private string _VisitorPhone;
        public string VisitorPhone
        {
            set { _VisitorPhone = value; }
            get { return _VisitorPhone; }
        }
		private string _VisitorIdNo;
        public string VisitorIdNo
        {
            set { _VisitorIdNo = value; }
            get { return _VisitorIdNo; }
        }
        private string _VisitorReason;
        public string VisitorReason
        {
            set { _VisitorReason = value; }
            get { return _VisitorReason; }
        }
        private string _VisitorNumber;
        public string VisitorNumber
        {
            set { _VisitorNumber = value; }
            get { return _VisitorNumber; }
        }
		private string _CarNo;
        public string CarNo
        {
            set { _CarNo = value; }
            get { return _CarNo; }
        }
		private string _VisitorStartDT;
        public string VisitorStartDT
        {
            set { _VisitorStartDT = value; }
            get { return _VisitorStartDT; }
        }
        private string _VisitorEndDT;
        public string VisitorEndDT
        {
            set { _VisitorEndDT = value; }
            get { return _VisitorEndDT; }
        }
		private string _StaffNo;
        public string StaffNo
        {
            set { _StaffNo = value; }
            get { return _StaffNo; }
        }
		private string _ConferenceTitle;
        public string ConferenceTitle
        {
            set { _ConferenceTitle = value; }
            get { return _ConferenceTitle; }
        }
        private string _ConferenceTime;
        public string ConferenceTime
        {
            set { _ConferenceTime = value; }
            get { return _ConferenceTime; }
        }
        private string _ConferenceLocation;
        public string ConferenceLocation
        {
            set { _ConferenceLocation = value; }
            get { return _ConferenceLocation; }
        }
		private string _ReceptionistNo;
        public string ReceptionistNo
        {
            set { _ReceptionistNo = value; }
            get { return _ReceptionistNo; }
        }
        private string _ReceptionistName;
        public string ReceptionistName
        {
            set { _ReceptionistName = value; }
            get { return _ReceptionistName; }
        }
        private string _ReceptionistPhone;
        public string ReceptionistPhone
        {
            set { _ReceptionistPhone = value; }
            get { return _ReceptionistPhone; }
        }
	}
}