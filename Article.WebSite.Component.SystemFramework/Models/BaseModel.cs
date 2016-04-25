using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json;
using Article.WebSite.Component.SystemFramework.Extensions;

namespace Article.WebSite.Component.SystemFramework.Models
{
    /// <summary>模型基类</summary>
    public abstract class BaseModel
    {
        private static readonly Type _GuidType = typeof(Guid);
        private static readonly Type _StringType = typeof(string);

        protected BaseModel() { }
        protected BaseModel(DataRow row)
        {
            Parse(row);
        }

        protected virtual void Parse(DataRow row)
        {
            if (row == null)
                return;

            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in properties)
            {
                var attr = GetColumnAttribute(property);

                if (row.HasValue(attr.Name))
                    try
                    {
                        property.SetValue(this, ConvertType(CultureInfo.CurrentCulture, row[attr.Name], attr.Type), null);
                    }
                    catch /*(Exception ex) when (!(ex is FileNotFoundException))*/ { }
            }
        }

        protected virtual ColumnAttribute GetColumnAttribute(PropertyInfo property)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;

            if (attr == null)
                attr = new ColumnAttribute(property.Name);

            attr.Type = attr.Type ?? property.PropertyType;

            return attr;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T Parse<T>(DataRow row) where T : BaseModel, new()
        {
            if (row == null)
                return null;

            var obj = new T();

            obj.Parse(row);

            return obj;
        }

        private static object ConvertType(CultureInfo culture, object value, Type destinationType)
        {
            if ((value == null) || destinationType.IsInstanceOfType(value))
                return value;

            if (destinationType == _StringType)
                return value.ToString();

            destinationType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            if (destinationType.IsInstanceOfType(value))
                return value;

            try
            {
                return Convert.ChangeType(value, destinationType);
            }
            catch (Exception ex)
            {
                var converter = TypeDescriptor.GetConverter(destinationType);
                if (converter.CanConvertFrom(value.GetType()))
                    return converter.ConvertFrom(null, culture, value);

                converter = TypeDescriptor.GetConverter(value.GetType());
                if (converter.CanConvertTo(destinationType))
                    return converter.ConvertTo(null, culture, value, destinationType);

                if (destinationType == _GuidType)
                    return Guid.Parse(value.ToString());
                else if (destinationType.IsEnum)
                    try
                    {
                        return Enum.ToObject(destinationType, Convert.ToUInt64(value));
                    }
                    catch
                    {
                        return Enum.Parse(destinationType, value.ToString());
                    }

                if (value is string)
                {
                    var json = (value as string).Trim();
                    if (json.Length > 1 && (json[0] == '{' || json[0] == '['))
                        try
                        {
                            return JsonConvert.DeserializeObject(json, destinationType);
                        }
                        catch { }
                }

                throw ex;
            }
        }
    }

    /// <summary>逻辑字符串比较，2string比20string小</summary>
    [Obsolete("请安装Tuhu包", true)]
    public class LogicalStringComparer : StringComparer
    {
        private static readonly LogicalStringComparer _default;
        public static LogicalStringComparer Default { get { return _default; } }

        private readonly StringComparison _comparisonType;
        public StringComparison ComparisonType { get { return _comparisonType; } }
        private readonly StringComparer comparer;

        static LogicalStringComparer()
        {
            _default = new LogicalStringComparer();
        }

        public LogicalStringComparer() : this(StringComparison.CurrentCultureIgnoreCase) { }

        public LogicalStringComparer(StringComparison comparisonType)
        {
            switch (comparisonType)
            {
                case StringComparison.CurrentCulture:
                    comparer = StringComparer.CurrentCulture;
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                    comparer = StringComparer.CurrentCultureIgnoreCase;
                    break;
                case StringComparison.InvariantCulture:
                    comparer = StringComparer.InvariantCulture;
                    break;
                case StringComparison.InvariantCultureIgnoreCase:
                    comparer = StringComparer.InvariantCultureIgnoreCase;
                    break;
                case StringComparison.Ordinal:
                    comparer = StringComparer.Ordinal;
                    break;
                case StringComparison.OrdinalIgnoreCase:
                    comparer = StringComparer.OrdinalIgnoreCase;
                    break;
                default:
                    throw new ArgumentException("comparisonType");
            }
            this._comparisonType = comparisonType;
        }

        public override int Compare(string strA, string strB)
        {
            if (strA == null)
            {
                if (strB == null)
                    return 0;
                else
                    return 1;
            }
            else if (strA.Length == 0)
            {
                if (strB == null)
                    return 1;
                else if (strB.Length == 0)
                    return 0;
                else
                    return -1;
            }
            else if (string.IsNullOrEmpty(strB))
                return 1;
            else
            {
                var charA = strA[0];
                var charB = strB[0];

                if (((charA >= '0' && charA <= '9')
                        && (charB >= '0' && charB <= '9'))
                    || ((charA < '0' || charA > '9')
                        && (charB < '0' || charB > '9')))
                    //如果第一个字符都是数字或者都不是数字
                    return InternalCompare(strA, strB);
                else
                    return comparer.Compare(strA, strB);
            }
        }

        private int InternalCompare(string strA, string strB)
        {
            int indexA = 0, indexB = 0, result = 0;
            bool isNumberA, isNumberB;

            while (true)
            {
                var lengthA = GetRange(strA, indexA, out isNumberA);
                var lengthB = GetRange(strB, indexB, out isNumberB);

                if (lengthA == 0 || lengthB == 0)
                {
                    if (lengthA == 0)
                    {
                        if (lengthB != 0)
                            result = -1;
                    }
                    else
                    {
                        if (lengthB == 0)
                            result = 1;
                    }
                    break;
                }
                else
                {
                    if (isNumberA != isNumberB)
                    {
                        //如果其中一个是数字另一个不是数字
                        result = comparer.Compare(strA, strB);
                        break;
                    }
                    else
                    {
                        var subA = strA.Substring(indexA, lengthA);
                        var subB = strB.Substring(indexB, lengthB);
                        if (!isNumberA)
                        {
                            //如果不是数字
                            result = comparer.Compare(subA, subB);
                        }
                        else
                        {
                            if (lengthA > 19 || lengthB > 19)
                            {
                                //如果是大数
                                result = BigInteger.Parse(subA).CompareTo(BigInteger.Parse(subB));
                                if (result == 0)
                                    result = comparer.Compare(subA, subB);
                            }
                            else
                            {
                                result = UInt64.Parse(subA).CompareTo(UInt64.Parse(subB));
                                if (result == 0)
                                    result = comparer.Compare(subA, subB);
                            }
                        }
                        if (result != 0)
                            break;

                        indexA += lengthA;
                        indexB += lengthB;
                    }
                }
            }

            return result;
        }

        private int GetRange(string str, int startIndex, out bool isNumber)
        {
            var stringLength = str.Length;
            var length = 0;
            isNumber = false;

            if (stringLength > startIndex)
            {
                isNumber = str[startIndex] >= '0' && str[startIndex] <= '9';

                while (stringLength > startIndex)
                {
                    if (isNumber)
                    {
                        if (str[startIndex] < '0' || str[startIndex] > '9')
                            break;
                    }
                    else
                    {
                        if (str[startIndex] >= '0' && str[startIndex] <= '9')
                            break;
                    }
                    startIndex++;
                    length++;
                }
            }

            return length;
        }

        public override bool Equals(string x, string y)
        {
            return Compare(x, y) == 0;
        }

        public override int GetHashCode(string obj)
        {
            return comparer.GetHashCode(obj);
        }
    }
}