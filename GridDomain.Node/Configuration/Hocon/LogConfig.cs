using System;

namespace GridDomain.Node.Configuration
{
    class LogConfig : IAkkaConfig
    {
        private readonly AkkaConfiguration _akkaConf;

        public LogConfig(AkkaConfiguration akkaConf)
        {
            _akkaConf = akkaConf;
        }

        public string Build()
        {
            return BuildLogConfig(_akkaConf);
        }

        public static string BuildLogConfig(AkkaConfiguration akkaConf)
        {
            string logConfig =
                @"
      stdout-loglevel = " + akkaConf.LogLevel + @"
      loglevel = " + akkaConf.LogLevel + @"
      log-config-on-start = on";
            return logConfig;
        }
    }
}