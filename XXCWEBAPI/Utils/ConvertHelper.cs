using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using XXCWEBAPI.Models;

namespace XXCWEBAPI.Utils
{
    /// <summary>
    /// ConvertHelper 的摘要说明
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// Table转json
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static string SerializeDataTableToJson(DataTable dt)
        {
            string rtn = "";
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss" };
            rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, timeConverter);
            return rtn;
          
        }
        /// <summary>
        /// table转json
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    if (dt.Columns[j].ColumnName == "VName" || dt.Columns[j].ColumnName == "VCertificatePhoto" || dt.Columns[j].ColumnName == "VLocalePhoto" || dt.Columns[j].ColumnName == "VAddress" || dt.Columns[j].ColumnName == "VCertificateNumber" || dt.Columns[j].ColumnName == "VMobilePhone")
                    {
                        jsonBuilder.Append((dt.Rows[i][j].ToString()).Replace("\"", "\\\""));
                    }
                    else {
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\""));
                    }
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }
        private static string statusOK = "1";
        private static string statusError = "0";
        /// <summary>
        /// int转json
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string IntToJson(int result)
        {
            string code = result == 0 ? statusError : statusOK;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"code\":\"" + code + "\"}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 结果信息提示
        /// </summary>
        /// <param name="code">状态</param>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        public static string resultJson(int status,string msg)
        {
            string code = status.ToString();
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"code\":\"" + code + "\",\"msg\":\""+ msg +"\"}");
            return jsonBuilder.ToString();
        }
        #region 将List<>转换为Json
        public static string List2JSON(List<VisitorAccessInf> objlist)
        {
            string result = "";

            result += "[";
            bool firstline = true;//处理第一行前面不加","号
            foreach (object oo in objlist)
            {
                if (!firstline)
                {
                    result = result + "," + OneObjectToJSON(oo);
                }
                else
                {
                    result = result + OneObjectToJSON(oo) + "";
                    firstline = false;
                }
            }
            return result + "]";
        }

        private static string OneObjectToJSON(object o)
        {
            string result = "{";
            List<string> ls_propertys = new List<string>();
            ls_propertys = GetObjectProperty(o);
            foreach (string str_property in ls_propertys)
            {
                if (result.Equals("{"))
                {
                    result = result + str_property;
                }
                else
                {
                    result = result + "," + str_property + "";
                }
            }
            return result + "}";
        }

        private static List<string> GetObjectProperty(object o)
        {
            List<string> propertyslist = new List<string>();
            PropertyInfo[] propertys = o.GetType().GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                propertyslist.Add("\"" + p.Name.ToString() + "\":\"" + p.GetValue(o, null) + "\"");
            }
            return propertyslist;
        }

        #endregion
    }
}