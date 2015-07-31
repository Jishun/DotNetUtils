using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace DotNetUtils
{
    public static class DataTypeExtension
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> ids)
        {
            var ret = new DataTable();
            ret.Columns.Add("Id");
            foreach (var id in ids)
            {
                var row = ret.NewRow();
                row["Id"] = id;
                ret.Rows.Add(row);
            }
            return ret;
        }

        public static string ToStringTrim(this decimal number)
        {
            return number.ToString("G29");
        }
        public static bool IsNumericType(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNumericType(this object o)
        {
            return o.GetType().IsNumericType();
        }

        public static bool CanCastTo(this Type from, Type to)
        {
            return to.IsAssignableFrom(from)
                || IsCastDefined(to, m => m.GetParameters()[0].ParameterType, _ => from, false)
                || IsCastDefined(from, _ => to, m => m.ReturnType, true);
        }

        private static bool IsCastDefined(IReflect type, Func<MethodInfo, Type> baseType, Func<MethodInfo, Type> derivedType, bool lookInBase)
        {
            var bindinFlags = BindingFlags.Public
                            | BindingFlags.Static
                            | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            return type.GetMethods(bindinFlags).Any(m => (m.Name == "op_Implicit" || m.Name == "op_Explicit")
                                                      && baseType(m).IsAssignableFrom(derivedType(m)));
        }

        public static T ChangeType<T>(this object value) 
        {
           var t = typeof(T);

           if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)) 
           {
               if (value == null) 
               { 
                   return default(T); 
               }

               t = Nullable.GetUnderlyingType(t);
           }

           return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeType(this object value, Type conversion) 
        {
           var t = conversion;

           if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)) 
           {
               if (value == null) 
               { 
                   return null; 
               }

               t = Nullable.GetUnderlyingType(t);
           }

           return Convert.ChangeType(value, t);
        }
        
        public static T SafeConvert<T>(this object value, T defaultValue, string dateFormat = null) 
        {
            if (value == null)
            {
                return defaultValue;
            }
            if (typeof(T) == typeof(object))
            {
                return (T)value;
            }
            if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
            {
                return (T)((object)value.ParseDateTimeNullable(dateFormat) ?? defaultValue);
            }
            var t = value.GetType();
            if (t == typeof(T))
            {
                return (T) value;
            }
            if (t.CanCastTo(typeof(T)))
            {
                if (t.IsValueType)
                {
                    return value.ChangeType<T>();
                }
                return (T) (value);
            }
            var convert = TypeDescriptor.GetConverter(typeof(T));
            if (convert.CanConvertFrom(value.GetType()))
            {
                return (T)convert.ConvertFrom(value);
            }
            return defaultValue;
        }
    }
}
