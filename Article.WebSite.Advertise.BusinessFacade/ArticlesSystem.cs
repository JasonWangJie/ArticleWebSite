using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Article.WebSite.Advertise.DataAccess;
using Article.WebSite.Advertise.BusinessData;
using Article.WebSite.Component.SystemFramework.Models;

namespace Article.WebSite.Advertise.BusinessFacade
{
    public static class ArticlesSystem
    {
        /// <summary>
        /// 查询所有文章
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectAllArticle()
        {
            var dt = Articles.SelectAllArticle();
            if (dt == null && dt.Rows.Count == 0)
                return new ArticleModel[0];
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }

        /// <summary>
        /// 查询所有类别
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CategoryListModel> SelectCategoryList()
        {
            var dt = Articles.SelectCategoryList();
            if (dt == null && dt.Rows.Count == 0)
                return new CategoryListModel[0];
            return dt.Rows.Cast<DataRow>().Select(x => new CategoryListModel(x)).ToArray();
        }

        /// <summary>
        /// 分页查询所有文章/搜索
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static IEnumerable<ArticleModel> SelectArticlePage(string condition, int pageSize, int pageNum, out int totalCount)
        {
            var dt = Articles.SelectArticlePage(condition, pageSize, pageNum,out totalCount);
            if (dt == null && dt.Rows.Count == 0)
                return new ArticleModel[0];
            return dt.Rows.Cast<DataRow>().Select(x => new ArticleModel(x)).ToArray();
        }
    }
}
