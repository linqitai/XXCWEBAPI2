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
    [RoutePrefix("api/lawyerInf")]
    public class LawyerInfController : ApiController
    {
        private static readonly string privateKey = ConfigurationManager.ConnectionStrings["privatekey"].ConnectionString;
        [HttpGet, Route("getListAll")]
        public string GetListAll()
        {
            string sql = "select l.*,c.CName,Sex=case";
	        sql += " when l.LSex='1' then '男'";
	        sql += " when l.LSex='0' then '女'";
	        sql += " else '保密'";
            sql += " end";
            sql += " from T_LawyerInf l";
            sql += " left join T_CourtInf c";
            sql += " on l.LFromCourtId = c.CNumber";
            sql += " order by l.LId";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, null);
                return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
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
        public string GetListByMore(string LName, string LIdentityNumber, string LFromCourtId, int pageSize, int pageIndex)
        {
            string sql = "sp_getLawyerInfByPage1";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@LName",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(LName)},
                new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(LIdentityNumber)},
                new SqlParameter("@LFromCourtId",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(LFromCourtId)},
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
        [HttpPost, Route("addInf4WF")]
        public string AddInf4WF(LawyerInf v)
        {
            string wramStr = "";
            if (v.LName == "" || v.LName == null)
            {
                wramStr = "姓名不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.LIdentityNumber == "" || v.LIdentityNumber == null)
            {
                wramStr = "身份证号不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }

            //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
            string name = v.LName.Replace(" ", "+");
            string identityNumber = v.LIdentityNumber.Replace(" ", "+");
            string photo = DataHelper.IsNullReturnLine(v.LPhoto, true);

            string p = "";
            p += "LName=" + name;
            p += "&LSex=" + v.LSex;
            p += "&LPhoto=" + photo;
            p += "&LIdentityNumber=" + identityNumber;
            p += "&LActuator=" + v.LActuator;
            p += "&LPCType=" + v.LPCType;
            p += "&LPCNumber=" + v.LPCNumber;
            p += "&LQualifityNumber=" + v.LQualifityNumber;
            p += "&LIssuingAuthority=" + v.LIssuingAuthority;
            p += "&LIssuingDate=" + v.LIssuingDate;
            p += "&LInTime=" + v.LInTime;
            p += "&LFromCourtId=" + v.LFromCourtId;
            p += "&LRemark=" + v.LRemark;

            string md5Ciphertext = v.LMD5Ciphertext;//对方传过来的所有字段的MD5密文
            //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
            string md5P = MD5Helper._md5(p);
            if (md5Ciphertext == md5P)
            {
                string sql1 = "select count(*) from T_LawyerInf where LIdentityNumber=@LIdentityNumber";
                SqlParameter[] pms1 = new SqlParameter[]{
                    new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value = RSAHelper.DecryptWithPrivateKey(privateKey,identityNumber)}
                };
                try
                {
                    object c = SQLHelper.ExecuteScalar(sql1, System.Data.CommandType.Text, pms1);
                    if (Convert.ToInt32(c) > 0)
                    {
                        return ConvertHelper.resultJson(0, "数据库中已存在此身份证号，请勿重复添加！");
                    }
                    else
                    {
                        string sql = "sp_addLawyerInf";
                        name = RSAHelper.DecryptWithPrivateKey(privateKey, name);
                        identityNumber = RSAHelper.DecryptWithPrivateKey(privateKey, identityNumber);
                        SqlParameter[] pms = new SqlParameter[]{
                            new SqlParameter("@LName",SqlDbType.NVarChar){Value=name},
                            new SqlParameter("@LSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LSex)},
                            new SqlParameter("@LPhoto",SqlDbType.NVarChar){Value=photo},
                            new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value=identityNumber},
                            new SqlParameter("@LActuator",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LActuator)},
                            new SqlParameter("@LPCType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPCType)},
                            new SqlParameter("@LPCNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPCNumber)},
                            new SqlParameter("@LQualifityNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LQualifityNumber)},
                            new SqlParameter("@LIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LIssuingAuthority)},
                            new SqlParameter("@LIssuingDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LIssuingDate)},
                            new SqlParameter("@LInTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LInTime)},
                            new SqlParameter("@LFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LFromCourtId)},
                            new SqlParameter("@LRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LRemark)}
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
                return ConvertHelper.resultJson(0, "数据传输过程中被篡改");
            }
        }
        [HttpPost, Route("addInf4Web")]
        public string AddInf4Web(LawyerInf v)
        {
            if (v.Token == DataHelper.getToken())
            {
                string wramStr = "";
                if (v.LName == "" || v.LName == null)
                {
                    wramStr = "姓名不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }
                if (v.LIdentityNumber == "" || v.LIdentityNumber == null)
                {
                    wramStr = "身份证号不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }

                //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
                string name = v.LName.Replace(" ", "+");
                string identityNumber = v.LIdentityNumber.Replace(" ", "+");

                string p = "";
                p += "LName=" + name;
                p += "&LSex=" + v.LSex;
                p += "&LPhoto=" + DataHelper.IsNullReturnLine(v.LPhoto, true);
                p += "&LIdentityNumber=" + identityNumber;
                p += "&LActuator=" + v.LActuator;
                p += "&LPCType=" + v.LPCType;
                p += "&LPCNumber=" + v.LPCNumber;
                p += "&LQualifityNumber=" + v.LQualifityNumber;
                p += "&LIssuingAuthority=" + v.LIssuingAuthority;
                p += "&LIssuingDate=" + v.LIssuingDate;
                p += "&LInTime=" + v.LInTime;
                p += "&LFromCourtId=" + v.LFromCourtId;
                p += "&LRemark=" + v.LRemark;

                string md5Ciphertext = v.LMD5Ciphertext;//对方传过来的所有字段的MD5密文
                //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
                string md5P = MD5Helper._md5(p);
                if (md5Ciphertext == md5P)
                {
                    string sql = "sp_addLawyerInf";
                    name = AESHelper.AesDecrypt(name);
                    identityNumber = AESHelper.AesDecrypt(identityNumber);
                    SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@LName",SqlDbType.NVarChar){Value=name},
                    new SqlParameter("@LSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LSex)},
                    new SqlParameter("@LPhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPhoto)},
                    new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value=identityNumber},
                    new SqlParameter("@LActuator",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LActuator)},
                    new SqlParameter("@LPCType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPCType)},
                    new SqlParameter("@LPCNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPCNumber)},
                    new SqlParameter("@LQualifityNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LQualifityNumber)},
                    new SqlParameter("@LIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LIssuingAuthority)},
                    new SqlParameter("@LIssuingDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LIssuingDate)},
                    new SqlParameter("@LInTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LInTime)},
                    new SqlParameter("@LFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LFromCourtId)},
                    new SqlParameter("@LRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LRemark)}
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
                    return ConvertHelper.resultJson(0, "数据传输过程中被篡改！");
                }
            }
            else
            {
                return ConvertHelper.resultJson(101, "权限受限！");
            }
        }
        [HttpPost, Route("editInf")]
        public string EditInf(LawyerInf v)
        {
            if (v.Token == DataHelper.getToken())
            {
                string wramStr = "";
                if (v.LName == "" || v.LName == null)
                {
                    wramStr = "姓名不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }
                else if (v.LIdentityNumber == "" || v.LIdentityNumber == null)
                {
                    wramStr = "身份证号不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }
                //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
                string name = v.LName.Replace(" ", "+");
                string identityNumber = v.LIdentityNumber.Replace(" ", "+");

                string p = "";
                p += "LName=" + name;
                p += "&LSex=" + v.LSex;
                p += "&LPhoto=" + DataHelper.IsNullReturnLine(v.LPhoto, true);
                p += "&LIdentityNumber=" + identityNumber;
                p += "&LActuator=" + v.LActuator;
                p += "&LPCType=" + v.LPCType;
                p += "&LPCNumber=" + v.LPCNumber;
                p += "&LQualifityNumber=" + v.LQualifityNumber;
                p += "&LIssuingAuthority=" + v.LIssuingAuthority;
                p += "&LIssuingDate=" + v.LIssuingDate;
                p += "&LInTime=" + v.LInTime;
                p += "&LFromCourtId=" + v.LFromCourtId;
                p += "&LRemark=" + v.LRemark;

                string md5Ciphertext = v.LMD5Ciphertext;//对方传过来的所有字段的MD5密文
                //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
                string md5P = MD5Helper._md5(p);
                if (md5Ciphertext == md5P)
                {
                    //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
                    string sql = "update T_LawyerInf set LName=@LName,LSex=@LSex,LPhoto=@LPhoto,LIdentityNumber=@LIdentityNumber,LActuator=@LActuator,";
                    sql += "LPCType=@LPCType,LPCNumber=@LPCNumber,LQualifityNumber=@LQualifityNumber,LIssuingAuthority=@LIssuingAuthority,";
                    sql += "LIssuingDate=@LIssuingDate,LInTime=@LInTime,LFromCourtId=@LFromCourtId,LRemark=@LRemark";
                    sql += " where LId=@LId";
                    SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@LName",SqlDbType.NVarChar){Value=AESHelper.AesDecrypt(name)},
                    new SqlParameter("@LSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LSex)},
                    new SqlParameter("@LPhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPhoto)},
                    new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value=AESHelper.AesDecrypt(identityNumber)},
                    new SqlParameter("@LActuator",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LActuator)},
                    new SqlParameter("@LPCType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPCType)},
                    new SqlParameter("@LPCNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LPCNumber)},
                    new SqlParameter("@LQualifityNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LQualifityNumber)},
                    new SqlParameter("@LIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LIssuingAuthority)},
                    new SqlParameter("@LIssuingDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LIssuingDate)},
                    new SqlParameter("@LInTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LInTime)},
                    new SqlParameter("@LFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LFromCourtId)},
                    new SqlParameter("@LRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.LRemark)},
                    new SqlParameter("@LId",SqlDbType.Int){Value=v.LId}
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
                return ConvertHelper.resultJson(0, "出错了！");
            }
            else
            {
                return ConvertHelper.resultJson(101, "权限受限！");
            }
        }
        [HttpGet, Route("haveThisModel")]
        public string HaveThisModel(string LIdentityNumber)
        {
            string sql = "select count(*) from T_LawyerInf where LIdentityNumber=@LIdentityNumber";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value = LIdentityNumber}
            };
            object result;
            try
            {
                result = SQLHelper.ExecuteScalar(sql, CommandType.Text, pms);
                int count = Convert.ToInt32(result);
                return "{\"code\":1,\"count\":\"" + count + "\"}";
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
        [HttpGet, Route("haveThisModel4Edit")]
        public string haveThisModel4Edit(string LIdentityNumber, int LId)
        {
            string sql = "select LId from T_LawyerInf where LIdentityNumber=@LIdentityNumber";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@LIdentityNumber",SqlDbType.NVarChar){Value = LIdentityNumber}
            };
            object result;
            try
            {
                result = SQLHelper.ExecuteScalar(sql, CommandType.Text, pms);
                if (result != null)
                {
                    int id = Convert.ToInt32(result);
                    if (id == LId)
                    {
                        return "{\"code\":1,\"count\":0}";
                    }
                    else
                    {
                        return "{\"code\":1,\"count\":1}";
                    }
                }
                else
                {
                    return "{\"code\":1,\"count\":0}";
                }
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
        [HttpPost, Route("deleteInfById")]
        public string DeleteInfById(LawyerInf B)
        {
            if (B.Token + "1" == DataHelper.getToken())
            {
                //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
                string sql = "delete from T_LawyerInf where LId=@LId";
                SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@LId",SqlDbType.Int){Value=B.LId}
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
            else {
                return ConvertHelper.resultJson(101, "权限受限！");
            }
            
        }
    }
}
