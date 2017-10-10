using Akka.Actor;
using Akka.Event;
using GridDomain.Node;

namespace GridDomain.Tests.Unit.Logging {
    class TestLogActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Context.GetSeriLogger();

        public TestLogActor()
        {
            _log.Debug("actor debug");
            _log.Info("actor info");
            _log.Error("actor error");
            _log.Warning("actor warn");

            _log.Info("Debug enabled: {status}",  _log.IsDebugEnabled);
            _log.Info("Error enabled: {status}",  _log.IsErrorEnabled);
            _log.Info("Info enabled: {status}",   _log.IsInfoEnabled);
            _log.Info("Warning enabled: {status}",_log.IsWarningEnabled);

            ReceiveAny(o =>
                       {
                           _log.Debug("Debug: {@received}", o);
                           _log.Info("Info: {@received}", o);
                           _log.Error("Error: {@received}", o);
                           _log.Warning("Warning: {@received}", o);
                           Sender.Tell(o);
                       });
        }
    }
}