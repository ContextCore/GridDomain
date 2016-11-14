using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Policy;
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
        [HandleProcessCorruptedStateExceptions]
        public static void Main(params string[] args)
        {
            var unityContainer = new UnityContainer();
            unityContainer.Register(new SampleDomainContainerConfiguration());

            var cfg = new CustomContainerConfiguration(
                c => c.Register(new SampleDomainContainerConfiguration()),
                c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
              //  c => c.RegisterType<IQuartzConfig, PersistedQuartzConfig>());
                c => c.RegisterType<IQuartzConfig, InMemoryQuartzConfig>());

          //  Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateSystem()};
            Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateInMemorySystem()};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer), actorSystemFactory);

            node.Start(new LocalDbConfiguration());

            var timer = new Stopwatch();
            timer.Start();

            var totalAggregatePacksCount = 50000;
            var aggregatePackSize = 100;
            int timeoutedCommads = 0;
            var random = new Random();
            int aggregateChangeAmount = 2;
            var commandsInPack = aggregatePackSize * (aggregateChangeAmount + 1);

            for (int i = 0; i < totalAggregatePacksCount; i += aggregatePackSize)
            {
                var packTimer = new Stopwatch();
                packTimer.Start();
                var tasks = Enumerable.Range(0, aggregatePackSize)
                                      .Select(t => WaitAggregateCommands(aggregateChangeAmount, random, node))
                                      .ToArray();
                try
                {
                    Task.WhenAll(tasks).Wait();
                }
                catch
                {
                    timeoutedCommads += tasks.Count(t => t.IsCanceled || t.IsFaulted);
                }

                packTimer.Stop();
                var speed = (decimal) (commandsInPack / packTimer.Elapsed.TotalSeconds); 
                //Console.WriteLine($"Executed {aggregatePackSize} commands in {packTimer.Elapsed}, Total: {i} in {timer.Elapsed}, " +
                Console.WriteLine($"speed :{speed} cmd/sec," +
                                  $"total errors: {timeoutedCommads}, " +
                                  $"total commands executed: {i * commandsInPack}," +
                                  $"approx time remaining: {(totalAggregatePacksCount - i) / speed }");
            }

            timer.Stop();
            Console.WriteLine($"Executed {totalAggregatePacksCount} batches in {timer.Elapsed}");
            node.Stop();

            Console.WriteLine("Sleeping");
            Thread.Sleep(60);

        }

        private static Task<IWaitResults> WaitAggregateCommands(int changeNumber, Random random, GridDomainNode node)
        {
            var commands = new List<ICommand>(changeNumber + 1);
            var createCmd = new CreateSampleAggregateCommand(random.Next(), Guid.NewGuid());

            var changeCmds = Enumerable.Range(0, changeNumber)
                                       .Select(n => new ChangeSampleAggregateCommand(random.Next(), createCmd.AggregateId))
                                       .ToArray();

            commands.Add(createCmd);
            commands.AddRange(changeCmds);


            var expectBuilder = node.NewCommandWaiter()
                                    .Expect<SampleAggregateCreatedEvent>(e => e.SourceId == createCmd.AggregateId);

            foreach (var cmd in changeCmds)
                expectBuilder.And<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId && e.Value == cmd.Parameter.ToString());


            return expectBuilder.Create()
                                .Execute(commands.ToArray());
        }
    }
}
