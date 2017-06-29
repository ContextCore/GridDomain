using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Microsoft.Practices.Unity;

namespace GridGomain.Tests.Stress
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            RawCommandExecution(10, 10, 10).Wait();

            Console.WriteLine("Sleeping");
            Console.ReadKey();
        }

        public static async Task RawCommandExecution(int totalAggregateScenariosCount,
                                                     int aggregateScenarioPackSize,
                                                     int aggregateChangeAmount)
        {
            var dbCfg = new AutoTestAkkaConfiguration();

            using (var connection = new SqlConnection(dbCfg.Persistence.JournalConnectionString))
            {
                connection.Open();
                var sqlText = @"TRUNCATE TABLE Journal";
                var cmdJournal = new SqlCommand(sqlText, connection);
                await cmdJournal.ExecuteNonQueryAsync();

                var sqlText1 = @"TRUNCATE TABLE Snapshots";
                var cmdSnapshots = new SqlCommand(sqlText1, connection);
                await cmdSnapshots.ExecuteNonQueryAsync();
            }


            var cfg = new ContainerConfiguration(
                                                       c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
                                                       c => c.RegisterType<IQuartzConfig, PersistedQuartzConfig>());

            var settings = new NodeSettings(() => new StressTestAkkaConfiguration().CreateSystem()) {CustomContainerConfiguration = cfg};
            settings.DomainBuilder.Register(new BalloonDomainConfiguration());
            var node = new GridDomainNode(settings);

            await node.Start();

            var timeoutedCommads = 0;
            var random = new Random();
            var commandsInScenario = aggregateScenarioPackSize * (aggregateChangeAmount + 1);
            var totalCommandsToIssue = commandsInScenario * totalAggregateScenariosCount;

            var timer = new Stopwatch();
            timer.Start();

            for (var i = 0; i < totalAggregateScenariosCount; i++)
            {
                var packTimer = new Stopwatch();
                packTimer.Start();
                var tasks =
                    Enumerable.Range(0, aggregateScenarioPackSize)
                              .Select(t => WaitAggregateCommands(aggregateChangeAmount, random, node))
                              .ToArray();
                try
                {
                    await Task.WhenAll(tasks);
                }
                catch
                {
                    timeoutedCommads += tasks.Count(t => t.IsCanceled || t.IsFaulted);
                }

                packTimer.Stop();
                var speed = (decimal) (commandsInScenario / packTimer.Elapsed.TotalSeconds);
                var timeLeft = TimeSpan.FromSeconds((double) ((totalCommandsToIssue - i * commandsInScenario) / speed));

                Console.WriteLine($"speed :{speed} cmd/sec," + $"total errors: {timeoutedCommads}, "
                                  + $"total commands executed: {i * commandsInScenario}/{totalCommandsToIssue},"
                                  + $"approx time remaining: {timeLeft}");
            }


            timer.Stop();
            await node.Stop();

            var speedTotal = (decimal) (totalCommandsToIssue / timer.Elapsed.TotalSeconds);
            Console.WriteLine(
                              $"Executed {totalAggregateScenariosCount} batches = {totalCommandsToIssue} commands in {timer.Elapsed}");
            Console.WriteLine($"Average speed was {speedTotal} cmd/sec");

            using (var connection = new SqlConnection(dbCfg.Persistence.JournalConnectionString))
            {
                connection.Open();
                var sqlText = @"SELECT COUNT(*) FROM Journal";
                var cmdJournal = new SqlCommand(sqlText, connection);
                var count = (int) await cmdJournal.ExecuteScalarAsync();

                Console.WriteLine(count == totalCommandsToIssue
                                      ? "Journal contains all events"
                                      : $"Journal contains only {count} of {totalCommandsToIssue}");
            }
        }

   

        private static async Task WaitAggregateCommands(int changeNumber, Random random, GridDomainNode node)
        {
            await node.Prepare(new InflateNewBallonCommand(random.Next(), Guid.NewGuid()))
                      .Expect<BalloonCreated>()
                      .Execute();

            for (var num = 0; num < changeNumber; num++)
                await node.Prepare(new WriteTitleCommand(random.Next(),
                                                         new InflateNewBallonCommand(random.Next(), Guid.NewGuid()).AggregateId))
                          .Expect<BalloonTitleChanged>()
                          .Execute();
        }
    }
}