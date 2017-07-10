using System.Collections.Generic;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs
{
    public interface IProcessorListCatalog : ICatalog<IReadOnlyCollection<IMessageProcessor>, object>
    {
        
    }
}