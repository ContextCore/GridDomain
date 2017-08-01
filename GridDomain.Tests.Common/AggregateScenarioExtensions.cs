using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Common {
    public static class AggregateScenarioExtensions
    {
        public static async Task<AggregateScenario<TAggregate>> Check<TAggregate>(this Task<AggregateScenario<TAggregate>> scenario) where TAggregate : Aggregate
        {
            var sc = await scenario;
            Console.WriteLine(CollectDebugInfo(sc));
            EventsExtensions.CompareEvents(sc.ExpectedEvents, sc.ProducedEvents);
            return sc;
        }

        private static string CollectDebugInfo<TAggregate>(AggregateScenario<TAggregate> sc) where TAggregate : Aggregate
        {
            var sb = new StringBuilder();
            foreach (var cmd in sc.GivenCommands)
                sb.AppendLine($"Command: {cmd.ToPropsString()}");

            AddEventInfo(sb, "Given events", sc.GivenEvents);
            AddEventInfo(sb, "Produced events", sc.ProducedEvents);
            AddEventInfo(sb, "Expected events", sc.ExpectedEvents);

            return sb.ToString();
        }
        private static void AddEventInfo(StringBuilder builder, string message, IEnumerable<DomainEvent> ev)
        {
            builder.AppendLine();
            builder.AppendLine(message);
            builder.AppendLine();
            foreach (var e in ev)
            {
                builder.AppendLine($"Event:{e?.GetType().Name} : ");
                builder.AppendLine(e?.ToPropsString());
            }
            builder.AppendLine();
        }

    }
}