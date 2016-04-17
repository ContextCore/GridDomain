using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit.NUnit;
using GridDomain.Node.Configuration;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
    public class ConsistentHashingRoutingTest:TestKit
    {
        private ActorSystem _system;
        private DiagMsg[] _diagMsgs;
        private DiagMsg[] _results;

        class DiagMsg
        {
            public long TicksStartProcess;
            public long TicksEndProcess;
            public long Thread;
            public long ActorHashCode;
            public long HashKey;
        }

        class TimeLoggerActor : TypedActor
        {
            public static Stopwatch watch = new Stopwatch();
            public TimeLoggerActor(IActorRef sender)
            {
                watch.Start();
            }

            
            public void Handle(DiagMsg m)
            {
                //m.TicksStartProcess = watch.ElapsedTicks;
                //m.ActorHashCode = this.GetHashCode();
                //m.Thread = Thread.CurrentThread.ManagedThreadId;
                //Thread.Sleep(100);
                //m.TicksEndProcess = watch.ElapsedMilliseconds;
                //Sender.Tell(m);
                Console.WriteLine( m.TicksStartProcess = watch.ElapsedTicks);
                m.ActorHashCode = this.GetHashCode();
                m.Thread = Thread.CurrentThread.ManagedThreadId;
                Thread.Sleep(100);
                m.TicksEndProcess = watch.ElapsedTicks;

                Console.WriteLine($"msg {m.GetHashCode()} correlation {m.HashKey} handled in thread {m.Thread} from {m.TicksStartProcess} ticks to {m.TicksEndProcess}");
                Sender.Tell(m);
            }
        }

        [SetUp]
        public void TestsRouting()
        {
            _system = ActorSystemFactory.CreateActorSystem(new AkkaConfiguration("hashing", 8100, "127.0.0.1",
                AkkaConfiguration.LogVerbosity.Trace));

            var actor = _system.ActorOf(Props.Create<TimeLoggerActor>(TestActor)
                                      .WithRouter(new ConsistentHashingPool(2).WithHashMapping(m => ((DiagMsg)m).HashKey)));

            _diagMsgs = CreateMessages();

            foreach (var msg in _diagMsgs)
                actor.Tell(msg);

            _results = _diagMsgs.Select(m => ExpectMsg<DiagMsg>()).ToArray();
        }

        private DiagMsg[] CreateMessages()
        {
            return new[]
            {
                new DiagMsg() {HashKey = 1},
                new DiagMsg() {HashKey = 2},
                new DiagMsg() {HashKey = 1},
                new DiagMsg() {HashKey = 1},
                new DiagMsg() {HashKey = 3},
                new DiagMsg() {HashKey = 2},
                new DiagMsg() {HashKey = 2},
                new DiagMsg() {HashKey = 1},
                new DiagMsg() {HashKey = 2},
                new DiagMsg() {HashKey = 3},
                new DiagMsg() {HashKey = 3},
                new DiagMsg() {HashKey = 3},
                new DiagMsg() {HashKey = 2},
                new DiagMsg() {HashKey = 3},
            };
        }

        [Test]
        public void All_messages_was_processed_in_linear_sequence()
        {

            foreach (var group in _results.GroupBy(r => r.HashKey))
            {
                var enumerator = group.GetEnumerator();
                enumerator.MoveNext();
                var prev = enumerator.Current as DiagMsg;

                while (enumerator.MoveNext())
                {
                    var current = (DiagMsg) enumerator.Current;
                    if (prev != null)
                        Assert.True(prev.TicksEndProcess < current.TicksStartProcess);
                }
            }
        }
    }
}