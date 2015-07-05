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
    }
}
