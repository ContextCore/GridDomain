using System;

namespace GridDomain.Node.Actors.CommandPipe.Messages
{
    public class Project
    {
        public Project(object[] messages, Guid projectId)
        {
            Messages = messages;
            ProjectId = projectId;
        }

        public Project(params object[] messages) : this(messages, Guid.NewGuid()) {}

        public object[] Messages { get; }

        public Guid ProjectId { get; }
    }
}