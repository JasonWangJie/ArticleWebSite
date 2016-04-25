using System.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using Article.WebSite.Component.SystemFramework.Models;
using System.Collections;

namespace Article.WebSite.Component.SystemFramework.Extensions
{
	public static class DataRowExtension
	{
		/// <summary>判断列是否有值</summary>
		/// <param name="row">对象</param>
		/// <param name="name">列名</param>
		/// <returns>是否存在</returns>
		public static bool HasValue(this DataRow row, string name)
		{
			if (row == null || !row.Table.Columns.Contains(name))
				return false;

			return !row.IsNull(name);
		}
		/// <summary>获得值</summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="row">对象</param>
		/// <param name="name">列名</param>
		/// <returns>值</returns>
		public static T GetValue<T>(this DataRow row, string name)
		{
			if (row == null || !row.HasValue(name))
				return default(T);

			var value = Convert.ChangeType(row[name], typeof(T));

			if (value == null)
				return default(T);
			return (T)value;
		}
		/// <summary>获得值</summary>
		/// <param name="row">对象</param>
		/// <param name="name">列名</param>
		/// <returns>值</returns>
		public static string GetValue(this DataRow row, string name)
		{
			if (row == null || !row.HasValue(name))
				return null;

			return row[name].ToString();
		}

		public static T ConvertTo<T>(this DataRow row) where T : BaseModel, new()
		{
			return BaseModel.Parse<T>(row);
		}

		public static IEnumerable<T> ConvertTo<T>(this DataTable dt) where T : BaseModel, new()
		{
			if (dt == null || dt.Rows.Count == 0)
				return new T[0];
			else
				return new DataTableConverterEnumerable<T>(dt);
		}
	}

	internal class DataTableConverterEnumerable<T> : IEnumerable<T>, IEnumerable where T : BaseModel, new()
	{
		private readonly IEnumerator<T> rowsEnumerator;

		public DataTableConverterEnumerable(DataTable dt)
		{
			rowsEnumerator = new DataTableConverterEnumerator(dt.Rows);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return rowsEnumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private class DataTableConverterEnumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			private readonly IEnumerator rowsEnumerator;

			public DataTableConverterEnumerator(DataRowCollection rows)
			{
				this.rowsEnumerator = rows.GetEnumerator();
			}

			object IEnumerator.Current { get { return this.Current; } }
			public T Current { get { return (rowsEnumerator.Current as DataRow).ConvertTo<T>(); } }

			public void Dispose() { }

			public bool MoveNext()
			{
				return rowsEnumerator.MoveNext();
			}

			public void Reset()
			{
				rowsEnumerator.Reset();
			}
		}
	}
}
