namespace GridDomain.Common
{
    public static class DateTimeStrategyHolder
    {
        public static DateTimeStrategy Current { get; set; }
        static DateTimeStrategyHolder()
        {
            Current = new DefaultDateTimeStrategy();
        }
    }
}