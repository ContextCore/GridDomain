using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;

namespace GridDomain.Tests.Stress
{
    public class CommandPlan
    {
        private readonly Func<ICommandWaiter, Task> _action;
        public ICommand Command { get; }

        public Task Execute(IGridDomainNode node)
        {
            return _action(node.Prepare(Command));
        }

        public CommandPlan(ICommand command, Func<ICommandWaiter, Task> action)
        {
            _action = action;
            Command = command;
        }
    }
}