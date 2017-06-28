using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;

namespace GridGomain.Tests.Stress
{
    public class BalloonsCreationAndChangeScenarioProjection : INodeScenario
    {
        private readonly Random _random = new Random();
        public ICollection<CommandPlan> CommandPlans { get; }

        public BalloonsCreationAndChangeScenarioProjection(int aggregateScenariosCount = 100,
                                                           int aggregateChangeAmount = 10)
        {
            CommandPlans = Enumerable.Range(0, aggregateScenariosCount)
                                     .SelectMany(c => CreateAggregatePlan(aggregateChangeAmount))
                                     .ToArray();
        }

        private IEnumerable<CommandPlan> CreateAggregatePlan(int changeAmount)
        {
            var balloonId = Guid.NewGuid();
            yield return new CommandPlan(new InflateNewBallonCommand(_random.Next(), balloonId), w => w.Expect<BalloonCreatedNotification>()
                                                                                                       .Execute(TimeSpan.FromSeconds(30)));
            for (var num = 0; num < changeAmount; num++)
                yield return new CommandPlan(new WriteTitleCommand(_random.Next(), balloonId), w => w.Expect<BalloonTitleChangedNotification>()
                                                                                                     .Execute(TimeSpan.FromSeconds(30)));
        }

        public Task Execute(IGridDomainNode node, Action<CommandPlan> singlePlanExecutedCallback)
        {
            return Task.WhenAll(CommandPlans.Select(p => node.ExecutePlan(p)
                                                             .ContinueWith(t => singlePlanExecutedCallback(p))));
        }
    }
}