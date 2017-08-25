using System;

namespace Sift.Common.Util
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime d)
        {
            return (d.Ticks - 621355968000000000) / 10000000;
        }
    }
}
