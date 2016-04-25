using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Article.WebSite.Component.SystemFramework.Models;
using Article.WebSite.Component.SystemFramework.Extensions;

namespace Article.WebSite.Advertise.BusinessData
{
    public class ArticleModel:BaseModel
    {
        public ArticleModel() : base() { }
        public ArticleModel(DataRow row) : base(row) { }

        /// <summary>主键ID</summary>
        public int Pkid { get; set; }
        /// <summary>目录</summary>
        public int Catalog { get; set; }
        /// <summary>内容大图</summary>
        public string image { get; set; }
        /// <summary>标题小图</summary>
        public string SmallImage { get; set; }
        /// <summary>内容大标题</summary>
        public string Titlle { get; set; }
        /// <summary>视图小标题</summary>
        public string SmallTitlle { get; set; }
        /// <summary>内容</summary>
        public string Content { get; set; }
        /// <summary>列表页展示内容</summary>
        public string SmallContent { get; set; }
        /// <summary>内容指向地址</summary>
        public string ContentUrl { get; set; }
        /// <summary>源地址</summary>
        public string Source { get; set; }
        /// <summary>发布时间（展示时间）</summary>
        public string PublishDateTime { get; set; }
        /// <summary>创建时间</summary>
        public string CreateDateTime { get; set; }
        /// <summary>最后修改时间</summary>
        public string LastUpdateTime { get; set; }
        /// <summary>点击数</summary>
        public int ClickCount { get; set; }
        /// <summary>重定向地址</summary>
        public string RedireUrl { get; set; }
        /// <summary>点赞数</summary>
        public int Vote { get; set; }
        /// <summary>类别</summary>
        public string Category { get; set; }
        /// <summary>类别别名</summary>
        public string CategoryAlias { get; set; }
        /// <summary>火热度</summary>
        public int Heat { get; set; }
        /// <summary>是否属于热门文章</summary>
        public bool HotArticles { get; set; }
        /// <summary>文章另一种类别</summary>
        public int Type { get; set; }
        /// <summary>是否展示（1：展示  0：隐藏）</summary>
        public int IsShow { get; set; }
        /// <summary>是否开启评论</summary>
        public bool IsComment { get; set; }
        /// <summary>用户名</summary>
        public string UserName { get; set; }
        /// <summary>标签</summary>
        public string BestLabel { get; set; }
        /// <summary>文章内容文本（扩展字段）</summary>
        public long ArticleText { get; set; }

        //protected override void Parse(DataRow row)
        //{
        //    this.Pkid = Convert.ToInt32(row["Pkid"]);
        //    this.Catalog = Convert.ToInt32(row["Catalog"]);
        //    this.image = row.GetValue("image");
        //    this.SmallImage = row.GetValue("SmallImage");
        //    this.Titlle = row.GetValue("Titlle");
        //    this.SmallTitlle = row.GetValue("SmallTitlle");
        //    this.Content = row.GetValue("Content");
        //    this.SmallContent = row.GetValue("SmallContent");
        //    this.ContentUrl = row.GetValue("ContentUrl");
        //    this.Source = row.GetValue("Source");
        //}
    }
}
