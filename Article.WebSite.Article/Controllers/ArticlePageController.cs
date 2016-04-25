using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Article.WebSite.Advertise.BusinessFacade;
using Article.WebSite.Advertise.BusinessData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web.Caching;

namespace Article.WebSite.Article.Controllers
{
    public class ArticlePageController : Controller
    {
        // GET: ArticlePage
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 文章详情页面
        /// </summary>
        /// <param name="pkId"></param>
        /// <returns></returns>
        public ActionResult ArticleDetail(int pkId)
        {
            ViewBag.pkId = pkId;
            return View();
        }

        /// <summary>
        /// 文章分类/标签查找
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public ActionResult SelectArticleForCategory(string category)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            IEnumerable<ArticleModel> Article;
            try
            {
                if (HttpRuntime.Cache["AllArticle_Cache"] != null)
                {
                    var artLists = HttpRuntime.Cache["AllArticle_Cache"] as IEnumerable<ArticleModel>;
                    Article = artLists;
                }
                else
                {
                    //缓存中没有则取数据并写入数据
                    Article = ArticlesSystem.SelectAllArticle();
                    HttpRuntime.Cache.Insert("AllArticle_Cache", Article, null, DateTime.Now.AddMinutes(10),
                        Cache.NoSlidingExpiration);
                }
                if(Article!=null && Article.Any())
                {
                    Article = Article.Where(x => category.Contains(x.BestLabel));
                    dic.Add("Code", "1");
                    dic.Add("Article", Article);
                }
                else
                {
                    dic.Add("Code","0");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Erro", ex.ToString());
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }
    }
}