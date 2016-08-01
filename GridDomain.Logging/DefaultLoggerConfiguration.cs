using System.Configuration;
using Serilog;

namespace GridDomain.Logging
{
    public class DefaultLoggerConfiguration : LoggerConfiguration
    {
        public DefaultLoggerConfiguration()
        {
            var filePath = ConfigurationManager.AppSettings["logFilePath"] ?? @"C:\Logs";
            var elasticEndpoint = ConfigurationManager.AppSettings["logElasticEndpoint"] ?? "http://soloinfra.cloudapp.net:9222";
            var withMachineName = WriteTo.RollingFile(filePath + "\\logs-{Date}.txt")
                //.WriteTo.Slack("https://hooks.slack.com/services/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")
                .WriteTo.Elasticsearch(elasticEndpoint)
                .WriteTo.Console()
                .Enrich
                .WithMachineName();

            foreach (var type in TypesForScalarDescruptionHolder.Types)
            {
                withMachineName = withMachineName.Destructure.AsScalar(type);
            }
        }
    }
}