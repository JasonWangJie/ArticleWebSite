using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Article.WebSite.Advertise.BusinessFacade;
using Article.WebSite.Advertise.BusinessData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Article.WebSite.Article.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PublicPage()
        {
            return View();
        }

        /// <summary>
        /// 文章分页/查询
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectArticlePage(string condition,int pageNum = 1,int pageSize = 5)
        {
            var dic = new Dictionary<string, object>();
            IEnumerable<ArticleModel> ArticlePage;
            int totalCount = 0;
            try
            {
                if (HttpRuntime.Cache["AllArticlePage_Cache" + pageSize + "_pageSize" + pageNum + "_pageNum" + condition + "_condition"] != null)
                {
                    var artLists = HttpRuntime.Cache["AllArticlePage_Cache" + pageSize + "_pageSize" + pageNum + "_pageNum" + condition + "_condition"] as IEnumerable<ArticleModel>;
                    ArticlePage = artLists;
                }
                else
                {
                    //缓存中没有则取数据并写入数据
                    ArticlePage = ArticlesSystem.SelectArticlePage(condition, pageSize, pageNum, out totalCount);
                    HttpRuntime.Cache.Insert("AllArticlePage_Cache"+pageSize+"_pageSize"+pageNum+"_pageNum"+condition+"_condition", ArticlePage, null, DateTime.Now.AddMinutes(5),
                        Cache.NoSlidingExpiration);
                }
                if(ArticlePage!=null && ArticlePage.Any())
                {
                    dic.Add("Code", "1");
                    dic.Add("ArticlePage", ArticlePage);
                    dic.Add("totalCount", totalCount);
                }
                else
                {
                    dic.Add("Code", "0");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Erro", ex.ToString());
            }
            return Json(dic,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询所有文章
        /// </summary>
        /// <returns></returns>
        //[OutputCache(CacheProfile = "ArticleCacheProfile", VaryByParam = "*")]
        public ActionResult SelectAllArticle(string condition, int pageSize = 1, int pageNum = 5)
        {
            #region MyRegion
            //if (HttpRuntime.Cache["API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category"] != null)
            //{
            //    var artLists = HttpRuntime.Cache["API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category"] as Dictionary<string, object>;
            //    Result = artLists["artList"] as List<ArticleModel>;

            //}
            //else
            //{
            //    //缓存中没有则取数据并写入数据
            //    Result = ArticlesSystem.SelectAllArticle();
            //    HttpRuntime.Cache.Insert("API_Cache_Article" + pIndex + "_pIndex" + pSize + "_pSize" + Category + "_Category", dics, null, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration);
            //}
            #endregion
            var dic =new Dictionary<string,object>();
            IEnumerable<ArticleModel> Article;
            IEnumerable<ArticleModel> ArticlePage;
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
                    HttpRuntime.Cache.Insert("AllArticle_Cache", Article, null, DateTime.Now.AddMinutes(5),
                        Cache.NoSlidingExpiration);
                }

                //热门文章
                var hotArticle = Article.Where(x=>x.HotArticles==true).Select(x => new
                {
                    PKID=x.Pkid,
                    SmallTittle=x.SmallTitlle,
                    ClickCount = x.ClickCount
                });
                //热门标签
                var hotLabel = Article.Where(e=>e.HotArticles==true).Select(x=>new
                {
                    BestLabel = x.BestLabel,
                    LabelCount = Article.Where(z => z.BestLabel == x.BestLabel).Count()
                });
                hotLabel = (from list in hotLabel select list).Distinct();
                //文章类别
                IEnumerable<CategoryListModel> articleCategory;
                if (HttpRuntime.Cache["AllCategory_Cache"] != null)
                {
                    var artLists = HttpRuntime.Cache["AllCategory_Cache"] as IEnumerable<CategoryListModel>;
                    articleCategory = artLists;
                }
                else
                {
                    articleCategory = ArticlesSystem.SelectCategoryList();
                    HttpRuntime.Cache.Insert("AllCategory_Cache", articleCategory, null, DateTime.Now.AddMinutes(5),
                        Cache.NoSlidingExpiration);
                }
                
                dic.Add("Code", "1");
                dic.Add("AllArticle", Article);
                dic.Add("hotArticle", hotArticle);
                dic.Add("hotLabel", hotLabel);
                dic.Add("articleCategory", articleCategory.Where(x=>x.ParentId==0));
            }
            catch (Exception ex)
            {
                dic.Add("Code", "0");
                dic.Add("Erro", ex.ToString());
            }
            //return JavaScript(JsonConvert.SerializeObject(dic, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:mm" }));
            return Json(dic,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看文章详情
        /// </summary>
        /// <param name="pkId"></param>
        /// <returns></returns>
        public ActionResult GetArticleForPkid(int pkId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                IEnumerable<ArticleModel> Article;
                if (HttpRuntime.Cache["AllArticle_Cache"] != null)
                {
                    var artLists = HttpRuntime.Cache["AllArticle_Cache"] as IEnumerable<ArticleModel>;
                    Article = artLists;
                }
                else
                {
                    //缓存中没有则取数据并写入数据
                    Article = ArticlesSystem.SelectAllArticle();
                    HttpRuntime.Cache.Insert("AllArticle_Cache", Article, null, DateTime.Now.AddMinutes(1),
                        Cache.NoSlidingExpiration);
                }
                if(Article!=null && Article.Any())
                {
                    Article = Article.Where(x=>x.Pkid==pkId);
                    dic.Add("Code","1");
                    dic.Add("ReadyArticle", Article);
                }
                else
                {
                    dic.Add("Code", "0");
                }
            }
            catch (Exception ex)
            {
                dic.Add("Code","0");
                dic.Add("Erro", ex.ToString());
            }
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据父级菜单的id 查出下面子集菜单
        /// </summary>
        /// <param name="pkId"></param>
        /// <returns></returns>
        public ActionResult SelectCategoryChild(int pkId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            try
            {
                //文章类别
                IEnumerable<CategoryListModel> articleCategory;
                if (HttpRuntime.Cache["AllCategory_Cache"] != null)
                {
                    var artLists = HttpRuntime.Cache["AllCategory_Cache"] as IEnumerable<CategoryListModel>;
                    articleCategory = artLists;
                }
                else
                {
                    articleCategory = ArticlesSystem.SelectCategoryList();
                    HttpRuntime.Cache.Insert("AllCategory_Cache", articleCategory, null, DateTime.Now.AddMinutes(30),
                        Cache.NoSlidingExpiration);
                }
                articleCategory = articleCategory.Where(x=>x.ParentId==pkId);
                dic.Add("Code","1");
                dic.Add("articleCategoryChild", articleCategory);
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