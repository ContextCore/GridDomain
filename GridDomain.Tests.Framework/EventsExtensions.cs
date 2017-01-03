using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDomain.EventSourcing;
using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using NUnit.Framework;

namespace GridDomain.Tests.Framework
{
    public static class EventsExtensions
    {
        private static readonly ComparisonConfig StrictConfig = new ComparisonConfig {DoublePrecision = 0.0001};

        private static readonly ComparisonConfig DateCreatedAndSagaId_IgnoreConfig = new ComparisonConfig
        {
            MembersToIgnore = new[]
            {
                nameof(DomainEvent.CreatedTime),
                nameof(DomainEvent.SagaId)
            }.ToList(),
            CustomComparers = new List<BaseTypeComparer>() { new GuidComparer(RootComparerFactory.GetRootComparer()) },
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
            CompareEventsByLogic(expected, published, logic ?? new CompareLogic(DateCreatedAndSagaId_IgnoreConfig));
        }

        private static void CompareEventsByLogic(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2,
            CompareLogic compareLogic)
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

                Assert.Fail(sb.ToString());
            }

            var eventPairs = expected.Zip(published, (e, p) => new {Expected = e, Produced = p});

            foreach (var events in eventPairs)
            {
                compareLogic.Config.ActualName = events.Produced.GetType().Name;
                compareLogic.Config.ExpectedName = events.Expected.GetType().Name;
                var comparisonResult = compareLogic.Compare(events.Expected, events.Produced);

                if (!comparisonResult.AreEqual)
                {
                    Assert.Fail(comparisonResult.DifferencesString);
                }
            }
        }

        public static void CompareEventsStrict(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2)
        {
            CompareEventsByLogic(expected1, published2, new CompareLogic {Config = StrictConfig});
        }
    }
}