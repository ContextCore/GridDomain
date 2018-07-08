using Akka.Actor;
using Akka.Event;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Node.Actors.CommandPipe
{
    /// <summary>
    ///     Synhronize process managers transition and produced command execution for produced domain events
    ///     If message process policy is set to synchronized, will process such events one after one
    ///     Will process all other messages in parallel
    /// </summary>
    public abstract class ProcessesPipeActor : ReceiveActor
    {
        public const string ProcessManagersPipeActorRegistrationName = nameof(ProcessManagersPipeActorRegistrationName);
        protected IActorRef CommandExecutionActor;
        private ILoggingAdapter Log { get; } = Context.GetSeriLogger();

        public ProcessesPipeActor(IMessageProcessor<ProcessesTransitComplete> processor)
        {
            Receive<Initialize>(i =>
                                {
                                    CommandExecutionActor = i.CommandExecutorActor;
                                    Sender.Tell(Initialized.Instance);
                                });
            //part of events or fault from command execution
            Receive<IMessageMetadataEnvelop>(env =>
                                             {
                                                 Log.Debug("Start process managers for message {@env}", env);
                                                 processor.Process(env)
                                                          .ContinueWith(t =>
                                                                        {
                                                                            var m = t.Result;
                                                                            Log.Debug("Process managers transited. {@res}", m);

                                                                            foreach (var command in m.ProducedCommands)
                                                                                CommandExecutionActor.Tell(EnvelopCommand(command, m.InitialMessage));

                                                                            return m;
                                                                        })
                                                          .PipeTo(Sender);
                                             });
        }

        public abstract IMessageMetadataEnvelop EnvelopCommand(ICommand cmd, IMessageMetadataEnvelop initialMessage);
    }
}