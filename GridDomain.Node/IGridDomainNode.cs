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

        void ConfirmedExecute(ICommand command, Type confirmationMessageType);
    }
}