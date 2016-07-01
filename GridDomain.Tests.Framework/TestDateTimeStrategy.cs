using System;
using GridDomain.Common;

namespace GridDomain.Tests.Framework
{
    public class TestDateTimeStrategy : DateTimeStrategy
    {
        public DateTime EditableNow { get; set; }
        public DateTime EditableUtcNow { get; set; }

        public override DateTime Now => EditableNow;
        public override DateTime UtcNow => EditableUtcNow;
    }
}