using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCommon
{
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.UtcNow;
    }
}
