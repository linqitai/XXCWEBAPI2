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
    [RoutePrefix("api/userInf")]
    public class UserInfController : ApiController
    {
        private static readonly string privateKey = ConfigurationManager.ConnectionStrings["privatekey"].ConnectionString;
        [HttpGet, Route("login")]
        public string GetModel4Login(string UserName, string UPassword)
        {
            string sql = "sp_getModel4Login";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@UserName",SqlDbType.NVarChar){Value = AESHelper.AesDecrypt(UserName)},
                new SqlParameter("@UPassword",SqlDbType.NVarChar){Value = AESHelper.AesDecrypt(UPassword)},
                new SqlParameter("@count",SqlDbType.Int){Direction=ParameterDirection.Output}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.StoredProcedure, pms);
                string token = DataHelper.getToken();
                return "{\"code\":1,\"count\":" + pms[2].Value.ToString() + ",\"Token\":\"" + token + "\",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
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
        [HttpPost, Route("editInf")]
        public string EditInf(UserInf v)
        {
            if (v.Token == DataHelper.getToken())
            {
                string wramStr = "";
                if (string.IsNullOrEmpty(v.OldPassword))
                {
                    wramStr = "旧密码不能为空";
                    return ConvertHelper.resultJson(0, wramStr);
                }
                else if (string.IsNullOrEmpty(v.NewPassword))
                {
                    wramStr = "新密码不能为空";
                    return ConvertHelper.resultJson(0, wramStr);
                }
                else
                {
                    string p = "";
                    p += "UserName=" + v.UserName;
                    p += "OldPassword=" + v.OldPassword;
                    p += "NewPassword=" + v.NewPassword;

                    string md5Ciphertext = v.MD5Ciphertext;//对方传过来的所有字段的MD5密文
                    //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
                    string md5P = MD5Helper._md5(p);
                    if (md5Ciphertext == md5P)
                    {
                        string oldPwd = AESHelper.AesDecrypt(v.OldPassword);
                        string pwd = AESHelper.AesDecrypt(v.NewPassword);
                        string username = AESHelper.AesDecrypt(v.UserName);

                        string sql1 = "select count(*) from T_UserInf where UserName=@UserName and UPassword=@UPassword";
                        SqlParameter[] pms1 = new SqlParameter[]{
                            new SqlParameter("@UPassword",SqlDbType.NVarChar){Value = (oldPwd)},
                            new SqlParameter("@UserName",SqlDbType.NVarChar){Value = (username)}
                        };
                        try
                        {
                            object c = SQLHelper.ExecuteScalar(sql1, System.Data.CommandType.Text, pms1);
                            if (Convert.ToInt32(c) > 0)
                            {
                                string sql2 = "update T_UserInf set UPassword=@UPassword";
                                sql2 += " where UserName=@UserName";
                                SqlParameter[] pms2 = new SqlParameter[]{
                                    new SqlParameter("@UPassword",SqlDbType.NVarChar){Value=pwd},
                                    new SqlParameter("@UserName",SqlDbType.NVarChar){Value=username}
                                };
                                try
                                {
                                    int result = SQLHelper.ExecuteNonQuery(sql2, System.Data.CommandType.Text, pms2);
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
                                return ConvertHelper.resultJson(0, "旧密码不正确！");
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
                        //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
                        
                    }
                    else
                    {
                        return ConvertHelper.resultJson(0, "数据在传输过程中被篡改！");
                    }
                }
            }
            else
            {
                return ConvertHelper.resultJson(0, "权限受限！");
            }

        }

        [HttpGet, Route("getListByMore")]
        public string GetListByMore(string UserName, int pageSize, int pageIndex)
        {
            string sql = "sp_getUserInfByPage";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@UserName",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(UserName)},
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
        
        [HttpPost, Route("addInf4Web")]
        public string AddInf4Web(LawyerInf v)
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
            p += "&LIdentityNumber=" + v.LIdentityNumber;
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
                return ConvertHelper.resultJson(0, "数据传输过程中被篡改");
            }
        }
        [HttpPost, Route("deleteInfById")]
        public string DeleteInfById(LawyerInf B)
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
    }
}
