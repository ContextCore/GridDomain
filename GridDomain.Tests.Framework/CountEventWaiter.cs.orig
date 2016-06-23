using Akka.Actor;

namespace GridDomain.Tests.Framework
{
    public class CountEventWaiter<T> : ReceiveActor
    {
        private int _count;

        public CountEventWaiter(int count, IActorRef notifyActor)
        {
            _count = count;
            Receive<T>(
                msg =>
                {
                    if (-- _count > 0) return;
                    notifyActor.Tell(new ExpectedMessagesRecieved<T>(msg));
                });
        }
    }
}