using Akka.Actor;
using Akka.IO;
using Akka.Persistence;
using Akka.Util.Internal;
using Automatonymous;
using Automatonymous.States;

namespace GridDomain.Node.Actors.Aggregates
{
    
    public class CommandStateFastActor : ReceiveActor
    {
        private PersistenceExtension Extension { get; }
        private IActorRef Journal { get; }
        readonly int _instanceId ;
        private static readonly AtomicCounter InstanceCounter = new AtomicCounter(1);
        private CommandStateActor.CommandState State { get; set; }

        public CommandStateFastActor()
        {
            _instanceId = InstanceCounter.GetAndIncrement();
            Extension = Persistence.Instance.Apply(ActorBase.Context.System);
            Journal = Extension.JournalFor("");
            IActorRef commandWaiter = null;
            State = CommandStateActor.CommandState.New;
            //If current command state was not persisted into db, persist will succeed
            //otherwise we will get exception and will trigger full recovery to get command state
            Receive<CommandStateActor.AcceptCommandExecution>(c =>
                                                              {
                                                                  commandWaiter = Sender;
                                                                  Persist(CommandStateActor.CommandState.Accepted, 1);
                                                                  State = CommandStateActor.CommandState.Accepted;
                                                              });
            
            Receive<CommandStateActor.CommandSucceed>(c =>
                                                      {
                                                          Persist(CommandStateActor.CommandState.Executed, 2);
                                                          State = CommandStateActor.CommandState.Executed;
                                                      });
            //it is default, fast path
            Receive<WriteMessageSuccess>(s => {
                                             if(State == CommandStateActor.CommandState.Accepted)
                                                 commandWaiter.Tell(CommandStateActor.Accepted.Instance);
                                             
                                             //Command executed, and state actor will be need only in case of resend 
                                             if(State == CommandStateActor.CommandState.Executed)
                                             Context.Stop(Self);
                                         }, s => s.ActorInstanceId == _instanceId);
            
            //if something went wrong, need to read data from db 
            Receive<WriteMessageRejected>(s =>
                                          {
                                              //dont stop self, as we have chances of futher errors durin command execution 
                                              //and it will require additionals fetches of command state
                                              SendToFullCommandStateActor(commandWaiter);
                                          }, s => s.ActorInstanceId == _instanceId);
            
            Receive<WriteMessageFailure>(s =>
                                         {
                                             //dont stop self, as we have chances of futher errors durin command execution 
                                             //and it will require additionals fetches of command state
                                             SendToFullCommandStateActor(commandWaiter);
                                         }, s => s.ActorInstanceId == _instanceId);
        }

        private void Persist(CommandStateActor.CommandState commandState, int sequenceNr)
        {
            AtomicWrite atomicWrite = new AtomicWrite(new Persistent(commandState, sequenceNr, "FastState" + Self.Path.Name, typeof(CommandStateActor.CommandState).ToString()));
            Journal.Tell(new WriteMessages(new IPersistentEnvelope[] {atomicWrite}, Self, _instanceId));
        }

        private void SendToFullCommandStateActor(IActorRef commandWaiter)
        {
            var name = "Full_State_" + Self.Path.Name;

            var fullStateActor = Context.Child(name);
            if (fullStateActor == null)
                fullStateActor = Context.ActorOf(Props.Create(() => new CommandStateActor()),name);
            
            fullStateActor.Tell(CommandStateActor.AcceptCommandExecution.Instance, commandWaiter);
        }
    }

    }

    public class CommandStateActor : ReceivePersistentActor
    {
        private CommandState State { get; set; }

        public CommandStateActor()
        {
            State = CommandState.New;
            PersistenceId = Self.Path.Name;
            Command<AcceptCommandExecution>(a =>
                                            {
                                                if (State != CommandState.Executed)
                                                    Sender.Tell(Accepted.Instance);
                                                else
                                                    Sender.Tell(Rejected.Instance);
                                            });
            Command<CommandSucceed>(s => Persist(CommandState.Executed, e => State = e));
            Command<CommandFailed>(s => Persist(CommandState.Failed, e => State = e));
            Recover<CommandState>(s => State = s);
        }

        public override string PersistenceId { get; }

        public enum CommandState
        {
            New,
            Accepted,
            Executed,
            Failed
        }

        public class AcceptCommandExecution
        {
            private AcceptCommandExecution() { }
            public static AcceptCommandExecution Instance { get; } = new AcceptCommandExecution();
        }

        public class Accepted
        {
            private Accepted() { }
            public static Accepted Instance { get; } = new Accepted();
        }

        public class Rejected
        {
            private Rejected() { }
            public static Rejected Instance { get; } = new Rejected();
        }

        public class CommandFailed
        {
            private CommandFailed() { }
            public static CommandFailed Instance { get; } = new CommandFailed();
        }

        public class CommandSucceed
        {
            private CommandSucceed() { }
            public static CommandSucceed Instance { get; } = new CommandSucceed();
        }
    }