using System;

namespace GridDomain.Common
{
    public class DefaultDateTimeStrategy : DateTimeStrategy
    {
        public override DateTime Now => DateTime.Now;
        public override DateTime UtcNow => DateTime.UtcNow;
    }
}
