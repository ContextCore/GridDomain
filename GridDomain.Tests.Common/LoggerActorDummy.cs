using Akka.Actor;
using Akka.Event;

namespace GridDomain.Tests.Common
{
    public class LoggerActorDummy : ReceiveActor
    {
        public LoggerActorDummy()
        {
            Receive<Error>(m => { });
            Receive<Warning>(m => { });
            Receive<Info>(m => { });
            Receive<Debug>(m => { });
            Receive<InitializeLogger>(m =>
                                      {
                                          Context.GetLogger().Info("logger sub initialized");
                                          Sender.Tell(new LoggerInitialized());
                                      });
        }
    }
}