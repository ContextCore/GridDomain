using System;
using System.Diagnostics;
using Serilog;

namespace GridDomain.Logging
{
    public class SerilogLogger : ISoloLogger
    {
        private static readonly ILogger Log;

        static SerilogLogger()
        {
            Log =  new LoggerConfiguration().WriteTo.RollingFile("C:\\Logs\\logs-{Date}.txt")
                                            //.WriteTo.Slack("https://hooks.slack.com/services/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")
                                           // .WriteTo.Elasticsearch("http://soloinfra.cloudapp.net:9222")
                                            .CreateLogger();
        }

        public ISoloLogger ForContext(string name, object value)
        {
            Log.ForContext(name, value);
            return this;
        }

        public void Trace(string message)
        {
            ContextLogger.Verbose(message);
        }

        public void Debug(string message, params object[] parameters)
        {
            ContextLogger.Debug(message, parameters);
        }

        public void Info(string message, params object[] parameters)
        {
            ContextLogger.Information(message, parameters);
        }

        public void Error(Exception ex, string message = null, params object[] parameters)
        {
            var log = GetLoggerWithExceptionContext(ex);
            log.Error(ex, message ?? ex.Message, parameters);
        }

        private ILogger GetLoggerWithExceptionContext(Exception ex)
        {
            var log = ContextLogger;
            if (ex != null)
            {
                log = log.ForContext("ExceptionType", ex.GetType().FullName);
                if (ex.Data.Count > 0)
                {
                    foreach (var key in ex.Data.Keys)
                    {
                        log = log.ForContext(key.ToString(), ex.Data[key]);
                    }
                }
            }
            return log;
        }

        public void Warn(Exception ex, string message, params object[] parameters)
        {
            ContextLogger.Warning(ex, message, parameters);
        }

        public void Warn(string message, params object[] parameters)
        {
            ContextLogger.Warning(message, parameters);
        }

        private ILogger ContextLogger
        {
            get
            {
                return Log;
                //var logger = Log
                //.ForContext(TcsLoggingContextKeys.Application, RequestLogInfoManager.ApplicationName)
                //.ForContext(TcsLoggingContextKeys.Machine, Environment.MachineName)
                //.ForContext(TcsLoggingContextKeys.Environment, EnvironmentProvider.Environment)
                //.ForContext(TcsLoggingContextKeys.LoggerName, Name);

                //if (RequestLogInfoManager.Elapsed > -1)
                //{
                //    logger = logger.ForContext(TcsLoggingContextKeys.Elapsed, RequestLogInfoManager.Elapsed);
                //}

                //foreach (var name in TcsMappedDiagnosticsLogicalContext.GetKeys())
                //{
                //    var value = TcsMappedDiagnosticsLogicalContext.Get(name);
                //    if (ParameterShouldBeLogged(value))
                //    {
                //        logger = logger.ForContext(name, value, true);
                //    }
                //}
                //return logger;
            }
        }

        private bool ParameterShouldBeLogged(object parameter)
        {
            if (parameter == null)
            {
                return false;
            }
            if (parameter.GetType() == typeof(Stopwatch))
            {
                return false;
            }
            return true;
        }
    }
}