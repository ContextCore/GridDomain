using System;

namespace GridDomain.Node.Actors.CommandPipe.Messages
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