using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AllHandlersCompleted
    {
        public AllHandlersCompleted(Guid projectId)
        {
            ProjectId = projectId;
        }

        public Guid ProjectId { get; }
    }
}