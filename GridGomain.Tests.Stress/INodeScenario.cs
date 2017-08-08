using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Node;

namespace GridGomain.Tests.Stress
{
    public interface INodeScenario {
        Task Execute(IGridDomainNode node, Action<CommandPlan> singlePlanExecutedCallback);
        ICollection<CommandPlan> CommandPlans { get; }
    }
}