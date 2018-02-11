using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class PersonCreationAndChangeScenario : INodeScenario
    {
        public ICollection<CommandPlan> CommandPlans { get; }

        public PersonCreationAndChangeScenario(int aggregateScenariosCount,
                                             int aggregateChangeAmount)
        {
            CommandPlans = Enumerable.Range(0, aggregateScenariosCount)
                                     .SelectMany(c => CreateAggregatePlan(aggregateChangeAmount))
                                     .ToArray();
        }

        private IEnumerable<CommandPlan> CreateAggregatePlan(int changeAmount)
        {
            var personId = Guid.NewGuid().ToString();
            yield return new CommandPlan(new CreatePersonCommand(personId), (n,c) => n.Execute(c));
            for (var num = 0; num < changeAmount; num++)
                yield return new CommandPlan(new GoSleepCommand(personId,Guid.NewGuid().ToString()), (n,c) => n.Execute(c));
        }

        public Task Execute(IGridDomainNode node, Action<CommandPlan> singlePlanExecutedCallback)
        {
            return Task.WhenAll(CommandPlans.Select(p => node.ExecutePlan(p)
                                                             .ContinueWith(t => singlePlanExecutedCallback(p))));
        }
    }
}