using Serilog;

namespace GridDomain.Node
{
    class DebugLogConfig : LoggerConfiguration
    {
        public DebugLogConfig()
        {
            WriteTo.RollingFile(".\\GridDomainLogs\\logs-{Date}.txt").
                //.WriteTo.Slack("https://hooks.slack.com/services/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")
            WriteTo.Elasticsearch("http://soloinfra.cloudapp.net:9222");
        }
    }
}