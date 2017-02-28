namespace GridDomain.Common
{
    internal static class DateTimeStrategyHolder
    {
        internal static ICurrentDateTimeStrategy Current { get; set; }
        static DateTimeStrategyHolder()
        {
            Current = new DefaultDateTimeStrategy();
        }
    }
}