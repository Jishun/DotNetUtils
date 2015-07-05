using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace DotNetUtils
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            string description = String.Empty;
            var descriptionAttribute = value.GetAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                description = descriptionAttribute.Description;
            }
            else
            {
                description = value.ToString();
            }

            return description;
        }

        public static T GetAttribute<T>(this Enum value) where T : System.Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            return (T)Attribute.GetCustomAttribute(fi, typeof(T));
        }
    }
}
