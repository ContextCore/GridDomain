using System;

namespace GridDomain.Common
{
    class DefaultDateTimeStrategy : ICurrentDateTimeStrategy
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
