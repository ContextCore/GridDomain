using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using Microsoft.Practices.Unity;
using Ploeh.AutoFixture;

namespace GridGomain.Tests.Stress
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            var unityContainer = new UnityContainer();
            unityContainer.Register(new SampleDomainContainerConfiguration());

            var cfg = new CustomContainerConfiguration(
                c => c.Register(new SampleDomainContainerConfiguration()),
                c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
                c => c.RegisterType<IQuartzConfig,PersistedQuartzConfig>());

            Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateSystem()};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer),actorSystemFactory);

            node.Start(new LocalDbConfiguration());

            var timer = new Stopwatch();
            timer.Start();

            var count = 1000;
            var tasks = Enumerable.Range(0,count).Select(t =>
            {
                var data = new Fixture();
                var createAggregateCommand = data.Create<CreateSampleAggregateCommand>();
                var changeAggregateCommandA = new ChangeSampleAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);
                var changeAggregateCommandB = new ChangeSampleAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);
                var changeAggregateCommandC = new ChangeSampleAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);

                var createExpect  = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, createAggregateCommand.AggregateId);
                var changeAExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, changeAggregateCommandA.AggregateId);
                var changeBExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, changeAggregateCommandB.AggregateId);
                var changeCExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, changeAggregateCommandC.AggregateId);

                // A, B+C in parallel, C
                var executionPlan = CommandExecutorExtensions.Execute(node, createAggregateCommand, createExpect)
                    .ContinueWith(t1 => node.Execute(changeAggregateCommandA, changeAExpect))
                        .ContinueWith(t2 => node.Execute(changeAggregateCommandB, changeBExpect))
                            .ContinueWith(t3 => node.Execute(changeAggregateCommandC, changeCExpect));

                return executionPlan;

            }).ToArray();

            Task.WaitAll(tasks);


            timer.Stop();
            Console.WriteLine($"Executed {count} batches in {timer.Elapsed}");

            Console.WriteLine("Sleeping");
            Thread.Sleep(60);

            node.Stop();
        }
    }
}
