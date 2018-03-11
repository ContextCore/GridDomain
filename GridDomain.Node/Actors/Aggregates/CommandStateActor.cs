using System;
using Akka.Actor;
using Akka.Persistence;
using Akka.Util.Internal;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors.Aggregates
{

    public class CommandStateActor : ReceiveActor
    {
        private PersistenceExtension Extension { get; }
        private IActorRef Journal { get; }
        readonly int _instanceId;
        readonly string writerId = Guid.NewGuid().ToString();
        private static readonly AtomicCounter InstanceCounter = new AtomicCounter(1);
        private readonly string _persistenceId;

        public CommandStateActor(string persistenceId)
        {
            _persistenceId = persistenceId;
            _instanceId = InstanceCounter.GetAndIncrement();
            Extension = Persistence.Instance.Apply(ActorBase.Context.System);
            Journal = Extension.JournalFor("");
            IActorRef commandWaiter = null;
            //If current command state was not persisted into db, persist will succeed
            //otherwise we will get exception from journal 

            Receive<ICommand>(c =>
                                    {
                                        commandWaiter = Sender;
                                        Persist(c, 1);
                                    });
            //it is default, fast path
            Receive<WriteMessageSuccess>(s =>
                                         {
                                             //Command executed, and state actor will be need only in case of command 
                                             //execution retry
                                             commandWaiter.Tell(Accepted.Instance);
                                             Context.Stop(Self);
                                         },
                                         s => s.ActorInstanceId == _instanceId);

            //if something went wrong, need to read data from db 
            Receive<WriteMessageRejected>(s =>
                                          {
                                              commandWaiter.Tell(Rejected.Instance);
                                              //dont stop self, as we have chances of futher errors during command execution 
                                              //and it will require additionals fetches of command state
                                          },
                                          s => s.ActorInstanceId == _instanceId);

            Receive<WriteMessageFailure>(s =>
                                         {
                                             commandWaiter.Tell(Rejected.Instance);
                                             //dont stop self, as we have chances of futher errors during command execution 
                                             //and it will require additionals fetches of command state
                                         },
                                         s => s.ActorInstanceId == _instanceId);
        }

        private void Persist(object commandState, int sequenceNr)
        {
            AtomicWrite atomicWrite = new AtomicWrite(new Persistent(commandState, 
                                                                     sequenceNr, 
                                                                     _persistenceId, 
                                                                     commandState.GetType().FullName,
                                                                     false,
                                                                     Self,
                                                                     writerId));
            
            Journal.Tell(new WriteMessages(new IPersistentEnvelope[] {atomicWrite}, Self, _instanceId));
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
    }
}
