using System;

namespace GridDomain.Common
{
    internal class DefaultDateTimeStrategy : ICurrentDateTimeStrategy
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}