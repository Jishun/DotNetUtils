using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetUtils
{
    public class Triple<TFirst, TSecond, TThird> : Pair<TFirst, TSecond>
    {
        public TThird Third;

        public Triple(TFirst first, TSecond second, TThird third) : base(first, second)
        {
            Third = third;
        }

        public Triple()
        {
            
        }
    }
}
