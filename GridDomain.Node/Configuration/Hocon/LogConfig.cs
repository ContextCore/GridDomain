namespace GridDomain.Node.Configuration.Hocon
{
    internal class LogConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akkaConf;
        private readonly bool _includeConfig;

        public LogConfig(AkkaConfiguration akkaConf, bool includeConfig = true)
        {
            _akkaConf = akkaConf;
            _includeConfig = includeConfig;
        }

        public string Build()
        {
            var logConfig =
                @"
      stdout-loglevel = " + _akkaConf.LogLevel + @"
      loglevel = " + _akkaConf.LogLevel;

            if (_includeConfig)
                logConfig += @"
      log-config-on-start = on";

            return logConfig;
        }
    }
}