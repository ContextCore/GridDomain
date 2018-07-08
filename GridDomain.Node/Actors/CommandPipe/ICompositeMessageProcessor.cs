using System;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;

namespace GridDomain.Node.Actors.CommandPipe {

    public interface ICompositeMessageProcessor<TResult, TPart> : IMessageProcessor<TResult>
    {
        void Add(Type messageType, IMessageProcessor<TPart> messageProcessor);
    }

    public interface ICompositeMessageProcessor : IMessageProcessor
    {
        void Add(Type messageType, IMessageProcessor messageProcessor);
    }
}