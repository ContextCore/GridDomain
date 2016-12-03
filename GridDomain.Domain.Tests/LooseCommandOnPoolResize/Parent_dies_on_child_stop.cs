using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Event;
using Akka.TestKit.NUnit3;
using GridDomain.Common;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.LooseCommandOnPoolResize
{
    [TestFixture]
    class Parent_dies_on_child_stop: TestKit
    {
        class Parent : ReceiveActor
        {
            private ILoggingAdapter log;
            private Dictionary<Guid,IActorRef> children = new Dictionary<Guid, IActorRef>();
            public Parent()
            {
                log = Context.GetLogger();

                log.Info("parent created");
                Receive<Spawn>(s =>
                {
                    var child = Context.System.ActorOf<Child>();
                    Context.Watch(child);

                    children.Add(s.Id, child);
                });
                Receive<Stop>(s =>
                {
                    children[s.Id].Tell(s);
                });
                Receive<Terminated>(t =>
                {
                    log.Info("Child terminated at " + t.ActorRef.Path);
                });
            }

            protected override void PostStop()
            {
                log.Info("parent stopped");
                base.PostStop();
            }

            // if any child, e.g. the logger above. throws an exception
            // apply the rules below
            // e.g. Restart the child, if 10 exceptions occur in 30 seconds or
            // less, then stop the actor
            protected override SupervisorStrategy SupervisorStrategy()
            {
                return new OneForOneStrategy(
                                5,
                                TimeSpan.FromSeconds(30),
                                x =>
                                {
                                    if (x is NotSupportedException) return Directive.Stop;
                                    return Directive.Restart;
                                });
            }

        }

        class Child : ReceiveActor
        {
            private ILoggingAdapter _log;

            public Child()
            {
                _log = Context.GetLogger();

                _log.Info("child created");
                Receive<Stop>(s =>
                {
                    _log.Info("child is about to stop");

                    Context.System.Stop(Self);
                });
            }

            protected override void PostStop()
            {
                _log.Info("child stopped");
                base.PostStop();
            }
        }

        public Parent_dies_on_child_stop():base(@"akka {  
    loggers = [""Akka.Event.StandardOutLogger, Akka""]
    #stdout-loglevel = DEBUG
    #loglevel = DEBUG
    log-config-on-start = on        
    actor {                
        debug {  
              receive = on 
              autoreceive = on
              lifecycle = on
              event-stream = on
              unhandled = on
        }
    }")
        {
            
        }
        [Test]
        public void Start()
        {
            var parent = Sys.ActorOf<Parent>("Parent");

            var id = Guid.NewGuid();

            parent.Tell(new Spawn(id));
            parent.Tell(new Stop(id));


            Watch(parent);

            var t = ExpectMsg<Terminated>();

            Console.WriteLine("Closing system");
        }
    }


    [TestFixture]
    class Aggregate_Hub_dies_after_clear_children : SampleDomainCommandExecutionTests
    {
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => c.Register(base.CreateConfiguration()),
                                                    c => c.RegisterInstance<IPersistentChildsRecycleConfiguration>(
                                                        new PersistentChildsRecycleConfiguration(TimeSpan.FromSeconds(5),
                                                        TimeSpan.FromMilliseconds(50))));
        }

        [Test]
        public void Start()
        {
            var cmd = new CreateSampleAggregateCommand(1, Guid.NewGuid());
            GridNode.NewCommandWaiter()
                    .Expect<SampleAggregateCreatedEvent>()
                    .Create()
                    .Execute(cmd);

            var aggregate = LookupAggregateActor<SampleAggregate>(cmd.AggregateId);
            Watch(aggregate);

            ExpectMsg<Terminated>(t => t.ActorRef.Path == aggregate.Path, Timeout);
            ExpectNoMsg(TimeSpan.FromSeconds(5));

            //hubs does not close after child terminates
            char pooledLetter = 'a';
            for (int n = 0; n < Environment.ProcessorCount; n++)
            {
                var pooledHub = LookupAggregateHubActor<SampleAggregate>("$" + pooledLetter++);
                pooledHub.Ask<HealthStatus>(new CheckHealth("123")).Wait();
            }
        }

        protected override TimeSpan Timeout { get; } = TimeSpan.FromSeconds(30);
    }

}