using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;

namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors {

    public interface IMessageProcessor<TResult> : IMessageProcessor
    {
        new Task<TResult> Process(IMessageMetadataEnvelop message);
    }

    public interface IMessageProcessor
    {
        Task Process(IMessageMetadataEnvelop message);
    }
}