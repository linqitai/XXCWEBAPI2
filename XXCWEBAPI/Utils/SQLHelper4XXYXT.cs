using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace XXCWEBAPI.Utils
{
    public static class SQLHelper4XXYXT
    {
        //定义一个链接字符串
        private static readonly string conStr = ConfigurationManager.ConnectionStrings["mssqlserver4"].ConnectionString;
        #region 链接数据库
        public static string LinkSqlDatabase()
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                return "链接数据库成功！";
            }
        }
        public static string GetConstr()
        {
            return conStr;
        }
        #endregion
        //1.执行增(insert)、删(delete)、改(update)的方法
        //ExecuteNonQuery
        public static int ExecuteNonQuery(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = cmdType;
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        //2.执行查询,返回单个值的方法
        //ExecuteScalar()
        public static object ExecuteScalar(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = cmdType;
                    if (pms != null)
                    {
                        cmd.Parameters.AddRange(pms);
                    }
                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }
        //3.执行查询，返回多行，多列的方法
        //ExecuteReader()
        /// <summary>
        /// 获取表中的多行数据（有带参数）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            SqlConnection con = new SqlConnection(conStr);
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = cmdType;
                if (pms != null)
                {
                    cmd.Parameters.AddRange(pms);
                }
                try
                {
                    con.Open();
                    return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                }
                catch
                {
                    con.Close();
                    con.Dispose();
                    throw;
                }
            }
        }
        /// <summary>
        /// 返回一个DataTable的方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pms"></param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string sql, CommandType cmdType, params SqlParameter[] pms)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conStr))
            {
                adapter.SelectCommand.CommandType = cmdType;
                if (pms != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(pms);
                }
                adapter.Fill(dt);
            }
            return dt;
        }
    }
}