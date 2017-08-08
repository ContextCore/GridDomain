using System;
using GridDomain.CQRS;

namespace GridDomain.Node
{
    public interface IGridDomainNode : ICommandExecutor,
                                       IMessageWaiterFactory,
                                       IDisposable {}
}