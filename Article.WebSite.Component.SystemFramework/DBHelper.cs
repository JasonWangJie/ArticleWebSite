using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//访问数据库
using System.Data;
using System.Data.SqlClient;
//访问App.config
using System.Configuration;

namespace Article.WebSite.Component.SystemFramework
{
    /// <summary>
    /// 封装对数据库的常用操作（增删改查）
    /// </summary>
    public class DBHelper
    {
        
        private SqlConnection conn = null;
        private SqlCommand cmd = null;
        private SqlDataReader sdr = null;
        public DBHelper()
        {
            //从App.config中读取连接字符串
            string constr = ConfigurationManager.ConnectionStrings["dbcon"].ConnectionString;
            conn = new SqlConnection(constr);
        }
        private SqlConnection GetConn()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        /// <summary>  
        ///  执行不带参数的增删改SQL语句或存储过程  
        /// </summary>  
        /// <param name="cmdText">增删改SQL语句或存储过程</param>  
        /// <param name="ct">命令类型</param>  
        /// <returns></returns>  
        public int ExecuteNonQuery(string cmdText, CommandType ct)
        {
            int res;
            try
            {
                cmd = new SqlCommand(cmdText, GetConn());
                cmd.CommandType = ct;
                res = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return res;
        }

        /// <summary>  
        ///  执行带参数的增删改SQL语句或存储过程  
        /// </summary>  
        /// <param name="cmdText">增删改SQL语句或存储过程</param>  
        /// <param name="ct">命令类型</param>  
        /// <returns>int值</returns>  
        public int ExecuteNonQuery(string cmdText, SqlParameter[] paras, CommandType ct)
        {
            int res;
            using (cmd = new SqlCommand(cmdText, GetConn()))
            {
                cmd.CommandType = ct;
                cmd.Parameters.AddRange(paras);
                res = cmd.ExecuteNonQuery();
            }
            return res;
        }

        /// <summary>  
        ///  执行查询SQL语句或存储过程  
        /// </summary>  
        /// <param name="cmdText">查询SQL语句或存储过程</param>  
        /// <param name="ct">命令类型</param>  
        /// <returns>Table值</returns>  
        public DataTable ExecuteDataTable(string cmdText, CommandType ct)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(cmdText, GetConn());
            cmd.CommandType = ct;
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dt.Load(sdr);
            }
            return dt;
        }

        /// <summary>  
        ///  执行带参数的查询SQL语句或存储过程  
        /// </summary>  
        /// <param name="cmdText">查询SQL语句或存储过程</param>  
        /// <param name="paras">参数集合</param>  
        /// <param name="ct">命令类型</param>  
        /// <returns>Table值</returns>  
        public DataTable ExecuteDataTable(string cmdText, SqlParameter[] paras, CommandType ct)
        {
            DataTable dt = new DataTable();
            cmd = new SqlCommand(cmdText, GetConn());
            cmd.CommandType = ct;
            cmd.Parameters.AddRange(paras);
            using (sdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                dt.Load(sdr);
            }
            return dt;
        }

        /// <summary>  
        /// 执行带参数的Scalar查询  
        /// </summary>  
        /// <param name="cmdText">查询SQL语句或存储过程</param>  
        /// <param name="paras">参数集合</param>  
        /// <param name="ct">命令类型</param>  
        /// <returns>一个int型值</returns>  
        public int ExecuteScalar(string cmdText, SqlParameter[] paras, CommandType ct)
        {
            int result;
            using (cmd = new SqlCommand(cmdText, GetConn()))
            {
                cmd.CommandType = ct;
                cmd.Parameters.AddRange(paras);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }
    }
}
