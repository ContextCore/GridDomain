using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.CommandPipe {
    /// <summary>
    ///     Synhronize process managers transition and produced command execution for produced domain events
    ///     If message process policy is set to synchronized, will process such events one after one
    ///     Will process all other messages in parallel
    /// </summary>
    public class ProcessesPipeActor : ReceiveActor
    {
        public const string ProcessManagersPipeActorRegistrationName = nameof(ProcessManagersPipeActorRegistrationName);
        protected IActorRef _commandExecutionActor;
        private ILoggingAdapter Log { get; } = Context.GetSeriLogger();
        public ProcessesPipeActor(IMessageProcessor<ProcessesTransitComplete> processor)
        {
            Receive<Initialize>(i =>
                                {
                                    _commandExecutionActor = i.CommandExecutorActor;
                                    Sender.Tell(Initialized.Instance);
                                });
            //part of events or fault from command execution
            Receive<IMessageMetadataEnvelop>(env =>
                                             {
                                                 var sender = Sender;
                                                 Log.Debug("Start process managers for message {@env}", env);
                                                 processor.Process(env).ContinueWith(t =>
                                                                                     {
                                                                                         var m = t.Result;
                                                                                         Log.Debug("Process managers transited. {@res}", m);
                                                                                         
                                                                                         foreach(var envelop in m.ProducedCommands)
                                                                                             _commandExecutionActor.Tell(envelop);
                                                                                         
                                                                                         sender.Tell(m);
                                                                                     });
                                             });
        }
    
    }
}