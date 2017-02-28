using System;
using GridDomain.Common;

namespace GridDomain.Tests.Framework
{
    public class TestDateTimeStrategy : ICurrentDateTimeStrategy
    {
        public DateTime EditableNow { get; set; }
        public DateTime EditableUtcNow { get; set; }

        public DateTime Now => EditableNow;
        public DateTime UtcNow => EditableUtcNow;
    }
}