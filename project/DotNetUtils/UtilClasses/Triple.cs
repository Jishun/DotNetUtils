using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public class Triple<TFirst, TSecond, TThird>
    {
        public TFirst First;
        public TSecond Second;
        public TThird Third;

        public Triple(TFirst first, TSecond second, TThird third)
        {
            First = first;
            Second = second;
            Third = third;
        }
    }
}
