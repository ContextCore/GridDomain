using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDomain.EventSourcing;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace GridDomain.Tests
{
    public static class EventsExtensions
    {
        private static readonly ComparisonConfig StrictConfig = new ComparisonConfig {DoublePrecision = 0.0001};

        private static readonly ComparisonConfig DateCreatedIgnoreConfig = new ComparisonConfig
        {
            MembersToIgnore = new[] {nameof(DomainEvent.CreatedTime)}.ToList(),
            DoublePrecision = 0.0001
        };

        /// <summary>
        ///     Compare events ignoring creation date
        /// </summary>
        /// <param name="expected1"></param>
        /// <param name="published2"></param>
        public static void CompareEvents(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2)
        {
            CompareEventsByLogic(expected1, published2, new CompareLogic {Config = DateCreatedIgnoreConfig});
        }

        private static void CompareEventsByLogic(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2,
            CompareLogic compareLogic)
        {
            var expected = expected1.ToArray();
            var published = published2.ToArray();

            if (expected.Length != published.Length)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Разное количество событий");
                sb.AppendLine($"Ожидается: {expected.Length}, получено: {published.Length}");
                sb.AppendLine("Ожидаемые события:");
                sb.AppendLine(string.Join(";", expected.Select(e => e.ToString())));
                sb.AppendLine("Полученные события:");
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