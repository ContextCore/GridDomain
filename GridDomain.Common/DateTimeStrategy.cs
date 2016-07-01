using System;

namespace GridDomain.Common
{
    public abstract class DateTimeStrategy
    {
        public abstract DateTime Now { get; }
        public abstract DateTime UtcNow { get; }
    }
}