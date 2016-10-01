namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class LogConfig : IAkkaConfig
    {
        private readonly bool _includeConfig;
        private readonly string _logLevel;

        public LogConfig(string logLevel, bool includeConfig = true)
        {
            _includeConfig = includeConfig;
            _logLevel = logLevel;
        }

        public string Build()
        {
           
            var logConfig =
                @"
                stdout-loglevel = " + _logLevel + @"
                loglevel=" + _logLevel;

            logConfig += @"
                loggers=["""+typeof(SerilogExtendedLogger).AssemblyQualifiedShortName() + @"""]

                actor.debug {"+
#if DEBUG
                    @"receive = on
                      autoreceive = on
                      lifecycle = on
                      event-stream = on "
#endif
                            +@"
                      unhandled = on
                }";

            if (_includeConfig)
                    logConfig += @"
                log-config-on-start = on";

            return logConfig;
        }
    }
}