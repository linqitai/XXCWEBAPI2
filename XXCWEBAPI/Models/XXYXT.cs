using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
namespace XXCWEBAPI.Models
{
	[Serializable]
	public partial class XXYXT
	{
        public XXYXT()
		{
			// 在此点下面插入创建对象所需的代码。
		}
		private int? _Id;
        public int? Id
        {
            set { _Id = value; }
            get { return _Id; }
        }
        private string _SName;
        public string SName
        {
            set { _SName = value; }
            get { return _SName; }
        }
        private string _DName;
        public string DName
        {
            set { _DName = value; }
            get { return _DName; }
        }
        private string _ClassName;
        public string ClassName
        {
            set { _ClassName = value; }
            get { return _ClassName; }
        }
        private string _CreateTime;
        public string CreateTime
        {
            set { _CreateTime = value; }
            get { return _CreateTime; }
        }
        private string _CreateDate;
        public string CreateDate
        {
            set { _CreateDate = value; }
            get { return _CreateDate; }
        }
        private string _SubDate;
        public string SubDate
        {
            set { _SubDate = value; }
            get { return _SubDate; }
        }
        private string _ReceptionistPhone;
        public string ReceptionistPhone
        {
            set { _ReceptionistPhone = value; }
            get { return _ReceptionistPhone; }
        }

        private string _ReadHeadNote;
        public string ReadHeadNote
        {
            set { _ReadHeadNote = value; }
            get { return _ReadHeadNote; }
        }

        private string _Form_id;
        public string Form_id
        {
            set { _Form_id = value; }
            get { return _Form_id; }
        }
        private string _Form_ids;
        public string Form_ids
        {
            set { _Form_ids = value; }
            get { return _Form_ids; }
        }
        private string _OpenId;
        public string OpenId
        {
            set { _OpenId = value; }
            get { return _OpenId; }
        }
        private string _SActualNo;
        public string SActualNo
        {
            set { _SActualNo = value; }
            get { return _SActualNo; }
        }

        private string _SurrogateMPhone;
        public string SurrogateMPhone
        {
            set { _SurrogateMPhone = value; }
            get { return _SurrogateMPhone; }
        }
	}
}