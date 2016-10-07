using System;
using Akka.Actor;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class MessageNotifierActor<T> : ReceiveActor
    {
        private IActorRef _listener;

        public MessageNotifierActor(Predicate<T> filter, TimeSpan timeout)
        {
            Context.System.Scheduler.ScheduleTellOnce(timeout,Self,Timeout.Instance,Self);
            Receive<T>(t =>
            {
                _listener.Tell(t);
                Context.Stop(Self);
            }, filter);

            Receive<Timeout>(m =>
            {
                _listener.Tell(new Status.Failure(new TimeoutException()));
                Context.Stop(Self);
            });

            Receive<Wait>(m =>
            {
                _listener = Sender;
            });
        }

        class Timeout
        {
            public static Timeout Instance => new Timeout();
        }
        internal class Wait
        {
            public static Wait Instance => new Wait();
        }
    }
}