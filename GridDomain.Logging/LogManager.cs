namespace GridDomain.Logging
{
    public static class LogManager
    {
        private static LoggerFactory _loggerFactory = new DefaultLoggerFactory();
        public static void SetLoggerFactory(LoggerFactory loggerFactory)
        {
           // var a = typeof(Elasticsearch.Net.ElasticsearchDynamicValue);
            _loggerFactory = loggerFactory;
        }

        public static ISoloLogger GetLogger(LoggerFactory factory = null)
        {
            return factory != null ? factory.GetLogger() : _loggerFactory.GetLogger();
        }
    }
}
