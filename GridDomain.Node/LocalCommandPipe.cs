using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Routing;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.MessageProcessors;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.ProcessManagers.State;

namespace GridDomain.Node
{
    public interface ICommandPipe {
        IActorRef ProcessesPipeActor { get; }
        IActorRef HandlersPipeActor { get; }
        IActorRef CommandExecutor { get; }

        /// <summary>
        /// </summary>
        /// <returns>Reference to pipe actor for command execution</returns>
        Task<IActorRef> Init(ContainerBuilder container);
    }

    public class LocalCommandPipe : IMessagesRouter, ICommandPipe
    {
        private readonly TypeCatalog<IActorRef, object> _aggregatesCatalog = new TypeCatalog<IActorRef, object>();
        private readonly ICompositeMessageProcessor _handlersCatalog;

        private readonly ILoggingAdapter _log;
        private readonly ICompositeMessageProcessor<ProcessesTransitComplete, IProcessCompleted> _processCatalog;
        private readonly ActorSystem _system;

        public LocalCommandPipe(ActorSystem system,
                                ICompositeMessageProcessor handlersProcessor = null,
                                ICompositeMessageProcessor<ProcessesTransitComplete, IProcessCompleted> processProcessor = null)
        {
            _system = system;
            _log = system.Log;
            _handlersCatalog = handlersProcessor ?? new HandlersDefaultProcessor();
            _processCatalog = processProcessor ?? new ProcessesDefaultProcessor();
        }

        public IActorRef ProcessesPipeActor { get; private set; }
        public IActorRef HandlersPipeActor { get; private set; }
        public IActorRef CommandExecutor { get; private set; }

        public Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            var aggregateHubType = typeof(AggregateHubActor<>).MakeGenericType(descriptor.AggregateType);
            var aggregateActor = CreateDIActor(aggregateHubType, descriptor.AggregateType.BeautyName() + "_Hub");

            foreach (var aggregateCommandInfo in descriptor.RegisteredCommands)
                _aggregatesCatalog.Add(aggregateCommandInfo, aggregateActor);

            return Task.CompletedTask;
        }

        public Task RegisterProcess(IProcessDescriptor processDescriptor, string name = null)
        {
            var processHubActorType = typeof(ProcessHubActor<>).MakeGenericType(processDescriptor.StateType);
            var processHubActor = CreateDIActor(processHubActorType, name ?? processDescriptor.ProcessType.BeautyName() + "_Hub");

            var processStateHubType = typeof(ProcessStateHubActor<>).MakeGenericType(processDescriptor.StateType);
            //will be consumed in ProcessActor
            var processStateHubActor = CreateDIActor(processStateHubType, processDescriptor.StateType.BeautyName() + "_Hub");

            var processor = new ActorAskMessageProcessor<IProcessCompleted>(processHubActor);


            foreach (var acceptMsg in processDescriptor.AcceptMessages)
                _processCatalog.Add(acceptMsg.MessageType, processor);

            return Task.CompletedTask;
        }

        public Task RegisterSyncHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                              where TMessage : class, IHaveProcessId, IHaveId
        {
            return RegisterHandler<TMessage, THandler>(actor => new ActorAskMessageProcessor<HandlerExecuted>(actor));
        }

        public Task RegisterFireAndForgetHandler<TMessage, THandler>() where THandler : IHandler<TMessage>
                                                                       where TMessage : class, IHaveProcessId, IHaveId
        {
            return RegisterHandler<TMessage, THandler>(actor => new FireAndForgetActorMessageProcessor(actor));
        }

        /// <summary>
        /// </summary>
        /// <returns>Reference to pipe actor for command execution</returns>
        public async Task<IActorRef> Init(ContainerBuilder container)
        {
            _log.Debug("Command pipe is starting");

            ProcessesPipeActor = _system.ActorOf(Props.Create(() => new ProcessesPipeActor(_processCatalog)), nameof(Actors.CommandPipe.ProcessesPipeActor));

            HandlersPipeActor = _system.ActorOf(Props.Create(() => new HandlersPipeActor(_handlersCatalog, ProcessesPipeActor)),
                                                nameof(Actors.CommandPipe.HandlersPipeActor));

            CommandExecutor = _system.ActorOf(Props.Create(() => new AggregatesPipeActor(_aggregatesCatalog)),
                                              nameof(AggregatesPipeActor));

            container.RegisterInstance(HandlersPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.HandlersPipeActor.CustomHandlersProcessActorRegistrationName);
            container.RegisterInstance(ProcessesPipeActor)
                     .Named<IActorRef>(Actors.CommandPipe.ProcessesPipeActor.ProcessManagersPipeActorRegistrationName);

            await ProcessesPipeActor.Ask<Initialized>(new Initialize(CommandExecutor));
            return CommandExecutor;
        }

        private Task RegisterHandler<TMessage, THandler>(Func<IActorRef, IMessageProcessor> processorCreator) where THandler : IHandler<TMessage>
                                                                                                              where TMessage : class, IHaveProcessId, IHaveId
        {
            var handlerActorType = typeof(MessageHandleActor<TMessage, THandler>);
            var handlerActor = CreateDIActor(handlerActorType, handlerActorType.BeautyName());

            _handlersCatalog.Add<TMessage>(processorCreator(handlerActor));
            return Task.CompletedTask;
        }

        private IActorRef CreateDIActor(Type actorType, string actorName, RouterConfig routeConfig = null)
        {
            var actorProps = _system.DI()
                                    .Props(actorType);
            if (routeConfig != null)
                actorProps = actorProps.WithRouter(routeConfig);

            var actorRef = _system.ActorOf(actorProps, actorName);
            return actorRef;
        }
    }
}