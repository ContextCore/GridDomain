using System;
using Akka;
using Akka.Actor;
using Akka.Persistence.Fsm;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.PersistentHub;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Node.AkkaMessaging;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Actors.ProcessManagers
{

    public static class ProcessHubActor
    {
        public static string GetProcessStateActorSelection(Type t) => "user/" + t.BeautyName() + "_Hub";
    }
    public class ProcessHubActor<TState> : PersistentHubActor where TState : class, IProcessState
    {
        private readonly string _processName;
        const string ProcessSpawnId = "NewProcessSpawner";


        public ProcessHubActor(IRecycleConfiguration recycleConf, string processName): base(recycleConf, processName)
        {
            _processName = processName;
        }

        internal override string GetChildActorName(string childId)
        {
            return new EntityActorName(_processName,childId).ToString();
        }

       protected override string GetChildActorId(IMessageMetadataEnvelop env)
       {
           var id = ExtractId(env);
           return string.IsNullOrEmpty(id) ? ProcessSpawnId : id;
       }

        private static string ExtractId(IMessageMetadataEnvelop env)
        {
            switch (env.Message)
            {
                case IFault f: return f.ProcessId;
                case IHaveProcessId p: return p.ProcessId;
                case ProcessRedirect r: return r.ProcessId;
            }

            throw new CannotGetProcessIdFromMessageException(env.Message);
        }

        protected override Type ChildActorType { get; } = typeof(ProcessActor<TState>);
    }
}