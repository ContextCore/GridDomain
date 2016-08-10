using System;
using System.Configuration;
using Serilog;
using Serilog.Events;

namespace GridDomain.Logging
{
    public class GridDomainInternalLoggerConfiguration : LoggerConfiguration
    {
        public GridDomainInternalLoggerConfiguration()
        {
            var elasticEndpoint = ConfigurationManager.AppSettings["logElasticEndpoint"] ?? "http://soloinfra.cloudapp.net:9222";
            var configuration = WriteTo.RollingFile(".\\GridDomainLogs\\logs-{Date}.txt").
                WriteTo.Elasticsearch(elasticEndpoint).
                WriteTo.Console(LogEventLevel.Error)
                .Enrich.WithProperty("MachineName", Environment.MachineName);
            foreach (var type in TypesForScalarDestructionHolder.Types)
            {
                configuration = configuration.Destructure.AsScalar(type);
            }
        }
    }
} 