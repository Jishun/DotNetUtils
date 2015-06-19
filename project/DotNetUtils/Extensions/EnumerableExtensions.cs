using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> src, T item)
        {
            foreach (var i in src)
            {
                yield return i;
            }
            yield return item;
        }
    }
}
