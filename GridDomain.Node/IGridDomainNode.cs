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

       // ICommandStatus ExecuteTracking(ICommand command);
    }

    //public interface ICommandStatus
    //{
    //    //will throw an exception on failure
    //    Task<bool> Result { get; }
    //    //will be refreshed over time
    //    string State { get; }
    //    event EventHandler<string> OnStateChange;
    //}
}