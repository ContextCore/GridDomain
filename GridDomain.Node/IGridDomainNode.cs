using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.Configuration;

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

        protected CommandWithKnownResult(Guid id, params Type[] expectedMessages) : base(id)
        {
            ExpectedMessages = expectedMessages;
        }

        protected CommandWithKnownResult(params Type[] expectedMessages) : this(Guid.NewGuid(), expectedMessages)
        {
        }
    }
}