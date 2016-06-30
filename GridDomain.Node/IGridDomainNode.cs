using GridDomain.CQRS;

namespace GridDomain.Node
{
    //TODO: add execution track status
    public interface IGridDomainNode
    {
        void Execute(params ICommand[] commands);
    }
}