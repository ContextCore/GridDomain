using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.CommandPipe {
    public class LocalProcessesPipeActor : ProcessesPipeActor
    {
        public LocalProcessesPipeActor(IMessageProcessor<ProcessesTransitComplete> processor) : base(processor) { }

        public override IMessageMetadataEnvelop EnvelopCommand(ICommand cmd, IMessageMetadataEnvelop initialMessage)
        {
            return new MessageMetadataEnvelop<ICommand>(cmd, initialMessage.Metadata.CreateChild(cmd));
        }
    }
}