using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Common;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace GridDomain.Tests.Scenarios
{
    public static class EventsExtensions
    {
        private static readonly ComparisonConfig StrictConfig = new ComparisonConfig {DoublePrecision = 0.0001};

        private static readonly ComparisonConfig Ignore_Id_DateCreated_History
            = new ComparisonConfig
              {
                  MembersToIgnore = new[]
                                    {
                                        nameof(DomainEvent.CreatedTime),
                                        nameof(DomainEvent.Id),
                                        "History"
                                    }.ToList(),
                  CustomComparers = new List<BaseTypeComparer>
                                    {
                                        new AnyGuidAwareComparer(RootComparerFactory.GetRootComparer())
                                    },
                  DoublePrecision = 0.0001
              };

        private static readonly ComparisonConfig DateCreated_IgnoreConfig
            = new ComparisonConfig
              {
                  MembersToIgnore = new[]
                                    {
                                        nameof(Command.Time),
                                        nameof(Command.Id)
                                    }.ToList(),
                  CustomComparers =
                      new List<BaseTypeComparer>
                      {
                          new AnyGuidAwareComparer(RootComparerFactory.GetRootComparer()),
                          new DateTimeComparer(RootComparerFactory.GetRootComparer()),
                          new AnyIdStringAwareComparer(RootComparerFactory.GetRootComparer())
                      },
                  DoublePrecision = 0.0001
              };

        private static readonly ComparisonConfig IgnoreStateNameConfig
            = new ComparisonConfig
              {
                  MembersToIgnore = new[] {nameof(IProcessState.CurrentStateName)}.ToList(),
                  CustomComparers =
                      new List<BaseTypeComparer>
                      {
                          new AnyGuidAwareComparer(RootComparerFactory.GetRootComparer())
                      },
                  DoublePrecision = 0.0001
              };

        /// <summary>
        ///     Compare events ignoring creation date
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="published"></param>
        public static void CompareEvents(IEnumerable<DomainEvent> expected,
                                         IEnumerable<DomainEvent> published,
                                         CompareLogic logic = null)
        {
            CompareByLogic(expected, published, logic ?? new CompareLogic(Ignore_Id_DateCreated_History));
        }

        /// <summary>
        ///     Compare events ignoring creation date
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="published"></param>
        public static void CompareCommands(IEnumerable<ICommand> expected,
                                           IEnumerable<ICommand> published,
                                           CompareLogic logic = null)
        {
            CompareByLogic(expected, published, logic ?? new CompareLogic(DateCreated_IgnoreConfig));
        }

        public static void CompareState<T>(T expected, T published, CompareLogic logic = null)
        {
            CompareByLogic(new[] {expected}, new[] {published}, logic ?? new CompareLogic());
        }

        public static void CompareStateWithoutName<T>(T expected, T published, CompareLogic logic = null)
        {
            CompareByLogic(new[] {expected}, new[] {published}, logic ?? new CompareLogic(IgnoreStateNameConfig));
        }

        private static void CompareByLogic<T>(IEnumerable<T> expected1, IEnumerable<T> published2, CompareLogic compareLogic)
        {
            var expected = expected1.ToArray();
            var published = published2.ToArray();

            if (expected.Length != published.Length)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Different events number");
                sb.AppendLine($"Expected: {expected.Length}, received: {published.Length}");
                sb.AppendLine("Expected events:");
                sb.AppendLine(string.Join(";", expected.Select(e => e.ToString())));
                sb.AppendLine("Received events:");
                sb.AppendLine(string.Join(";", published.Select(e => e.ToString())));

                throw new ProducedEventsCountMismatchException(sb.ToString());
            }

            var eventPairs = expected.Zip(published, (e, p) => new {Expected = e, Produced = p});

            foreach (var events in eventPairs)
            {
                compareLogic.Config.ActualName = events.Produced.GetType().Name;
                compareLogic.Config.ExpectedName = events.Expected.GetType().Name;
                var comparisonResult = compareLogic.Compare(events.Expected, events.Produced);

                if (!comparisonResult.AreEqual)
                    throw new ProducedEventsDifferException(comparisonResult.DifferencesString);
            }
        }
    }
}