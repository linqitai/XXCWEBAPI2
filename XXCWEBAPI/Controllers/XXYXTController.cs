using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
    [RoutePrefix("api/XXYXT")]
    public class XXYXTController : ApiController
    {
        public string AppId = ConfigurationManager.AppSettings["AppId"];
        public string AppSecret = ConfigurationManager.AppSettings["AppSecret"];
        [HttpGet, Route("testLink")]
        public string TestLink()
        {
            string result = SQLHelper4XXYXT.LinkSqlDatabase();
            return ConvertHelper.resultJson(1, result);
            //return mssqlserver;
        }
        [HttpGet, Route("testLink4XXYXT")]
        public string TestLink4XXYXT()
        {
            string result = SQLHelper4XXYXT.LinkSqlDatabase();
            return ConvertHelper.resultJson(1, result);
            //return mssqlserver;
        }
        static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { 
                new DataColumn("Id",typeof(int)), 
                new DataColumn("Form_id",typeof(string)), 
                new DataColumn("CreateDate",typeof(string)),
                new DataColumn("OpenId",typeof(string))
            });
            return dt;
        }
        [HttpPost, Route("deletePastTimeForm_id")]
        public string DeletePastTimeForm_id(XXYXT v)
        {
            string sql_countPastFormId = "select count(*) from XXCLOUD.dbo.T_WXFormId where CreateDate<@SubDate";
            SqlParameter[] pms_countPastFormId = new SqlParameter[]{
                new SqlParameter("@SubDate",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.SubDate)}
            };
            try
            {
                object result_count1 = SQLHelper4XXYXT.ExecuteScalar(sql_countPastFormId, System.Data.CommandType.Text, pms_countPastFormId);
                if (Convert.ToInt32(result_count1) > 0)
                {
                    //删除已经过期的Form_id
                    string sql = "delete from XXCLOUD.dbo.T_WXFormId where CreateDate<@SubDate";
                    SqlParameter[] pms = new SqlParameter[]{
                        new SqlParameter("@SubDate",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.SubDate)}
                    };
                    try
                    {
                        object result_count2 = SQLHelper4XXYXT.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                        if (Convert.ToInt32(result_count2) > 0)
                        {
                            return ConvertHelper.resultJson(1, "删除了" + Convert.ToInt32(result_count2) + "条过期Form_id");
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
                }else{
                    return ConvertHelper.resultJson(0, "没有待删除的数据");
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
            return ConvertHelper.resultJson(0, "没有待删除的数据");
        }
        [HttpPost, Route("saveForm_id")]
        public string SaveForm_id(XXYXT v)
        {
            DateTime dataTime = DateTime.Now;
            string sql_countFormId = "select count(*) from XXCLOUD.dbo.T_WXFormId where CreateDate>@SubDate";
            string sql_countStudent = "select count(*) from XXCLOUD.dbo.T_ClassAndStudentInf";
            string sql_countStaff = "select count(*) from XXCLOUD.dbo.T_ClassCategroyInf";
            SqlParameter[] pms_countFormId = new SqlParameter[]{
                new SqlParameter("@SubDate",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.SubDate)}
            };
            try
            {
                object countFormId = SQLHelper4XXYXT.ExecuteScalar(sql_countFormId, System.Data.CommandType.Text, pms_countFormId);
                object countStudent = SQLHelper4XXYXT.ExecuteScalar(sql_countStudent, System.Data.CommandType.Text, null);
                object countStaff = SQLHelper4XXYXT.ExecuteScalar(sql_countStaff, System.Data.CommandType.Text, null);
                int count_formId = Convert.ToInt32(countFormId);
                int count_need_formId = Convert.ToInt32(countStudent) + Convert.ToInt32(countStaff);
                if (count_formId >= (count_need_formId*2))
                {
                    return ConvertHelper.resultJson(0, "数据库中form_id足够用，无需添加");
                }
                else if (count_formId < (count_need_formId * 2))
                {
                    Stopwatch sw = new Stopwatch();
                    DataTable dt = GetTableSchema();

                    string [] formIds = v.Form_ids.Split(',');
                    using (SqlConnection conn = new SqlConnection(SQLHelper4XXYXT.GetConstr()))
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                        bulkCopy.DestinationTableName = "T_WXFormId";
                        bulkCopy.BatchSize = dt.Rows.Count;
                        conn.Open();
                        sw.Start();

                        for (int i = 0; i < formIds.Length; i++)
                        {
                            string form_id = formIds[i];

                            DataRow dr = dt.NewRow();
                            dr[0] = 1;
                            dr[1] = form_id;
                            dr[2] = dataTime.ToString("yyyy-MM-dd");
                            dr[3] = v.OpenId;
                            dt.Rows.Add(dr);
                        }
                        if (dt != null && dt.Rows.Count != 0)
                        {
                            bulkCopy.WriteToServer(dt);
                            sw.Stop();
                            return ConvertHelper.resultJson(1, "数据插入成功");
                        }
                        //Console.WriteLine(string.Format("插入{0}条记录共花费{1}毫秒，{2}分钟", totalRow, sw.ElapsedMilliseconds, GetMinute(sw.ElapsedMilliseconds)));
                    }
                }
                return ConvertHelper.resultJson(0, "无效动作");
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
        [HttpPost, Route("getForm_id")]
        public string GetForm_id(XXYXT v)
        {
            //需要修改
            string sql = "select * from XXCLOUD.dbo.T_WXFormId";
            //SqlParameter[] pms = new SqlParameter[]{
            //    new SqlParameter("@ClassName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.ClassName)},
            //    new SqlParameter("@CreateDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CreateDate)}
            //};
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, System.Data.CommandType.Text, null);
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
        [HttpPost, Route("getLeaveListByDateAndClassName")]
        public string GetLeaveListByDateAndClassName(XXYXT v)
        {
            //需要修改
            string sql = "select b.SName,b.SSex,b.PUPName,b.PUPSex,b.PUPPhone,a.StartTime,a.EndTime,a.CreateTime,a.CreateDate,a.Reason,a.ReplyText from XXCLOUDALL.dbo.Table_LeaveInf a LEFT JOIN JYKJXTZT.dbo.JY_PUPAndStudentInf b on a.StuId = b.Id where b.ClassName=@ClassName and a.CreateDate=@CreateDate";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@ClassName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.ClassName)},
                new SqlParameter("@CreateDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CreateDate)}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, System.Data.CommandType.Text, pms);
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
        [HttpPost, Route("addWXUserInfo")]
        public string AddWXUserInfo(WXUserInfo v)
        {
            if (string.IsNullOrEmpty(v.Phone))
            {
                return ConvertHelper.resultJson(0, "请先填写常用手机号");
            }
            string sql = "select count(*) from XXCLOUD.dbo.T_WXUserInfo where OpenId = @OpenId";
            object obj;
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)}
            };
            try
            {
                obj = SQLHelper4XXYXT.ExecuteScalar(sql, System.Data.CommandType.Text, pms);
                if (Convert.ToInt32(obj) == 1)
                {
                    return ConvertHelper.resultJson(1, "数据库中已经存在此数据");
                }
                else if (Convert.ToInt32(obj) == 0)
                {
                    //数据库中不存在此数据的时候
                    //1.先去判断该用户是教师还是家长
                    if (v.Role == "教师")
                    {
                        //if教师 根据手机号搜索是否有此教师，有就将此数据保存到数据库XXCLOUDALL.dbo.T_WXUserInfo表中
                        string sql_teacher = "select count(*) from XXCLOUD.dbo.T_ClassCategroyInf where SMPhone1=@SMPhone or SMPhone2=@SMPhone";
                        SqlParameter[] pms_teacher = new SqlParameter[]{
                            new SqlParameter("@SMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.Phone)}
                        };
                        try
                        {
                            object obj_teacher = SQLHelper4XXYXT.ExecuteScalar(sql_teacher, System.Data.CommandType.Text, pms_teacher);
                            if (Convert.ToInt32(obj_teacher) == 1)
                            {
                                string sql2 = "insert into XXCLOUD.dbo.T_WXUserInfo(NickName, Gender, City, Province, AvatarUrl, OpenId, CreateTime, Phone, Role)" +
                "values(@NickName, @Gender, @City, @Province, @AvatarUrl, @OpenId, @CreateTime, @Phone, @Role)";
                                DateTime dt = DateTime.Now;
                                SqlParameter[] pms2 = new SqlParameter[]{
                                    new SqlParameter("@NickName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.NickName)},
                                    new SqlParameter("@Gender",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Gender)},
                                    new SqlParameter("@City",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.City)},
                                    new SqlParameter("@Province",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Province)},
                                    new SqlParameter("@AvatarUrl",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.AvatarUrl)},
                                    new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)},
                                    new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                                    new SqlParameter("@Phone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Phone)},
                                    new SqlParameter("@Role",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Role)}
                                };
                                try
                                {
                                    int result = SQLHelper4XXYXT.ExecuteNonQuery(sql2, System.Data.CommandType.Text, pms2);
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
                            else if (Convert.ToInt32(obj_teacher) == 0)
                            {
                                return ConvertHelper.resultJson(0, "教师数据中不存在该教师手机号，请先在后台录入此手机号");
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
                    else if (v.Role == "家长")
                    {
                        //if家长 根据手机号搜索是否有此家长，有就将此数据保存到数据库XXCLOUDALL.dbo.T_WXUserInfo表中
                        string sql_parent = "select count(*) from XXCLOUD.dbo.T_SurrogateInf where SurrogateMPhone=@SurrogateMPhone";
                        SqlParameter[] pms_parent = new SqlParameter[]{
                            new SqlParameter("@SurrogateMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.Phone)}
                        };
                        try
                        {
                            object obj_parent = SQLHelper4XXYXT.ExecuteScalar(sql_parent, System.Data.CommandType.Text, pms_parent);
                            if (Convert.ToInt32(obj_parent) > 0)
                            {
                                string sql2 = "insert into XXCLOUD.dbo.T_WXUserInfo(NickName, Gender, City, Province, AvatarUrl, OpenId, CreateTime, Phone, Role)" +
                "values(@NickName, @Gender, @City, @Province, @AvatarUrl, @OpenId, @CreateTime, @Phone, @Role)";
                                DateTime dt = DateTime.Now;
                                SqlParameter[] pms2 = new SqlParameter[]{
                                    new SqlParameter("@NickName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.NickName)},
                                    new SqlParameter("@Gender",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Gender)},
                                    new SqlParameter("@City",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.City)},
                                    new SqlParameter("@Province",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Province)},
                                    new SqlParameter("@AvatarUrl",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.AvatarUrl)},
                                    new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)},
                                    new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                                    new SqlParameter("@Phone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Phone)},
                                    new SqlParameter("@Role",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Role)}
                                };
                                try
                                {
                                    int result = SQLHelper4XXYXT.ExecuteNonQuery(sql2, System.Data.CommandType.Text, pms2);
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
                            else if (Convert.ToInt32(obj_parent) == 0)
                            {
                                return ConvertHelper.resultJson(0, "学生数据中不存在该家长手机号，请先联系院方给孩子绑定此手机号");
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
                    else if (v.Role == "管理员")
                    {
                        //if教师 根据手机号搜索是否有此教师，有就将此数据保存到数据库XXCLOUDALL.dbo.T_WXUserInfo表中
                        string sql_admin = "select count(*) from XXCLOUD.dbo.T_OperatorInf where OPhone=@Phone";
                        SqlParameter[] pms_admin = new SqlParameter[]{
                            new SqlParameter("@Phone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.Phone)}
                        };
                        try
                        {
                            object obj_teacher = SQLHelper4XXYXT.ExecuteScalar(sql_admin, System.Data.CommandType.Text, pms_admin);
                            if (Convert.ToInt32(obj_teacher) == 1)
                            {
                                //string sql_WXUserInfo = "select count(*) from XXCLOUD.dbo.T_WXUserInfo where OpenId=@OpenId";
                                //object result_WXUserInfo = SQLHelper4XXYXT.ExecuteScalar(sql_WXUserInfo, System.Data.CommandType.Text, pms);
                                //if (Convert.ToInt32(result_WXUserInfo) > 0)
                                //{
                                //    return ConvertHelper.resultJson(0, "教师数据中不存在该教师手机号，请先在后台录入此手机号");
                                //}

                                string sql2 = "insert into XXCLOUD.dbo.T_WXUserInfo(NickName, Gender, City, Province, AvatarUrl, OpenId, CreateTime, Phone, Role)" +
                "values(@NickName, @Gender, @City, @Province, @AvatarUrl, @OpenId, @CreateTime, @Phone, @Role)";
                                DateTime dt = DateTime.Now;
                                SqlParameter[] pms2 = new SqlParameter[]{
                                    new SqlParameter("@NickName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.NickName)},
                                    new SqlParameter("@Gender",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Gender)},
                                    new SqlParameter("@City",SqlDbType.NVarChar){Value= DataHelper.IsNullReturnLine(v.City)},
                                    new SqlParameter("@Province",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Province)},
                                    new SqlParameter("@AvatarUrl",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.AvatarUrl)},
                                    new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)},
                                    new SqlParameter("@CreateTime",SqlDbType.NVarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")},
                                    new SqlParameter("@Phone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Phone)},
                                    new SqlParameter("@Role",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Role)}
                                };
                                try
                                {
                                    int result = SQLHelper4XXYXT.ExecuteNonQuery(sql2, System.Data.CommandType.Text, pms2);
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
                            else if (Convert.ToInt32(obj_teacher) == 0)
                            {
                                return ConvertHelper.resultJson(0, "教师数据中不存在该教师手机号，请先在后台录入此手机号");
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
                    
                    //return ConvertHelper.resultJson(0, "数据库中不存在此数据");
                    
                }
                return ConvertHelper.resultJson(0, "系统出错了");
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
        [HttpPost, Route("updateWXUserInfo")]
        public string UpdateWXUserInfo(WXUserInfo v)
        {
            string sql = "select count(*) from XXCLOUD.dbo.T_WXUserInfo where OpenId = @OpenId";
            object obj;
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)}
            };
            try
            {
                obj = SQLHelper4XXYXT.ExecuteScalar(sql, System.Data.CommandType.Text, pms);
                if (Convert.ToInt32(obj) == 0)
                {
                    return ConvertHelper.resultJson(1, "请先联系管理员在后台录入该身份");
                }
                else if (Convert.ToInt32(obj) == 1)
                {
                    string sql_update = "update XXCLOUD.dbo.T_WXUserInfo set Role=@Role where OpenId = @OpenId";
                    SqlParameter[] pms_update = new SqlParameter[]{
                        new SqlParameter("@Role",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.Role)},
                        new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)}
                    };
                    int update_result = SQLHelper4XXYXT.ExecuteNonQuery(sql_update, System.Data.CommandType.Text, pms_update);
                    return ConvertHelper.IntToJson(update_result);
                }
                return ConvertHelper.IntToJson(0);
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
        [HttpPost, Route("haveUserInfo")]
        public string HaveUserInfo(WXUserInfo v)
        {
            string sql = "select * from XXCLOUD.dbo.T_WXUserInfo where OpenId = @OpenId";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@OpenId",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.OpenId)}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper4XXYXT.ExecuteDataTable(sql, System.Data.CommandType.Text, pms);
                if (dt.Rows.Count > 0)
                {
                    if (v.Role == "管理员") {
                        DataTable dt_admin;
                        string sql_admin = "select * from XXCLOUD.dbo.T_OperatorInf where OPhone=@Phone";
                        SqlParameter[] pms_admin = new SqlParameter[]{
                            new SqlParameter("@Phone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                        };
                        dt_admin = SQLHelper4XXYXT.ExecuteDataTable(sql_admin, System.Data.CommandType.Text, pms_admin);
                        return "{\"code\":1,\"msg\":\"" + v.Role + "\",\"count\":" + dt_admin.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_admin) + "}"; 
                    }
                    if (v.Role == "家长")
                    {
                        DataTable dt_parent;
                        string sql_parent = "select * from XXCLOUD.dbo.T_SurrogateInf where SurrogateMPhone=@SurrogateMPhone";
                        SqlParameter[] pms_parent = new SqlParameter[]{
                            new SqlParameter("@SurrogateMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                        };
                        dt_parent = SQLHelper4XXYXT.ExecuteDataTable(sql_parent, System.Data.CommandType.Text, pms_parent);
                        return "{\"code\":1,\"msg\":\"" + "家长" + "\",\"count\":" + dt_parent.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_parent) + "}";
                    }
                    if (v.Role == "教师")
                    {
                        DataTable dt_teacher;
                        string sql_teacher = "select * from XXCLOUD.dbo.T_ClassCategroyInf where SMPhone1=@SMPhone";
                        SqlParameter[] pms_teacher1 = new SqlParameter[]{
                            new SqlParameter("@SMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                        };
                        dt_teacher = SQLHelper4XXYXT.ExecuteDataTable(sql_teacher, System.Data.CommandType.Text, pms_teacher1);
                        if (dt_teacher.Rows.Count > 0)
                        {
                            return "{\"code\":1,\"msg\":\"" + "教师" + "\",\"tearch\":\"" + 1 + "\",\"count\":" + dt_teacher.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_teacher) + "}";
                        }
                        else {
                            string sql_teacher2 = "select * from XXCLOUD.dbo.T_ClassCategroyInf where SMPhone2=@SMPhone";
                            SqlParameter[] pms_teacher2 = new SqlParameter[]{
                                new SqlParameter("@SMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                            };
                            dt_teacher = SQLHelper4XXYXT.ExecuteDataTable(sql_teacher2, System.Data.CommandType.Text, pms_teacher2);
                            if (dt_teacher.Rows.Count > 0)
                            {
                                return "{\"code\":1,\"msg\":\"" + "教师" + "\",\"tearch\":\"" + 2 + "\",\"count\":" + dt_teacher.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_teacher) + "}";
                            }
                        }
                        //return "{\"code\":1,\"msg\":\"" + v.Role + "\",\"count\":" + dt_teacher.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_teacher) + "}";
                    }
                    if (v.Role == "" || v.Role == null || v.Role == "null")
                    {
                        if (Convert.ToString(dt.Rows[0]["Role"]) == "家长")
                        {
                            DataTable dt_parent;
                            string sql_parent = "select * from XXCLOUD.dbo.T_SurrogateInf where SurrogateMPhone=@SurrogateMPhone";
                            SqlParameter[] pms_parent = new SqlParameter[]{
                                new SqlParameter("@SurrogateMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                            };
                            dt_parent = SQLHelper4XXYXT.ExecuteDataTable(sql_parent, System.Data.CommandType.Text, pms_parent);
                            return "{\"code\":1,\"msg\":\"" + "家长" + "\",\"count\":" + dt_parent.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_parent) + "}";
                        }
                        if (Convert.ToString(dt.Rows[0]["Role"]) == "教师")
                        {
                            DataTable dt_teacher;
                            string sql_teacher = "select * from XXCLOUD.dbo.T_ClassCategroyInf where SMPhone1=@SMPhone";
                            SqlParameter[] pms_teacher1 = new SqlParameter[]{
                                new SqlParameter("@SMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                            };
                            dt_teacher = SQLHelper4XXYXT.ExecuteDataTable(sql_teacher, System.Data.CommandType.Text, pms_teacher1);
                            if (dt_teacher.Rows.Count > 0)
                            {
                                return "{\"code\":1,\"msg\":\"" + "教师" + "\",\"tearch\":\"" + 1 + "\",\"count\":" + dt_teacher.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_teacher) + "}";
                            }
                            else
                            {
                                string sql_teacher2 = "select * from XXCLOUD.dbo.T_ClassCategroyInf where SMPhone2=@SMPhone";
                                SqlParameter[] pms_teacher2 = new SqlParameter[]{
                                    new SqlParameter("@SMPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                                };
                                dt_teacher = SQLHelper4XXYXT.ExecuteDataTable(sql_teacher2, System.Data.CommandType.Text, pms_teacher2);
                                if (dt_teacher.Rows.Count > 0)
                                {
                                    return "{\"code\":1,\"msg\":\"" + "教师" + "\",\"tearch\":\"" + 2 + "\",\"count\":" + dt_teacher.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_teacher) + "}";
                                }
                            }
                        }
                        if (Convert.ToString(dt.Rows[0]["Role"]) == "管理员")
                        {
                            DataTable dt_teacher;
                            string sql_teacher = "select * from XXCLOUD.dbo.T_OperatorInf where OPhone=@Phone";
                            SqlParameter[] pms_teacher = new SqlParameter[]{
                                new SqlParameter("@Phone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(Convert.ToString(dt.Rows[0]["Phone"]))}
                            };
                            dt_teacher = SQLHelper4XXYXT.ExecuteDataTable(sql_teacher, System.Data.CommandType.Text, pms_teacher);
                            return "{\"code\":1,\"msg\":\"" + "管理员" + "\",\"count\":" + dt_teacher.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt_teacher) + "}";
                        }
                    }
                    //return "{\"code\":1,\"count\":" + dt.Rows.Count + ",\"data\":" + ConvertHelper.DataTableToJson(dt) + "}";
                    //return ConvertHelper.resultJson(1, "数据库中已经存在此数据");
                }
                else
                {
                    return ConvertHelper.resultJson(0, "请先联系管理员绑定该身份");
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
            return ConvertHelper.resultJson(0, "系统出错");
        }
        [HttpPost, Route("getStudentListByClassName")]
        public string GetStudentListByClassName(XXYXT v)
        {
            string sql = "select * from XXCLOUD.dbo.T_ClassAndStudentInf where SDDetailName=@ClassName";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@ClassName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.ClassName)}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper4XXYXT.ExecuteDataTable(sql, System.Data.CommandType.Text, pms);
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
        static DateTime dateNow = DateTime.Now;
        static string year = dateNow.ToString("yyyy");
        string database = "YEAR" + year;
        [HttpPost, Route("getAccessListByDateAndClassName")]
        public string GetAccessListByDateAndClassName(XXYXT v)
        {
            string sql = "select * from " + database + ".dbo.T_MJRecordAccessInf where SDDetailName=@ClassName and RecordDate>=@CreateDate and ReadHeadNote=@ReadHeadNote";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@ClassName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.ClassName)},
                new SqlParameter("@CreateDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CreateDate)},
                new SqlParameter("@ReadHeadNote",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.ReadHeadNote)}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper4XXYXT.ExecuteDataTable(sql, System.Data.CommandType.Text, pms);
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

        [HttpPost, Route("getChildListByDateAndSActualNo")]
        public string GetChildListByDateAndSActualNo(XXYXT v)
        {
            string sql = "select * from " + database + ".dbo.T_MJRecordAccessInf where RecordDate=@CreateDate and SActualNo=@SActualNo";//RecordDate是新增字段
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@CreateDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CreateDate)},
                new SqlParameter("@SActualNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.SActualNo)}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper4XXYXT.ExecuteDataTable(sql, System.Data.CommandType.Text, pms);
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

        [HttpPost, Route("getMyChildBySurrogateMPhone")]
        public string GetMyChildBySurrogateMPhone(XXYXT v)
        {
            string sql = "select * from  XXCLOUD.dbo.T_ClassAndStudentInf a,XXCLOUD.dbo.T_SurrogateInf b where a.SActualNo=b.SActualNo and b.SurrogateMPhone=@SurrogateMPhone";
            SqlParameter[] pms = new SqlParameter[]{
                new SqlParameter("@SurrogateMPhone",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.SurrogateMPhone)}
            };
            DataTable dt;
            try
            {
                dt = SQLHelper4XXYXT.ExecuteDataTable(sql, System.Data.CommandType.Text, pms);
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

        [HttpPost, Route("getClassList")]
        public string GetClassList()
        {
            string sql = "select * from XXCLOUD.dbo.T_ClassCategroyInf";
            //SqlParameter[] pms = new SqlParameter[]{
            //    new SqlParameter("@CreateDate",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CreateDate)},
            //    new SqlParameter("@SActualNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.SActualNo)}
            //};
            DataTable dt;
            try
            {
                dt = SQLHelper4XXYXT.ExecuteDataTable(sql, System.Data.CommandType.Text, null);
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

        [HttpPost, Route("getOpenIdByCode")]
        public string GetOpenIdByCode(VisitorsSearchModel v)
        {
            if (v.js_code != "" && v.js_code != null)
            {
                //string url = "https://api.weixin.qq.com/sns/jscode2session?appid=" + AppId + "&secret=" + AppSecret + "&js_code=" + v.Code + "&grant_type=authorization_code";
                string url = "https://api.weixin.qq.com/sns/jscode2session";
                string p = "";
                p = "appid=" + v.appid + "&secret=" + v.secret + "&js_code=" + v.js_code + "&grant_type=" + v.grant_type;
                string result = HttpHelper.HttpPost(url, p);
                return ConvertHelper.resultJson(0, result);
            }
            return ConvertHelper.resultJson(0, "系统出错了");
        }
        [HttpGet, Route("getStaffInfoByName")]
        public string GetStaffInfoByName(string SName)
        {
            string sql = "";
            SqlParameter[] pms = null;
            pms = new SqlParameter[]{
                new SqlParameter("@SName",SqlDbType.NVarChar){Value = (SName)}
            };

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            sql = "select * from XXCLOUD.dbo.T_ClassCategroyInf where SName = @SName";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
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
        [HttpGet, Route("getStaffInfoByPhone")]
        public string GetStaffInfoByPhone(string SMPhone)
        {
            string sql = "";
            SqlParameter[] pms = null;
            pms = new SqlParameter[]{
                new SqlParameter("@SMPhone",SqlDbType.NVarChar){Value = (SMPhone)}
            };

            //new SqlParameter("@Phone",SqlDbType.NVarChar){Value = (v.Phone)},
            //string sql_str = 
            sql = "select * from XXCLOUD.dbo.T_ClassCategroyInf where SMPhone = @SMPhone";
            DataTable dt;
            try
            {
                dt = SQLHelper.ExecuteDataTable(sql, CommandType.Text, pms);
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
    }
}
