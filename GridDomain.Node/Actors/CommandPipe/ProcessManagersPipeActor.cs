using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Util.Internal;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Logging;

namespace GridDomain.Node.Actors.CommandPipe
{
    /// <summary>
    ///     Synhronize process managers transition and produced command execution for produced domain events
    ///     If message process policy is set to synchronized, will process such events one after one
    ///     Will process all other messages in parallel
    /// </summary>
    public class ProcessesPipeActor : ReceiveActor
    {
        public const string ProcessManagersPipeActorRegistrationName = nameof(ProcessManagersPipeActorRegistrationName);
        private readonly IProcessorListCatalog<IProcessCompleted> _catalog;
        private IActorRef _commandExecutionActor;
        private ILoggingAdapter Log { get; } = Context.GetSeriLogger();
        public ProcessesPipeActor(IProcessorListCatalog<IProcessCompleted> catalog)
        {
            _catalog = catalog;

            Receive<Initialize>(i =>
                                {
                                    _commandExecutionActor = i.CommandExecutorActor;
                                    Sender.Tell(Initialized.Instance);
                                });
            //part of events or fault from command execution
            Receive<IMessageMetadataEnvelop>(env =>
                                                  {
                                                      Log.Debug("Start process managers for message {@env}", env);
                                                      Process(env).PipeTo(Self);
                                                  });

            Receive<ProcessTransitComplete>(m =>
                                          {
                                              Log.Debug("Process managers transited. {@res}", m );

                                              foreach(var envelop in m.ProducedCommands)
                                                  _commandExecutionActor.Tell(envelop);
                                          });
        }
        
        private async Task<ProcessTransitComplete> Process(IMessageMetadataEnvelop messageMetadataEnvelop)
        {
           var res = await _catalog.ProcessMessage(messageMetadataEnvelop);
           var processTransited = res.OfType<ProcessTransited>().ToArray();
           if(!processTransited.Any())
               return ProcessTransitComplete.NoResults;

           return new ProcessTransitComplete(messageMetadataEnvelop,CreateCommandEnvelops(processTransited).ToArray());
        }

        private static IEnumerable<IMessageMetadataEnvelop<ICommand>> CreateCommandEnvelops(IEnumerable<ProcessTransited> messages)
        {
            return
                messages.SelectMany(msg => msg.ProducedCommands
                                              .Select(c => new MessageMetadataEnvelop<ICommand>(c,
                                                                                                msg.Metadata.CreateChild(c.Id, msg.ProcessTransitEntry))));
        }
    }
}