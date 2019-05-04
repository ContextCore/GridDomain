using System;

namespace GridDomain.Common
{
    internal interface ICurrentDateTimeStrategy
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }
}