using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using XXCWEBAPI.Models;
using XXCWEBAPI.Utils;

namespace XXCWEBAPI.Controllers
{   
    [RoutePrefix("api/visitorAccessInf")]
    public class VisitorAccessInfController : ApiController
    {
        private static readonly string privateKey = ConfigurationManager.ConnectionStrings["privatekey"].ConnectionString;
        [HttpGet, Route("getList")]
        public string GetList()
        {
            string sql = "select * from T_VisitorAccessInf";
            //string sql = "select * from T_BlacklistInf";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, System.Data.CommandType.Text, null);
                return "{\"code\":1,\"data\":" + ConvertHelper.DataTableToJson(dt) + "}"; 
            }
            catch (Exception e)
            {
                return "{\"code\":0,\"msg\":\"" + new StringContent(e.ToString()) + "\"}";
            }
        }
        [HttpGet, Route("getListByVName")]
        public string GetListByPage(string VName,int pageSize,int pageIndex)
        {
            string sql = "sp_getVisitorAccessInfByPage";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@VName",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(VName)},
                new SqlParameter("@pageSize",SqlDbType.Int){Value = pageSize},
                new SqlParameter("@pageIndex",SqlDbType.Int){Value = pageIndex},
                new SqlParameter("@count",SqlDbType.Int){Direction=ParameterDirection.Output}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.StoredProcedure, pms);
                return "{\"code\":1,\"count\":" + pms[3].Value.ToString() + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpGet, Route("getListByMore")]
        public string GetListByMore(string VName, string VCertificateNumber, string VFromCourtId, int pageSize, int pageIndex)
        {
            string sql = "sp_getVisitorAccessInfByPage1";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@VName",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(VName)},
                new SqlParameter("@VCertificateNumber",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(VCertificateNumber)},
                new SqlParameter("@VFromCourtId",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(VFromCourtId)},
                new SqlParameter("@pageSize",SqlDbType.Int){Value = pageSize},
                new SqlParameter("@pageIndex",SqlDbType.Int){Value = pageIndex},
                new SqlParameter("@count",SqlDbType.Int){Direction=ParameterDirection.Output}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.StoredProcedure, pms);
                return "{\"code\":1,\"count\":" + pms[5].Value.ToString() + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("getListByTimeAndTop")]
        /// <summary>
        /// flag (time:按照时间查询 top:按照次数查询)
        /// </summary>
        public string GetListByTimeAndTop(VisitorSearchModel v)
        {
            string Time = DataHelper.IsNullReturnLine(v.Time);
            string VCertificateNumber = DataHelper.IsNullReturnLine(v.VCertificateNumber);
            string Top = DataHelper.IsNullReturnLine(v.Top);
            string Flag = DataHelper.IsNullReturnLine(v.Flag);
            string sql = "";
            SqlParameter[] pms = null;
            if (string.IsNullOrEmpty(VCertificateNumber))
            {
                return ConvertHelper.resultJson(0, "证件号不能为空");
            }
            if (Flag == "time")
            {
                if (string.IsNullOrEmpty(Time)) {
                    return ConvertHelper.resultJson(0, "请选择时间");
                }
                string timeStart = Time.Split(' ')[0] + " 00:00:00";
                string timeEnd = Time.Split(' ')[1] + " 23:59:59";
                pms = new SqlParameter[]{
                    new SqlParameter("@timeStart",SqlDbType.NVarChar){Value = (timeStart)},
                    new SqlParameter("@timeEnd",SqlDbType.NVarChar){Value = (timeEnd)},
                    new SqlParameter("@VCertificateNumber",SqlDbType.NVarChar){Value = (VCertificateNumber)}
                };
                sql += "select v.VId,v.VName,v.VSex,v.VNation,v.VBirthDate,v.VAddress,v.VIssuingAuthority,v.VExpiryDate,v.VCertificateType,v.VCertificateNumber,v.VType,v.VFromCourtId,v.VInTime,v.VOutTime,v.VInPost,v.VOutPost,v.VInDoorkeeper,v.VOutDoorkeeper,v.VVisitingReason,v.VIntervieweeDept,v.VInterviewee,v.VOffice,v.VOfficePhone,v.VExtensionPhone,v.VMobilePhone,v.VRemark,c.CName from T_VisitorAccessInf v ";
                sql += "left join T_CourtInf c ";
                sql += "on v.VFromCourtId = c.CNumber ";
                sql += "where v.VInTime between @timeStart and @timeEnd ";
                sql += "and v.VCertificateNumber=@VCertificateNumber ";
                sql += "order by v.VId desc";
            }
            else if (Flag == "top")
            {
                if (string.IsNullOrEmpty(Top.ToString()))
                {
                    return ConvertHelper.resultJson(0, "次数不能为空");
                }
                pms = new SqlParameter[]{
                    new SqlParameter("@Top",SqlDbType.Int){Value = (Convert.ToInt32(Top))},
                    new SqlParameter("@VCertificateNumber",SqlDbType.NVarChar){Value = (VCertificateNumber)}
                };
                sql += "select top (@Top) v.VId,v.VName,v.VSex,v.VNation,v.VBirthDate,v.VAddress,v.VIssuingAuthority,v.VExpiryDate,v.VCertificateType,v.VCertificateNumber,v.VType,v.VFromCourtId,v.VInTime,v.VOutTime,v.VInPost,v.VOutPost,v.VInDoorkeeper,v.VOutDoorkeeper,v.VVisitingReason,v.VIntervieweeDept,v.VInterviewee,v.VOffice,v.VOfficePhone,v.VExtensionPhone,v.VMobilePhone,v.VRemark,c.CName from T_VisitorAccessInf v ";
                sql += "left join T_CourtInf c ";
                sql += "on v.VFromCourtId = c.CNumber ";
                sql += "where v.VCertificateNumber=@VCertificateNumber ";
                sql += "order by v.VId desc";
            }
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
                //return ConvertHelper.DataTableToJson(dt);
                //return dt;
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpGet, Route("getVTrackByCertificateNumber")]
        /// <summary>
        /// flag (time:按照时间查询 top:按照次数查询)
        /// </summary>
        public string GetVTrackByCertificateNumber(string VCertificateNumber)
        {
            if (string.IsNullOrEmpty(VCertificateNumber))
            {
                return ConvertHelper.resultJson(0, "证件号不能为空！");
            }
            string sql = "";
            sql += "select v.VId,v.VInTime,c.CName ";
            sql += "from T_VisitorAccessInf v ";
            sql += "left join T_CourtInf c ";
            sql += "on v.VFromCourtId = c.CNumber ";
            sql += "where v.VCertificateNumber = @VCertificateNumber ";
            sql += "order by v.VInTime desc";
            SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@VCertificateNumber",SqlDbType.NVarChar){Value=VCertificateNumber}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
                return "{\"code\":1,\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
                //return ConvertHelper.DataTableToJson(dt);
                //return dt;
            }
            catch (Exception e)
            {
                //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp);
            }
        }
        [HttpPost, Route("addInf4WF")]
        public string AddInf(VisitorAccessInf v)
        {
            string wramStr = "";
            if (v.VName == "" || v.VName == null)
            {
                wramStr = "姓名不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.VAddress == "" || v.VAddress == null)
            {
                wramStr = "身份证中的住址不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.VCertificateNumber == "" || v.VCertificateNumber == null)
            {
                wramStr = "证件号不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            
            //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
            string name = v.VName.Replace(" ","+");
            string address = v.VAddress.Replace(" ","+");
            string certificateNumber = v.VCertificateNumber.Replace(" ","+");
            
            string p = "";
            p += "VName=" + name;
            p += "&VSex=" + v.VSex;
            p += "&VNation=" + v.VNation;
            p += "&VBirthDate=" + v.VBirthDate;
            p += "&VAddress=" + address;
            p += "&VIssuingAuthority=" + v.VIssuingAuthority;
            p += "&VExpiryDate=" + v.VExpiryDate;
            p += "&VCertificatePhoto=" + DataHelper.IsNullReturnLine(v.VCertificatePhoto,true);
            p += "&VLocalePhoto=" + DataHelper.IsNullReturnLine(v.VLocalePhoto,true);
            p += "&VCertificateType=" + v.VCertificateType;
            p += "&VCertificateNumber=" + certificateNumber;
            p += "&VType=" + v.VType;
            p += "&VFromCourtId=" + v.VFromCourtId;
            p += "&VInTime=" + v.VInTime;
            p += "&VOutTime=" + v.VOutTime;
            p += "&VInPost=" + v.VInPost;
            p += "&VOutPost=" + v.VOutPost;
            p += "&VInDoorkeeper=" + v.VInDoorkeeper;
            p += "&VOutDoorkeeper=" + v.VOutDoorkeeper;
            p += "&VVisitingReason=" + v.VVisitingReason;
            p += "&VIntervieweeDept=" + v.VIntervieweeDept;
            p += "&VInterviewee=" + v.VInterviewee;
            p += "&VOffice=" + v.VOffice;
            p += "&VOfficePhone=" + v.VOfficePhone;
            p += "&VExtensionPhone=" + v.VExtensionPhone;
            p += "&VMobilePhone=" + v.VMobilePhone;
            p += "&VRemark=" + v.VRemark;

            string md5Ciphertext = v.VMD5Ciphertext;//对方传过来的所有字段的MD5密文
            //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
            string md5P = MD5Helper._md5(p);
            if (md5Ciphertext == md5P)
            {
                string sql = "sp_addVisitorAccessInf";
                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@VName",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey, name)},
                    new SqlParameter("@VSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VSex)},
                    new SqlParameter("@VNation",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VNation)},
                    new SqlParameter("@VBirthDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VBirthDate)},
                    new SqlParameter("@VAddress",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey,address)},
                    new SqlParameter("@VIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VIssuingAuthority)},
                    new SqlParameter("@VExpiryDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VExpiryDate)},
                    new SqlParameter("@VCertificatePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VCertificatePhoto,true)},
                    new SqlParameter("@VLocalePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VLocalePhoto,true)},
                    new SqlParameter("@VCertificateType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VCertificateType)},
                    new SqlParameter("@VCertificateNumber",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey,certificateNumber)},
                    new SqlParameter("@VType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VType)},
                    new SqlParameter("@VFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VFromCourtId)},
                    new SqlParameter("@VInTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInTime)},
                    new SqlParameter("@VOutTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOutTime)},
                    new SqlParameter("@VInPost",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInPost)},
                    new SqlParameter("@VOutPost",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOutPost)},
                    new SqlParameter("@VInDoorkeeper",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInDoorkeeper)},
                    new SqlParameter("@VOutDoorkeeper",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOutDoorkeeper)},
                    new SqlParameter("@VVisitingReason",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VVisitingReason)},
                    new SqlParameter("@VIntervieweeDept",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VIntervieweeDept)},
                    new SqlParameter("@VInterviewee",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInterviewee)},
                    new SqlParameter("@VOffice",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOffice)},
                    new SqlParameter("@VOfficePhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOfficePhone)},
                    new SqlParameter("@VExtensionPhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VExtensionPhone)},
                    new SqlParameter("@VMobilePhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VMobilePhone)},
                    new SqlParameter("@VRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VRemark)}
                };
                try
                {
                    int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.StoredProcedure, pms);
                    return ConvertHelper.IntToJson(result);
                }
                catch (Exception e)
                {
                    //在webapi中要想抛出异常必须这样抛出，否则只抛出一个默认500的异常
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.ToString()),
                        ReasonPhrase = "error"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            else {
                return ConvertHelper.resultJson(0,"数据传输过程中被篡改");
            }
        }
        
        [HttpPost, Route("editInf")]
        public string EditInf(VisitorAccessInf v)
        {
            //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
            string sql = "update T_VisitorAccessInf set VName=@VName,VSex=@VSex,VNation=@VNation,VBirthDate=@VBirthDate,VAddress=@VAddress,";
            sql += "VIssuingAuthority=@VIssuingAuthority,VExpiryDate=@VExpiryDate,VCertificatePhoto=@VCertificatePhoto,VLocalePhoto=@VLocalePhoto,VCertificateType=@VCertificateType,VCertificateNumber=@VCertificateNumber,VType=@VType,VFromCourtId=@VFromCourtId,";
            sql += "VInTime=@VInTime,VOutTime=@VOutTime,VInPost=@VInPost,VOutPost=@VOutPost,VInDoorkeeper=@VInDoorkeeper,VOutDoorkeeper=@VOutDoorkeeper,VVisitingReason=@VVisitingReason,VIntervieweeDept=@VIntervieweeDept,VInterviewee=@VInterviewee,";
            sql += "VOffice=@VOffice,VOfficePhone=@VOfficePhone,VExtensionPhone=@VExtensionPhone,VMobilePhone=@VMobilePhone,VRemark=@VRemark";
            sql += " where VId=@VId";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@VName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VName)},
                new SqlParameter("@VSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VSex)},
                new SqlParameter("@VNation",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VNation)},
                new SqlParameter("@VBirthDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VBirthDate)},
                new SqlParameter("@VAddress",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VAddress)},
                new SqlParameter("@VIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VIssuingAuthority)},
                new SqlParameter("@VExpiryDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VExpiryDate)},
                new SqlParameter("@VCertificatePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VCertificatePhoto,true)},
                new SqlParameter("@VLocalePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VLocalePhoto,true)},
                new SqlParameter("@VCertificateType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VCertificateType)},
                new SqlParameter("@VCertificateNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VCertificateNumber)},
                new SqlParameter("@VType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VType)},
                new SqlParameter("@VFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VFromCourtId)},
                new SqlParameter("@VInTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInTime)},
                new SqlParameter("@VOutTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOutTime)},
                new SqlParameter("@VInPost",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInPost)},
                new SqlParameter("@VOutPost",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOutPost)},
                new SqlParameter("@VInDoorkeeper",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInDoorkeeper)},
                new SqlParameter("@VOutDoorkeeper",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOutDoorkeeper)},
                new SqlParameter("@VVisitingReason",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VVisitingReason)},
                new SqlParameter("@VIntervieweeDept",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VIntervieweeDept)},
                new SqlParameter("@VInterviewee",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VInterviewee)},
                new SqlParameter("@VOffice",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOffice)},
                new SqlParameter("@VOfficePhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VOfficePhone)},
                new SqlParameter("@VExtensionPhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VExtensionPhone)},
                new SqlParameter("@VMobilePhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VMobilePhone)},
                new SqlParameter("@VRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VRemark)},
                new SqlParameter("@VId",SqlDbType.Int){Value=v.VId}
            };
            try
            {
                int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                return ConvertHelper.IntToJson(result);
            }
            catch (Exception e) {
                //在webapi中要想抛出异常必须这样抛出，否则之抛出一个默认500的异常
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString()),
                    ReasonPhrase = "error"
                };
                throw new HttpResponseException(resp); 
            }
            
        }
        [HttpPost, Route("deleteInfById")]
        public string DeleteInfById(VisitorAccessInf v)
        {
            if (v.Token == DataHelper.getToken())
            {
                //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
                string sql = "delete from T_VisitorAccessInf where VId=@VId";
                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@VId",SqlDbType.Int){Value=v.VId}
                };
                try
                {
                    int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                    return ConvertHelper.IntToJson(result);
                }
                catch (Exception e)
                {
                    //在webapi中要想抛出异常必须这样抛出，否则之抛出一个默认500的异常
                    var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(e.ToString()),
                        ReasonPhrase = "error"
                    };
                    throw new HttpResponseException(resp);
                }
            }
            else
            {
                return ConvertHelper.resultJson(101, "权限受限！");
            }
            
        }
    }
}
