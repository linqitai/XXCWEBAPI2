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
    [RoutePrefix("api/courtInf")]
    public class CourtInfController : ApiController
    {
        private static readonly string privateKey = ConfigurationManager.ConnectionStrings["privatekey"].ConnectionString;
        [HttpGet, Route("getList")]
        public string GetList()
        {
            string sql = "select * from T_CourtInf";
            //string sql = "select * from T_BlacklistInf";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, System.Data.CommandType.Text, null);
                return "{\"code\":1,\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
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
        public string GetListByMore(string CName, int pageSize, int pageIndex)
        {
            string sql = "sp_getCourtInfByPage";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@CName",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(CName)},
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
        [HttpPost, Route("addInf4WF")]
        public string AddInf4WF(CourtInf v)
        {
            string wramStr = "";
            if (v.CNumber == "" || v.CNumber == null)
            {
                wramStr = "法院编号不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.CName == "" || v.CName == null)
            {
                wramStr = "法院名称不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.CLinkman == "" || v.CLinkman == null)
            {
                wramStr = "联系人不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.CWorkTelephone == "" || v.CWorkTelephone == null)
            {
                wramStr = "单位电话不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            string linkman = v.CLinkman.Replace(" ", "+");
            string workTelephone = v.CWorkTelephone.Replace(" ", "+");

            string p = "";
            p += "CNumber=" + v.CNumber;
            p += "&CName=" + v.CName;
            p += "&CLinkman=" + v.CLinkman;
            p += "&CWorkTelephone=" + workTelephone;
            p += "&CAddress=" + v.CAddress;
            p += "&CLongitude=" + v.CLongitude;
            p += "&CLatitude=" + v.CLatitude;

            string md5Ciphertext = v.CMD5Ciphertext;//对方传过来的所有字段的MD5密文
            //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
            string md5P = MD5Helper._md5(p);
            if (md5Ciphertext == md5P)
            {
                string sql = "insert into T_CourtInf(CNumber, CName, CLinkman, CWorkTelephone, CAddress, CLongitude, CLatitude) values(@CNumber, @CName, @CLinkman, @WorkTelephone, @CAddress, @CLongitude, @CLatitude)";
                workTelephone = RSAHelper.DecryptWithPrivateKey(privateKey, workTelephone);
                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@CNumber",SqlDbType.NVarChar){Value=v.CNumber},
                    new SqlParameter("@CName",SqlDbType.NVarChar){Value=v.CName},
                    new SqlParameter("@CLinkman",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey, linkman)},
                    new SqlParameter("@CWorkTelephone",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey, workTelephone)},
                    new SqlParameter("@CAddress",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CAddress)},
                    new SqlParameter("@CLongitude",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CLongitude)},
                    new SqlParameter("@CLatitude",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CLatitude)}
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
            else
            {
                return ConvertHelper.resultJson(0, "数据传输过程中被篡改");
            }
        }
        [HttpPost, Route("addInf4Web")]
        public string AddInf4Web(CourtInf v)
        {
            string wramStr = "";
            if (v.CNumber == "" || v.CNumber == null)
            {
                wramStr = "法院编号不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.CName == "" || v.CName == null)
            {
                wramStr = "法院名称不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.CLinkman == "" || v.CLinkman == null)
            {
                wramStr = "联系人不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.CWorkTelephone == "" || v.CWorkTelephone == null)
            {
                wramStr = "单位电话不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            string linkman = v.CLinkman.Replace(" ", "+");
            string workTelephone = v.CWorkTelephone.Replace(" ", "+");

            string p = "";
            p += "CNumber=" + v.CNumber;
            p += "&CName=" + v.CName;
            p += "&CLinkman=" + linkman;
            p += "&CWorkTelephone=" + workTelephone;
            p += "&CAddress=" + v.CAddress;
            p += "&CLongitude=" + v.CLongitude;
            p += "&CLatitude=" + v.CLatitude;

            string md5Ciphertext = v.CMD5Ciphertext;//对方传过来的所有字段的MD5密文
            //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
            string md5P = MD5Helper._md5(p);
            if (md5Ciphertext == md5P)
            {
                string sql = "insert into T_CourtInf(CNumber, CName, CLinkman, CWorkTelephone, CAddress, CLongitude, CLatitude) values(@CNumber, @CName, @CLinkman, @WorkTelephone, @CAddress, @CLongitude, @CLatitude)";
                workTelephone = RSAHelper.DecryptWithPrivateKey(privateKey, workTelephone);
                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@CNumber",SqlDbType.NVarChar){Value=v.CNumber},
                    new SqlParameter("@CName",SqlDbType.NVarChar){Value=v.CName},
                    new SqlParameter("@CLinkman",SqlDbType.NVarChar){Value= AESHelper.AesDecrypt(linkman)},
                    new SqlParameter("@CWorkTelephone",SqlDbType.NVarChar){Value=AESHelper.AesDecrypt(workTelephone)},
                    new SqlParameter("@CAddress",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CAddress)},
                    new SqlParameter("@CLongitude",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CLongitude)},
                    new SqlParameter("@CLatitude",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CLatitude)}
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
            else
            {
                return ConvertHelper.resultJson(0, "数据传输过程中被篡改");
            }
        }
        [HttpPost, Route("deleteInfById")]
        public string DeleteInfById(CourtInf B)
        {
            //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
            string sql = "delete from T_CourtInf where CId=@CId";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@CId",SqlDbType.Int){Value=B.CId}
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
    }
}
