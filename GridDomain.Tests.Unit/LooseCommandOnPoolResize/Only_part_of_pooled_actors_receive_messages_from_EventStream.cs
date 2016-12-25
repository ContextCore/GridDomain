using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit.NUnit3;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.LooseCommandOnPoolResize
{
    [TestFixture]
    [Ignore("not actual")]
    class Only_part_of_pooled_actors_receive_messages_from_EventStream : TestKit
    {
        class Worker : ReceiveActor
        {
            public Worker()
            {
                Receive<DoWorkMessage>(m =>
                    Context.System.EventStream.Publish(new WorkDoneMessage(m.Id, Self.Path))
                );
            }
        }

        class DoWorkMessage
        {

            public DoWorkMessage(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        class WorkDoneMessage
        {
            public WorkDoneMessage(Guid id, ActorPath executor)
            {
                Id = id;
                Executor = executor;
            }

            public Guid Id { get; }
            public ActorPath Executor { get;}
        }

        [Test]
        public void Publish_to_stream()
        {
            var pooledWorkers = Sys.ActorOf(Props.Create<Worker>()
                .WithRouter(
                    new ConsistentHashingPool(4, m => (m as DoWorkMessage)?.Id)), "PooledWorker");

            Sys.EventStream.Subscribe(pooledWorkers, typeof(DoWorkMessage));

            var inbox = Inbox.Create(Sys);

            Sys.EventStream.Subscribe(inbox.Receiver, typeof(WorkDoneMessage));
            int totalCount = 10;

            while (--totalCount > 0)
            {
                foreach (var m in Enumerable.Range(0, 100).Select(i => new DoWorkMessage(Guid.NewGuid())))
                {
                    Sys.EventStream.Publish(m);
                    var msg = (WorkDoneMessage) inbox.Receive();
                    Assert.AreEqual(m.Id, msg.Id);
                    Console.WriteLine($"work {m.Id} done by {msg.Executor}");
                }

                Thread.Sleep(TimeSpan.FromSeconds(30));
            }

            Console.WriteLine("received all messages");
        }
    }
}
