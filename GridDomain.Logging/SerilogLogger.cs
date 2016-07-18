using System;
using Serilog;

namespace GridDomain.Logging
{
    public class SerilogLogger : ISoloLogger
    {
        private readonly ILogger _log;
        
      
        public SerilogLogger(ILogger log)
        {
            _log = log;
        }

        public ISoloLogger ForContext(string name, object value)
        {
            _log.ForContext(name, value);
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

        private Serilog.ILogger GetLoggerWithExceptionContext(Exception ex)
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

        private Serilog.ILogger ContextLogger
        {
            get
            {
                return _log;
                
            }
        }
    }
}