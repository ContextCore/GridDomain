using System;
using System.Linq;
using GridDomain.CQRS;

namespace GridDomain.Node
{
    //TODO: add execution track status
    public interface IGridDomainNode
    {
        void Execute(params ICommand[] commands);

        void ExecuteWithConfirmation(CommandWithKnownResult command);
    }


    public class CommandWithKnownResult : Command
    {
        public Type[] ExpectedMessages { get; }
        public Guid EventId { get; }

        protected CommandWithKnownResult(Guid id, Guid eventId,Type expectedMessageType) : base(id)
        {
            var commandFaultType = typeof(CommandFault<>).MakeGenericType(this.GetType());
            ExpectedMessages = new [] { commandFaultType, expectedMessageType };
            EventId = eventId;
        }
    }
}