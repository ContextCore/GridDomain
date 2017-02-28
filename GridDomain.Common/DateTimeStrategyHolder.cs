namespace GridDomain.Common
{
    internal static class DateTimeStrategyHolder
    {
        static DateTimeStrategyHolder()
        {
            Current = new DefaultDateTimeStrategy();
        }

        internal static ICurrentDateTimeStrategy Current { get; set; }
    }
}