using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdf
{
    public class TimeValue
    {
        public long Time { get; private set; }

        public TimeValue()
        {
            Time = 0;
        }

        public TimeValue(long time)
        {
            Time = time;
        }

        //TimeSpan.FromMilliseconds(BitConverter.ToInt64(binary, 0))
    }
}
