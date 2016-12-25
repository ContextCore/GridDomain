using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.Framework.Configuration;
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
            RawCommandExecution(1, 1000, 20);
            Console.WriteLine("Sleeping");
            Thread.Sleep(60);
        }

        private static void RawCommandExecution(int totalAggregateScenariosCount, int aggregateScenarioPackSize, int aggregateChangeAmount)
        {
            var dbCfg = new AutoTestAkkaConfiguration();

            using (var connection = new SqlConnection(dbCfg.Persistence.JournalConnectionString))
            {
                connection.Open();
                var sqlText = @"TRUNCATE TABLE Journal";
                var cmdJournal = new SqlCommand(sqlText, connection);
                cmdJournal.ExecuteNonQuery();

                var sqlText1 = @"TRUNCATE TABLE Snapshots";
                var cmdSnapshots = new SqlCommand(sqlText, connection);
                cmdSnapshots.ExecuteNonQuery();
            }

            var unityContainer = new UnityContainer();
            unityContainer.Register(new SampleDomainContainerConfiguration());

            var cfg = new CustomContainerConfiguration(
                c => c.Register(new SampleDomainContainerConfiguration()),
                c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
                c => c.RegisterType<IQuartzConfig, PersistedQuartzConfig>());

            Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateSystem()};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer), actorSystemFactory);

            node.Start().Wait();

            var timer = new Stopwatch();
            timer.Start();

            int timeoutedCommads = 0;
            var random = new Random();
            var commandsInScenario = aggregateScenarioPackSize*(aggregateChangeAmount + 1);
            var totalCommandsToIssue = commandsInScenario*totalAggregateScenariosCount;


            for (int i = 0; i < totalAggregateScenariosCount; i ++)
            {
                var packTimer = new Stopwatch();
                packTimer.Start();
                var tasks = Enumerable.Range(0, aggregateScenarioPackSize)
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
                var speed = (decimal) (commandsInScenario/packTimer.Elapsed.TotalSeconds);
                var timeLeft = TimeSpan.FromSeconds((double) ((totalCommandsToIssue - i*commandsInScenario)/speed));

                Console.WriteLine($"speed :{speed} cmd/sec," +
                                  $"total errors: {timeoutedCommads}, " +
                                  $"total commands executed: {i*commandsInScenario}/{totalCommandsToIssue}," +
                                  $"approx time remaining: {timeLeft}");
            }


            timer.Stop();
            node.Stop().Wait();

            var speedTotal = (decimal) (totalCommandsToIssue/timer.Elapsed.TotalSeconds);
            Console.WriteLine(
                $"Executed {totalAggregateScenariosCount} batches = {totalCommandsToIssue} commands in {timer.Elapsed}");
            Console.WriteLine($"Average speed was {speedTotal} cmd/sec");

            using (var connection = new SqlConnection(dbCfg.Persistence.JournalConnectionString))
            {
                connection.Open();
                var sqlText = @"SELECT COUNT(*) FROM Journal";
                var cmdJournal = new SqlCommand(sqlText, connection);
                var count = (int) cmdJournal.ExecuteScalar();

                Console.WriteLine(count == totalCommandsToIssue
                    ? "Journal contains all events"
                    : $"Journal contains only {count} of {totalCommandsToIssue}");
            }
        }

        private static GridDomainNode StartSampleDomainNode()
        {
            var unityContainer = new UnityContainer();
            unityContainer.Register(new SampleDomainContainerConfiguration());

            var cfg = new CustomContainerConfiguration(
                c => c.Register(new SampleDomainContainerConfiguration()),
                c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
                c => c.RegisterType<IQuartzConfig, PersistedQuartzConfig>());

            Func<ActorSystem[]> actorSystemFactory = () => new[] {new StressTestAkkaConfiguration().CreateSystem()};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer), actorSystemFactory);

            node.Start().Wait();
            return node;
        }

        private static AutoTestAkkaConfiguration ClearWriteDb()
        {
            var dbCfg = new AutoTestAkkaConfiguration();

            using (var connection = new SqlConnection(dbCfg.Persistence.JournalConnectionString))
            {
                connection.Open();
                var sqlText = @"TRUNCATE TABLE Journal";
                var cmdJournal = new SqlCommand(sqlText, connection);
                cmdJournal.ExecuteNonQuery();

                var sqlText1 = @"TRUNCATE TABLE Snapshots";
                var cmdSnapshots = new SqlCommand(sqlText, connection);
                cmdSnapshots.ExecuteNonQuery();
            }
            return dbCfg;
        }

        private static Task<IWaitResults> WaitAggregateCommands(int changeNumber, Random random, GridDomainNode node)
        {
            var commands = new List<ICommand>(changeNumber + 1);
            var createCmd = new CreateSampleAggregateCommand(random.Next(), Guid.NewGuid());

            commands.Add(createCmd);

            var expectBuilder = node.NewCommandWaiter()
                                    .Expect<SampleAggregateCreatedEvent>(e => e.SourceId == createCmd.AggregateId);


            var changeCmds = Enumerable.Range(0, changeNumber)
                                       .Select(n => new ChangeSampleAggregateCommand(random.Next(), createCmd.AggregateId))
                                       .ToArray();

            commands.AddRange(changeCmds);

        
            foreach (var cmd in changeCmds)
                expectBuilder.And<SampleAggregateChangedEvent>(e => e.SourceId == cmd.AggregateId && e.Value == cmd.Parameter.ToString());

            return expectBuilder.Create()
                                .Execute(commands.ToArray());
        }
    }
}
