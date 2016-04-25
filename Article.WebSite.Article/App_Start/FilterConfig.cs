using System.Web;
using System.Web.Mvc;

namespace Article.WebSite.Article
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
