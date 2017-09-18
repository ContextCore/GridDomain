using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;

namespace GridDomain.Tests.Stress
{
    public class CommandPlan
    {
        private readonly Func<IGridDomainNode, ICommand, Task> _action;
        public ICommand Command { get; }

        public Task Execute(IGridDomainNode node)
        {
            return _action(node,Command);
        }

        public CommandPlan(ICommand command, Func<IGridDomainNode,ICommand, Task> action)
        {
            _action = action;
            Command = command;
        }
    }
}