using System;

namespace GridDomain.Common
{
    [Obsolete("use BusinessDateTime instead for better naming")]
    public static class DateTimeFacade
    {
        public static DateTime Now => BusinessDateTime.Now;
        public static DateTime UtcNow => BusinessDateTime.UtcNow;
    }
}