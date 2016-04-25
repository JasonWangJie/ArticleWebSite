using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Article.WebSite.Component.SystemFramework;

namespace Article.WebSite.Advertise.DataAccess
{
    public class Articles
    {
        /// <summary>
        /// 查询所有文章
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectAllArticle()
        {
            DBHelper db = new DBHelper();
            string cmd = @"SELECT  ba.Pkid,
                        Catalog ,
                        image ,
                        SmallImage ,
                        Titlle ,
                        SmallTitlle ,
                        SmallContent,
                        Content ,
                        ContentUrl ,
                        Source ,
                        CONVERT(VARCHAR(50), ba.PublishDateTime, 120) AS PublishDateTime,
                        CONVERT(VARCHAR(50), ba.CreateDateTime, 120) AS CreateDateTime ,
                        CONVERT(VARCHAR(50), ba.LastUpdateTime, 120) AS LastUpdateTime ,
                        ClickCount ,
                        RedireUrl ,
                        Vote ,
                        cl.Category ,
                        cl.CategoryAlias ,
                        Heat ,
                        HotArticles ,
                        Type ,
                        ba.IsShow ,
                        IsComment ,
                        UserName ,
                        BestLabel ,
                        ArticleText
                FROM    Gungnir..BBSAtrticle AS ba ( NOLOCK )
                        LEFT JOIN Gungnir..CategoryList AS cl ( NOLOCK ) ON ba.Category = cl.Pkid
                ORDER BY ba.LastUpdateTime DESC;";
            return db.ExecuteDataTable(cmd, CommandType.Text);
        }

        /// <summary>
        /// 分页查询所有文章/搜索
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static DataTable SelectArticlePage(string condition, int pageSize,int pageNum, out int totalCount)
        {
            DBHelper db = new DBHelper();
            if (string.IsNullOrWhiteSpace(condition))
                condition = "0";
            string sql = "Gungnir..SelectArticlePage";
            SqlParameter[] parm = new SqlParameter[]
            {
                new SqlParameter("@PageSize",pageSize),
                new SqlParameter("@PageNumber",pageNum),
                new SqlParameter("@condition",condition),
                new SqlParameter() {
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.Int
                }
            };
            DataTable dt = db.ExecuteDataTable(sql, parm, CommandType.StoredProcedure);
            totalCount = Convert.ToInt32(parm[3].Value);
            return dt;
        }

        /// <summary>
        /// 查询所有类别
        /// </summary>
        /// <returns></returns>
        public static DataTable SelectCategoryList()
        {
            DBHelper db = new DBHelper();
            string cmd = @"SELECT * FROM Gungnir..CategoryList AS cl( NOLOCK )";
            return db.ExecuteDataTable(cmd, CommandType.Text);
        }

    }
}
