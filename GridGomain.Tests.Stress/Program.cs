using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
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
                c => c.RegisterType<IQuartzConfig, PersistedQuartzConfig>());

            Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateSystem()};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer), actorSystemFactory);

            node.Start(new LocalDbConfiguration());

            var timer = new Stopwatch();
            timer.Start();

            var count = 500000;
            var step = 100;


            for (int i = 0; i < count; i += step)
            {
                var packTimer = new Stopwatch();
                packTimer.Start();
                var tasks = Enumerable.Range(0, step).Select(t =>
                {
                    var data = new Fixture();
                    var createAggregateCommand = data.Create<CreateSampleAggregateCommand>();
                    var changeAggregateCommandA = new ChangeSampleAggregateCommand(data.Create<int>(),
                        createAggregateCommand.AggregateId);
                    var changeAggregateCommandB = new ChangeSampleAggregateCommand(data.Create<int>(),
                        createAggregateCommand.AggregateId);
                    var changeAggregateCommandC = new ChangeSampleAggregateCommand(data.Create<int>(),
                        createAggregateCommand.AggregateId);

                    return node.NewCommandWaiter(TimeSpan.FromSeconds(100))
                               .Expect<SampleAggregateCreatedEvent>(e => e.SourceId == createAggregateCommand.AggregateId)
                               .And<SampleAggregateCreatedEvent>   (e => e.SourceId == changeAggregateCommandA.AggregateId)
                               .And<SampleAggregateCreatedEvent>   (e => e.SourceId == changeAggregateCommandB.AggregateId)
                               .And<SampleAggregateCreatedEvent>   (e => e.SourceId == changeAggregateCommandC.AggregateId)
                               .Create()
                               .Execute(createAggregateCommand);

                }).ToArray();

                Task.WaitAll(tasks);
                packTimer.Stop();
                Console.WriteLine($"Executed {step} commands in {packTimer.Elapsed}, Total: {i} in {timer.Elapsed}");
            }

            timer.Stop();
            Console.WriteLine($"Executed {count} batches in {timer.Elapsed}");

            Console.WriteLine("Sleeping");
            Thread.Sleep(60);

            node.Stop();
        }
    }
}
