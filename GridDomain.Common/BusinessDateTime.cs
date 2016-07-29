using System;

namespace GridDomain.Common
{
    public static class BusinessDateTime
    {
        public static DateTime Now => DateTimeStrategyHolder.Current.Now;
        public static DateTime UtcNow => DateTimeStrategyHolder.Current.UtcNow;
    }
}