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
    [RoutePrefix("api/SHFY")]
    public class SHFYController : ApiController
    {
        [HttpGet, Route("testLink")]
        public string TestLink()
        {
            string result = SQLHelper.LinkSqlDatabase();
            return ConvertHelper.resultJson(1, result);
            //return mssqlserver;
        }
        [HttpPost, Route("addOrderInfo")]
        public string AddOrderInfo(SHFY v)
        {
            string wramStr = "";
            if (string.IsNullOrEmpty(v.VisitorName))
            {
                wramStr = "访客姓名不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            if (string.IsNullOrEmpty(v.VisitorSex))
            {
                wramStr = "访客性别不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            if (string.IsNullOrEmpty(v.VisitorPhone))
            {
                wramStr = "访客电话不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            if (string.IsNullOrEmpty(v.StaffNo))
            {
                wramStr = "员工编号不能为空";
                return ConvertHelper.resultJson(0, wramStr);
            }
            string sql = "";
            SqlParameter[] pms = null;
            DateTime dt = DateTime.Now;
            int RandKey = 1000;

            bool is_ec_ok = false;
            while (!is_ec_ok)
            {
                Random ran = new Random();
                RandKey = ran.Next(1000, 9999);
                int OrderCodeIsUse = 0;

                string sqlIsExistEC = "select count(*) from XXCLOUDALL.dbo.T_SHFYOrderInfo where OrderCode=@OrderCode and OrderCodeIsUse=@OrderCodeIsUse";
                SqlParameter[] pms4EC = new SqlParameter[]{
                        new SqlParameter("@OrderCode",SqlDbType.VarChar){Value = RandKey.ToString()},
                        new SqlParameter("@OrderCodeIsUse",SqlDbType.VarChar){Value = OrderCodeIsUse.ToString()}
                    };
                object obj = SQLHelper.ExecuteScalar(sqlIsExistEC, System.Data.CommandType.Text, pms4EC);
                if (Convert.ToInt32(obj) == 0)
                { //说明此EnterCode可以使用
                    is_ec_ok = true;
                }
            }
            sql = "insert into XXCLOUDALL.dbo.T_SHFYOrderInfo(OrderNo, OrderCode, VisitorName, VisitorSex, VisitorPhone, VisitorIdNo, VisitorReason, VisitorNumber, CarNo, VisitorStartDT, VisitorEndDT, StaffNo, CreateTime)" +
                "values(@OrderNo, @OrderCode, @VisitorName, @VisitorSex, @VisitorPhone, @VisitorIdNo, @VisitorReason, @VisitorNumber, @CarNo, @VisitorStartDT, @VisitorEndDT, @StaffNo, @CreateTime)";
            
            pms = new SqlParameter[]{
                new SqlParameter("@OrderNo",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.OrderNo)},
                new SqlParameter("@OrderCode",SqlDbType.VarChar){Value= DataHelper.IsNullReturnLine(RandKey.ToString())},
                new SqlParameter("@VisitorName",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VisitorName)},
                new SqlParameter("@VisitorSex",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VisitorSex)},
                new SqlParameter("@VisitorPhone",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.VisitorPhone)},
                new SqlParameter("@VisitorIdNo",SqlDbType.VarChar){Value=DataHelper.IsNullReturnLine(v.VisitorIdNo)},
                new SqlParameter("@VisitorReason",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VisitorReason)},
                new SqlParameter("@VisitorNumber",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VisitorNumber)},
                new SqlParameter("@CarNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.CarNo)},
                new SqlParameter("@VisitorStartDT",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VisitorStartDT)},
                new SqlParameter("@VisitorEndDT",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.VisitorEndDT)},
                new SqlParameter("@StaffNo",SqlDbType.NVarChar){Value=DataHelper.IsNullReturnLine(v.StaffNo)},
                new SqlParameter("@CreateTime",SqlDbType.VarChar){Value=dt.ToString("yyyy-MM-dd hh:mm:ss")}
            };
            try
            {
                int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                if (result == 1)
                {
                    int code = 1;
                    return "{\"code\":\"" + code + "\",\"ordercode\":\"" + RandKey.ToString() + "\"}";
                    //return "{\"code\":1,\"ordercode\":" + RandKey.ToString() + "}";
                }
                else {
                    return ConvertHelper.resultJson(0, "操作数据库失败，请联系技术人员");
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
        [HttpPost, Route("checkOrderCode")]
        public string CheckOrderCode(SHFY v)
        {
            SqlParameter[] pms = null;
            string sql = "";
            int OrderCodeIsUse0 = 0;
            string sqlIsExistEC = "select count(*) from XXCLOUDALL.dbo.T_SHFYOrderInfo where OrderCode=@OrderCode and OrderCodeIsUse=@OrderCodeIsUse0";
            SqlParameter[] pms4EC = new SqlParameter[]{
                new SqlParameter("@OrderCode",SqlDbType.NVarChar){Value = v.OrderCode},
                new SqlParameter("@OrderCodeIsUse0",SqlDbType.VarChar){Value = ( OrderCodeIsUse0.ToString())}
            };
            object obj = SQLHelper.ExecuteScalar(sqlIsExistEC, System.Data.CommandType.Text, pms4EC);
            if (Convert.ToInt32(obj) == 1)
            {
                int OrderCodeIsUse = 1;
                pms = new SqlParameter[]{
                    new SqlParameter("@OrderCode",SqlDbType.VarChar){Value = (v.OrderCode)},
                    new SqlParameter("@OrderCodeIsUse",SqlDbType.VarChar){Value = ( OrderCodeIsUse.ToString())}
                };
                sql = "update XXCLOUDALL.dbo.T_SHFYOrderInfo set OrderCodeIsUse=@OrderCodeIsUse where OrderCode=@OrderCode";
                try
                {
                    int result = SQLHelper.ExecuteNonQuery(sql, System.Data.CommandType.Text, pms);
                    if (result == 1) {
                        return ConvertHelper.resultJson(1, "此预约码有效,允许进入");
                    }
                    else
                    {
                        return ConvertHelper.resultJson(0, "操作数据库失败，请联系技术人员");
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
            else {
                return ConvertHelper.resultJson(0, "此预约码无效，需人工审核");
            }
        }
        [HttpGet, Route("getList")]
        public string GetList()
        {
            string sql = "select * from XXCLOUDALL.dbo.T_SHFYOrderInfo";
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
    }
}
