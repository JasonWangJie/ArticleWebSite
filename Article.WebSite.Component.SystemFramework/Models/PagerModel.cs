using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuhu.WebSite.Component.SystemFramework.Models
{
	/// <summary>
	/// 分页模型
	/// </summary>
	public class PagerModel
	{
		/// <summary>ctor</summary>
		public PagerModel() : this(1, 20) { }
		/// <summary>ctor</summary>
		/// <param name="currentPage">当前页</param>
		public PagerModel(int currentPage) : this(currentPage, 20) { }
		/// <summary>ctor</summary>
		/// <param name="currentPage">当前页</param>
		/// <param name="pageSize">页大小</param>
		public PagerModel(int currentPage, int pageSize)
		{
			this.CurrentPage = currentPage < 1 ? 1 : currentPage;
			this.PageSize = pageSize < 1 ? 20 : pageSize;
		}

		/// <summary>当前页，默认为第一页</summary>
		public int CurrentPage { get; set; }
		/// <summary>每页数量，默认为20</summary>
		public int PageSize { get; set; }
		/// <summary>总项目数</summary>
		public int TotalItem { get; set; }
		/// <summary>总页数</summary>
		public int TotalPage { get { return (TotalItem + PageSize - 1) / PageSize; } }
	}
}
