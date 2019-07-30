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
    [RoutePrefix("api/blacklistInf")]
    public class BlacklistInfController : ApiController
    {
        private static readonly string privateKey = ConfigurationManager.ConnectionStrings["privatekey"].ConnectionString;
        [HttpGet, Route("getListAll")]
        public string GetListAll()
        {
            string sql = "select l.*,c.CName,Sex=case";
            sql += " when l.BSex='1' then '男'";
            sql += " when l.BSex='0' then '女'";
            sql += " else '保密'";
            sql += " end";
            sql += " from T_BlacklistInf l";
            sql += " left join T_CourtInf c";
            sql += " on l.BFromCourtId = c.CNumber";
            sql += " order by l.BId";
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
        public string GetListByMore(string BName, string BCertificateNumber, string BFromCourtId, int pageSize, int pageIndex)
        {
            string sql = "sp_getBalcklistInfByPage1";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@BName",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(BName)},
                new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(BCertificateNumber)},
                new SqlParameter("@BFromCourtId",SqlDbType.NVarChar){Value = DataHelper.IsNullReturnLine(BFromCourtId)},
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
        public string AddInf4WF(BlacklistInf v)
        {
            string wramStr = "";
            if (v.BName == "" || v.BName == null)
            {
                wramStr = "姓名不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.BAddress == "" || v.BAddress == null)
            {
                wramStr = "身份证中的住址不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }
            if (v.BCertificateNumber == "" || v.BCertificateNumber == null)
            {
                wramStr = "证件号不能为空";
                return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
            }

            //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
            string name = v.BName.Replace(" ", "+");
            string address = v.BAddress.Replace(" ", "+");
            string certificateNumber = v.BCertificateNumber.Replace(" ", "+");

            string p = "";
            p += "BName=" + name;
            p += "&BSex=" + v.BSex;
            p += "&BNation=" + v.BNation;
            p += "&BBirthDate=" + v.BBirthDate;
            p += "&BAddress=" + address;
            p += "&BIssuingAuthority=" + v.BIssuingAuthority;
            p += "&BExpiryDate=" + v.BExpiryDate;
            p += "&BCertificatePhoto=" + DataHelper.IsNullReturnLine(v.BCertificatePhoto,true);
            p += "&BLocalePhoto=" + DataHelper.IsNullReturnLine(v.BLocalePhoto,true);
            p += "&BCertificateType=" + v.BCertificateType;
            p += "&BCertificateNumber=" + certificateNumber;
            p += "&BCreateTime=" + v.BCreateTime;
            p += "&BFromCourtId=" + v.BFromCourtId;
            p += "&BLevel=" + v.BLevel;
            p += "&BRemark=" + v.BRemark;

            string md5Ciphertext = v.BMD5Ciphertext;//对方传过来的所有字段的MD5密文
            //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
            string md5P = MD5Helper._md5(p);
            if (md5Ciphertext == md5P)
            {
                string sql1 = "select count(*) from T_BlacklistInf where BCertificateNumber=@BCertificateNumber";
                SqlParameter[] pms1 = new SqlParameter[]{
                    new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value = RSAHelper.DecryptWithPrivateKey(privateKey,certificateNumber)}
                };
                try
                {
                    object c = SQLHelper.ExecuteScalar(sql1, System.Data.CommandType.Text, pms1);
                    if (Convert.ToInt32(c) > 0)
                    {
                        return ConvertHelper.resultJson(0, "数据库中已存在此证件号，请勿重复添加！");
                    }
                    else {
                        string sql = "sp_addBlacklistInf";
                        SqlParameter[] pms = new SqlParameter[]{
                            new SqlParameter("@BName",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey, name)},
                            new SqlParameter("@BSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BSex)},
                            new SqlParameter("@BNation",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BNation)},
                            new SqlParameter("@BBirthDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BBirthDate)},
                            new SqlParameter("@BAddress",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey,address)},
                            new SqlParameter("@BIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BIssuingAuthority)},
                            new SqlParameter("@BExpiryDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BExpiryDate)},
                            new SqlParameter("@BCertificatePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCertificatePhoto,true)},
                            new SqlParameter("@BLocalePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BLocalePhoto,true)},
                            new SqlParameter("@BCertificateType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCertificateType)},
                            new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value=RSAHelper.DecryptWithPrivateKey(privateKey,certificateNumber)},
                            new SqlParameter("@BCreateTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCreateTime)},
                            new SqlParameter("@BFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BFromCourtId)},
                            new SqlParameter("@BLevel",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BLevel)},
                            new SqlParameter("@BRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BRemark)}
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
        public string AddInf4Web(BlacklistInf v)
        {
            if (v.Token == DataHelper.getToken())
            {
                string wramStr = "";

                //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
                string name = v.BName.Replace(" ", "+");
                string address = v.BAddress.Replace(" ", "+");
                string certificateNumber = v.BCertificateNumber.Replace(" ", "+");

                string p = "";
                p += "BName=" + name;
                p += "&BSex=" + v.BSex;
                p += "&BNation=" + v.BNation;
                p += "&BBirthDate=" + v.BBirthDate;
                p += "&BAddress=" + address;
                p += "&BIssuingAuthority=" + v.BIssuingAuthority;
                p += "&BExpiryDate=" + v.BExpiryDate;
                p += "&BCertificatePhoto=" + v.BCertificatePhoto;
                p += "&BLocalePhoto=" + v.BLocalePhoto;
                p += "&BCertificateType=" + v.BCertificateType;
                p += "&BCertificateNumber=" + certificateNumber;
                p += "&BCreateTime=" + v.BCreateTime;
                p += "&BFromCourtId=" + v.BFromCourtId;
                p += "&BLevel=" + v.BLevel;
                p += "&BRemark=" + v.BRemark;

                string md5Ciphertext = v.BMD5Ciphertext;//对方传过来的所有字段的MD5密文
                //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
                string md5P = MD5Helper._md5(p);
                if (md5Ciphertext == md5P)
                {
                    string sql = "sp_addBlacklistInf";
                    name = AESHelper.AesDecrypt(name);
                    address = AESHelper.AesDecrypt(address);
                    certificateNumber = AESHelper.AesDecrypt(certificateNumber);
                    if (name == "" || name == null || certificateNumber == "undefined")
                    {
                        wramStr = "姓名不能为空";
                        return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                    }
                    if (address == "" || address == null || certificateNumber == "undefined")
                    {
                        wramStr = "身份证中的住址不能为空";
                        return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                    }
                    if (certificateNumber == "" || certificateNumber == null || certificateNumber == "undefined")
                    {
                        wramStr = "证件号不能为空";
                        return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                    }

                    SqlParameter[] pms = new SqlParameter[]{
                    new SqlParameter("@BName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(name)},
                    new SqlParameter("@BSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BSex)},
                    new SqlParameter("@BNation",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BNation)},
                    new SqlParameter("@BBirthDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BBirthDate)},
                    new SqlParameter("@BAddress",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(address)},
                    new SqlParameter("@BIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BIssuingAuthority)},
                    new SqlParameter("@BExpiryDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BExpiryDate)},
                    new SqlParameter("@BCertificatePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCertificatePhoto,true)},
                    new SqlParameter("@BLocalePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BLocalePhoto,true)},
                    new SqlParameter("@BCertificateType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCertificateType)},
                    new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(certificateNumber)},
                    new SqlParameter("@BCreateTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCreateTime)},
                    new SqlParameter("@BFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BFromCourtId)},
                    new SqlParameter("@BLevel",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BLevel)},
                    new SqlParameter("@BRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BRemark)}
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
            else
            {
                return ConvertHelper.resultJson(101, "权限受限！");
            }
        }
        [HttpGet, Route("haveThisModel")]
        public string HaveThisModel(string BCertificateNumber)
        {
            string sql = "select count(*) from T_BlacklistInf where BCertificateNumber=@BCertificateNumber";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value = BCertificateNumber}
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
        public string haveThisModel4Edit(string BCertificateNumber, int BId)
        {
            string sql = "select BId from T_BlacklistInf where BCertificateNumber=@BCertificateNumber";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value = BCertificateNumber}
            };
            object result;
            try
            {
                result = SQLHelper.ExecuteScalar(sql, CommandType.Text, pms);
                if (result != null)
                {
                    int id = Convert.ToInt32(result);
                    if (id == BId)
                    {
                        return "{\"code\":1,\"count\":0}";
                    }
                    else {
                        return "{\"code\":1,\"count\":1}";
                    }
                }
                else {
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
        [HttpPost, Route("editInf")]
        public string EditInf(BlacklistInf v)
        {
            if (v.Token == DataHelper.getToken())
            {
                string wramStr = "";
                if (v.BName == "" || v.BName == null)
                {
                    wramStr = "姓名不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }
                else if (v.BAddress == "" || v.BAddress == null)
                {
                    wramStr = "身份证中的住址不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }
                else if (v.BCertificateNumber == "" || v.BCertificateNumber == null)
                {
                    wramStr = "证件号不能为空";
                    return "{\"code\":0,\"msg\":\"" + wramStr + "\"}";
                }
                else
                {
                    //数据在传输过程中，密文中的“+”号会被替换合成“ ”空格号，把它还原回来
                    string name = v.BName.Replace(" ", "+");
                    string address = v.BAddress.Replace(" ", "+");
                    string certificateNumber = v.BCertificateNumber.Replace(" ", "+");

                    string p = "";
                    p += "BName=" + name;
                    p += "&BSex=" + v.BSex;
                    p += "&BNation=" + v.BNation;
                    p += "&BBirthDate=" + v.BBirthDate;
                    p += "&BAddress=" + address;
                    p += "&BIssuingAuthority=" + v.BIssuingAuthority;
                    p += "&BExpiryDate=" + v.BExpiryDate;
                    p += "&BCertificatePhoto=" + v.BCertificatePhoto;
                    p += "&BLocalePhoto=" + v.BLocalePhoto;
                    p += "&BCertificateType=" + v.BCertificateType;
                    p += "&BCertificateNumber=" + certificateNumber;
                    p += "&BCreateTime=" + v.BCreateTime;
                    p += "&BFromCourtId=" + v.BFromCourtId;
                    p += "&BLevel=" + v.BLevel;
                    p += "&BRemark=" + v.BRemark;

                    string md5Ciphertext = v.BMD5Ciphertext;//对方传过来的所有字段的MD5密文
                    //把传过来的信息再次MD5加密，和所有字段的MD5密文进行比对，保证数据在传输过程中没被修改才允许添加到数据库
                    string md5P = MD5Helper._md5(p);
                    if (md5Ciphertext == md5P)
                    {
                        //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
                        string sql = "update T_BlacklistInf set BName=@BName,BSex=@BSex,BNation=@BNation,BBirthDate=@BBirthDate,BAddress=@BAddress,";
                        sql += "BIssuingAuthority=@BIssuingAuthority,BExpiryDate=@BExpiryDate,BCertificatePhoto=@BCertificatePhoto,BLocalePhoto=@BLocalePhoto,BCertificateType=@BCertificateType,BCertificateNumber=@BCertificateNumber,BCreateTime=@BCreateTime,BFromCourtId=@BFromCourtId,";
                        sql += "BLevel=@BLevel,BRemark=@BRemark";
                        sql += " where BId=@BId";
                        SqlParameter[] pms = new SqlParameter[]{
                            new SqlParameter("@BName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(AESHelper.AesDecrypt(name))},
                            new SqlParameter("@BSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BSex)},
                            new SqlParameter("@BNation",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BNation)},
                            new SqlParameter("@BBirthDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BBirthDate)},
                            new SqlParameter("@BAddress",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(AESHelper.AesDecrypt(address))},
                            new SqlParameter("@BIssuingAuthority",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BIssuingAuthority)},
                            new SqlParameter("@BExpiryDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BExpiryDate)},
                            new SqlParameter("@BCertificatePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCertificatePhoto,true)},
                            new SqlParameter("@BLocalePhoto",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BLocalePhoto,true)},
                            new SqlParameter("@BCertificateType",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCertificateType)},
                            new SqlParameter("@BCertificateNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(AESHelper.AesDecrypt(certificateNumber))},
                            new SqlParameter("@BCreateTime",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BCreateTime)},
                            new SqlParameter("@BFromCourtId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BFromCourtId)},
                            new SqlParameter("@BLevel",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BLevel)},
                            new SqlParameter("@BRemark",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.BRemark)},
                            new SqlParameter("@BId",SqlDbType.Int){Value=v.BId}
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
                        return ConvertHelper.resultJson(0, "数据在传输过程中被篡改！");
                    }
                }
            }
            else
            {
                return ConvertHelper.resultJson(101, "权限受限！");
            }
            
        }
        [HttpPost, Route("deleteInfById")]
        public string DeleteInfById(BlacklistInf B)
        {
            if (B.Token == DataHelper.getToken())
            {
                //string sql = "insert into T_VisitorAccessInf(VName, VSex, VNation, VBirthDate, VAddress, VIssuingAuthority, VExpiryDate, VCertificatePhoto, VLocalePhoto, VCertificateType, VCertificateNumber, VType, VFromCourtId, VInTime, VOutTime, VInPost, VOutPost, VInDoorkeeper, VOutDoorkeeper, VVisitingReason, VIntervieweeDept, VInterviewee, VOffice, VOfficePhone, VExtensionPhone, VMobilePhone, VRemark) values(@VName, @VSex, @VNation, @VBirthDate, @VAddress, @VIssuingAuthority, @VExpiryDate, @VCertificatePhoto, @VLocalePhoto, @VCertificateType, @VCertificateNumber, @VType, @VFromCourtId, @VInTime, @VOutTime, @VInPost, @VOutPost, @VInDoorkeeper, @VOutDoorkeeper, @VVisitingReason, @VIntervieweeDept, @VInterviewee, @VOffice, @VOfficePhone, @VExtensionPhone, @VMobilePhone, @VRemark)";
                string sql = "delete from T_BlacklistInf where BId=@BId";
                SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@BId",SqlDbType.Int){Value=B.BId}
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
