using System;

namespace GridDomain.Node.Configuration
{
    class LogConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akkaConf;
        private bool _includeConfig;
        public LogConfig(AkkaConfiguration akkaConf, bool includeConfig = true)
        {
            _akkaConf = akkaConf;
            _includeConfig = includeConfig;
        }

        public string Build()
        {
            string logConfig =
                @"
      stdout-loglevel = " + _akkaConf.LogLevel + @"
      loglevel = " + _akkaConf.LogLevel;

            if(_includeConfig)
                logConfig  += @"
      log-config-on-start = on";

            return logConfig;
        }
    }
}