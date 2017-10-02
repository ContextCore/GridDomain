using System.Threading.Tasks;
using GridDomain.Node;

namespace GridDomain.Tests.Stress
{
    public static class GridNodeExtensions
    {
        public static Task ExecutePlan(this IGridDomainNode node, CommandPlan plan)
        {
            return plan.Execute(node);
        }
    }
}