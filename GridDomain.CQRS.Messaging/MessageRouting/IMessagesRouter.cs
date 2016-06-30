using System;
using System.Linq.Expressions;
using CommonDomain.Core;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IMessagesRouter
    {
        IRouteBuilder<TMessage> Route<TMessage>();

        void RegisterAggregate<TAggregate, TCommandHandler>()
            where TCommandHandler : AggregateCommandsHandler<TAggregate>, new()
            where TAggregate : AggregateBase;

        void RegisterAggregate(IAggregateCommandsHandlerDesriptor descriptor);

        void RegisterSaga(ISagaDescriptor sagaDescriptor);

         
    }
}

public interface IProjectionPackBuilder
{
    IProjectionConfiguration<TBuilder> Builder<TBuilder>();
    void Create();
}

public interface IProjectionConfiguration<T>
{
    IProjectionConfiguration<T> Message<TMessage>(Expression<Func<TMessage, Guid>> correlator);
    IProjectionPackBuilder Create();
}


public class ProjectionPackBuidler: IProjectionPackBuilder
{
    
}



/* RegisterProjectionBuildersPack()
        .Builder<Type>()
          .Message<Msg1>(m => m.Id)
          .Message<Msg2>((m => m.Id2).
        .Create().
        .Builder<Type>()
           .Message<Msg3>((m => m.Id3)
        .Create()
   .Create() 

    */
