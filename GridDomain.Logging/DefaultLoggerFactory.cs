using System;
using System.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace GridDomain.Logging
{
    public class DefaultLoggerFactory : LoggerFactory
    {
        private static readonly Lazy<SerilogLogger> LoggerFactory = new Lazy<SerilogLogger>(() => new SerilogLogger(GetConfiguration().CreateLogger()));

        private static LoggerConfiguration GetConfiguration()
        {
            var filePath = ConfigurationManager.AppSettings["logFilePath"] ?? @"C:\Logs";
            var machineName = ConfigurationManager.AppSettings["envName"] ?? Environment.MachineName;
            var elasticEndpoint = ConfigurationManager.AppSettings["logElasticEndpoint"] ?? "http://soloinfra.cloudapp.net:9222";
            var elasticOptions = new ElasticsearchSinkOptions(new Uri(elasticEndpoint));
            var consoleLevel = ConfigurationManager.AppSettings["logConsoleLevel"] ?? "Verbose";
            var level = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), consoleLevel);
            elasticOptions.MinimumLogEventLevel = level;
            var configuration = new LoggerConfiguration();
            var instrumentationKey = ConfigurationManager.AppSettings["instrumentationKey"];
            configuration = configuration.WriteTo.RollingFile(filePath + "\\logs-{Date}.txt")
                .WriteTo.Elasticsearch(elasticOptions)
                .WriteTo.ApplicationInsightsTraces(instrumentationKey, level)
                .WriteTo.Console(level)
                .Enrich.WithProperty("MachineName", machineName);

            foreach (var type in TypesForScalarDestructionHolder.Types)
            {
                configuration = configuration.Destructure.AsScalar(type);
            }
            return configuration;
        }

        public override ISoloLogger GetLogger(string className = null)
        {
            className = className ?? GetClassName() ;
            return LoggerFactory.Value.ForContext("className", className);

        }
    }
}