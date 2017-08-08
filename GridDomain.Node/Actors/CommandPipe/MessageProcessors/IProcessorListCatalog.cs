using System.Collections.Generic;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors
{
    public interface IProcessorListCatalog : ICatalog<IReadOnlyCollection<IMessageProcessor>, object>
    {
        
    }
    public interface IProcessorListCatalog<T> : ICatalog<IReadOnlyCollection<IMessageProcessor<T>>, object>
    {

    }
}