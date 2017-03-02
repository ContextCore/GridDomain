using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.CommandPipe
{
    public class AllHandlersCompleted
    {
        public AllHandlersCompleted(IMessageMetadata metadata, DomainEvent[] events, Guid projectId)
        {
            ProjectId = projectId;
            DomainEvents = events;
            Metadata = metadata;
        }

        public AllHandlersCompleted(IMessageMetadata metadata, IFault fault, Guid projectId)
        {
            Fault = fault;
            Metadata = metadata;
            DomainEvents = new DomainEvent[] {};
            ProjectId = projectId;
        }

        public DomainEvent[] DomainEvents { get; }
        public IMessageMetadata Metadata { get; }
        public IFault Fault { get; }
        public Guid ProjectId { get; }
    }
}