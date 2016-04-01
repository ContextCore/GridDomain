using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDomain.EventSourcing;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    public static class EventsExtensions
    {
        public static void CompareEvents(IEnumerable<DomainEvent> expected1, IEnumerable<DomainEvent> published2)
        {
            DomainEvent[] expected = expected1.ToArray();
            DomainEvent[] published = published2.ToArray();

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

            var compareObjects = new CompareLogic{Config = {DoublePrecision = 0.0001}};

            var eventPairs = expected.Zip(published, (e, p) => new { Expected = e, Produced = p });


            foreach (var events in eventPairs)
            {
                compareObjects.Config.ActualName = events.Produced.GetType().Name;
                compareObjects.Config.ExpectedName = events.Expected.GetType().Name;
                var comparisonResult = compareObjects.Compare(events.Expected, events.Produced);

                if (!comparisonResult.AreEqual)
                {
                    Assert.Fail(comparisonResult.DifferencesString);
                }
            }
        }
    }
}