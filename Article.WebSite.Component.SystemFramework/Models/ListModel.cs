using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tuhu.WebSite.Component.SystemFramework.Models
{
	/// <summary>列表模型</summary>
	/// <typeparam name="T">类型</typeparam>
	public class ListModel<T> : IEnumerable<T>
	{
		/// <summary>
		/// 数据模型
		/// </summary>
		public IEnumerable<T> Source { get; set; }
		/// <summary>
		/// 分布模型
		/// </summary>
		public PagerModel Pager { get; set; }

		public ListModel() { }
		public ListModel(PagerModel pager)
		{
			Pager = pager;
		}
		public ListModel(IEnumerable<T> source)
		{
			Source = source;
		}
		public ListModel(PagerModel pager, IEnumerable<T> source)
		{
			Pager = pager;
			Source = source;
		}
		public ListModel(IEnumerable<T> source, PagerModel pager)
		{
			Source = source;
			Pager = pager;
		}

		#region IEnumerable<T> 成员
		/// <summary>返回一个循环访问集合的枚举器。</summary>
		/// <returns>可用于循环访问集合的 System.Collections.Generic.IEnumerator&lt;T&gt;。</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return Source.GetEnumerator();
		}
		#endregion

		#region IEnumerable 成员
		/// <summary>返回一个循环访问集合的枚举器。</summary>
		/// <returns>可用于循环访问集合的 System.Collections.IEnumerator。</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (Source as IEnumerable).GetEnumerator();
		}
		#endregion
	}
}
