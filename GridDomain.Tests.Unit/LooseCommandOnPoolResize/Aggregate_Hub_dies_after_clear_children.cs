using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.LooseCommandOnPoolResize
{
    [TestFixture]
    [Ignore("not actual")]

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
        public async Task Start()
        {
            var cmd = new CreateSampleAggregateCommand(1, Guid.NewGuid());
            await GridNode.Prepare(cmd)
                          .Expect<SampleAggregateCreatedEvent>()
                          .Execute();

            var aggregate = LookupAggregateActor<SampleAggregate>(cmd.AggregateId);
            Watch(aggregate);

            ExpectMsg<Terminated>(t => t.ActorRef.Path == aggregate.Path, Timeout);
            ExpectNoMsg(TimeSpan.FromSeconds(5));

            //wait until next clear childs message
            Thread.Sleep(6);

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