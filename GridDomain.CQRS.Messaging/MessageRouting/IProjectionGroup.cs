namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IProjectionGroup : IProjectionGroupDescriptor
    {
        void Project(object message);
    }

}