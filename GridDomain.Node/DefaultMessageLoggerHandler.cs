using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using Serilog;
using Serilog.Events;

namespace GridDomain.Node
{
    public class DefaultMessageLoggerHandler : IHandler<DomainEvent>,
                                               IHandler<ICommand>,
                                               IHandler<ICommandFault>
    {
        private static readonly ILogger _log  = new LoggerConfiguration().WriteTo
                                            .RollingFile(".\\GridDomainLogs\\TransportMessages\\logs-{Date}.txt",LogEventLevel.Verbose)
                                            .CreateLogger();
        private void Handle(object msg)
        {
            _log.Information("got message from transpot: {@msg}",msg);
        }

        public void Handle(DomainEvent msg)
        {
            Handle((object) msg);
        }

        public void Handle(ICommand msg)
        {
            Handle((object)msg);
        }

        public void Handle(ICommandFault msg)
        {
            Handle((object)msg);
        }
    }
}