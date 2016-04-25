using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Article.WebSite.Component.SystemFramework.Models;
using Article.WebSite.Component.SystemFramework.Extensions;
using System.Data;

namespace Article.WebSite.Advertise.BusinessData
{
    public class CategoryListModel:BaseModel
    {
        public CategoryListModel() : base() { }
        public CategoryListModel(DataRow row) : base(row) { }
        /// <summary>主键标识列</summary>
        public int Pkid { get; set; }
        /// <summary>类别名</summary>
        public string Category { get; set; }
        /// <summary>类别另类展示</summary>
        public string CategoryAlias { get; set; }
        /// <summary>类别地址</summary>
        public string CategoryUrl { get; set; }
        /// <summary>是否显示</summary>
        public bool IsShow { get; set; }
        /// <summary>排序字段（升序）</summary>
        public int Sort { get; set; }
        /// <summary>父级ID-->Pkid</summary>
        public int ParentId { get; set; }
    }
}
