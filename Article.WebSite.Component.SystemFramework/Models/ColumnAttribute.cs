using System;

namespace Article.WebSite.Component.SystemFramework.Models
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class ColumnAttribute : Attribute
	{
		private readonly string _name;
		private string _typeName;
		private Type _type;

		public ColumnAttribute(string name) : this(name, null) { }

		public ColumnAttribute(string name, string typeName)
		{
			this._name = name;
			this._typeName = typeName;
		}

		public string Name { get { return this._name; } }

		public string TypeName
		{
			get { return this._typeName; }
			set { this._typeName = value; }
		}

		public Type Type
		{
			get
			{
				if (_type == null)
					if (_typeName != null)
						_type = System.Type.GetType(_typeName);
				return _type;
			}
			set
			{
				_type = value;
				_typeName = value == null ? null : value.AssemblyQualifiedName;
			}
		}
	}
}
