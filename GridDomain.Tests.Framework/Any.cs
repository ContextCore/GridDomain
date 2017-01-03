using System;

namespace GridDomain.Tests.Framework
{
    public static class Any
    {
        private static readonly Guid AnyGuid = Guid.Parse("0A34A504-4DCF-4B68-A465-05B7F54FD009");

        public static Guid GUID { get; } = AnyGuid;
    }
}