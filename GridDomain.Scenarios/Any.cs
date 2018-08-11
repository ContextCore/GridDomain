using System;

namespace GridDomain.Tests.Scenarios
{
    public static class Any
    {
        private static readonly Guid AnyGuid = Guid.Parse("0A34A504-4DCF-4B68-A465-05B7F54FD009");
        private static readonly DateTime AnyDateTime = DateTime.Parse("01/01/1900");

        public static Guid GUID { get; } = AnyGuid;
        public static DateTime DateTime { get; } = AnyDateTime;
    }
}