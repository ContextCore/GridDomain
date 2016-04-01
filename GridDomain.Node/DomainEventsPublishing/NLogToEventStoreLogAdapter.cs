using NEventStore.Logging;
using NLog;

namespace GridDomain.Node.DomainEventsPublishing
{
    public class NLogToEventStoreLogAdapter: ILog
    {
        private readonly Logger _log;

        public NLogToEventStoreLogAdapter(Logger log)
        {
            _log = log;
        }

        public void Verbose(string message, params object[] values)
        {
            _log.Trace(string.Format(message,values));
        }

        public void Debug(string message, params object[] values)
        {
            _log.Debug(string.Format(message, values));
        }

        public void Info(string message, params object[] values)
        {
            _log.Info(string.Format(message, values));
        }

        public void Warn(string message, params object[] values)
        {
            _log.Warn(string.Format(message, values));
        }

        public void Error(string message, params object[] values)
        {
            _log.Error(string.Format(message, values));
        }

        public void Fatal(string message, params object[] values)
        {
            _log.Fatal(string.Format(message, values));
        }
    }
}