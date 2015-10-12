using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public class Pair<TFirst, TSecond>
    {
        public TFirst First;
        public TSecond Second;

        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public Pair()
        {
            
        }
    }
}
