using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Routing;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;
using Microsoft.Practices.Unity;

namespace GridDomain.Node
{
    public class CommandPipeBuilder : IMessagesRouter
    {
        private readonly TypeCatalog<Processor, ICommand> _aggregatesCatalog = new TypeCatalog<Processor, ICommand>();
        private readonly IUnityContainer _container;
        private readonly ProcessorListCatalog _handlersCatalog = new ProcessorListCatalog();

        private readonly ILoggingAdapter _log;
        private readonly ProcessorListCatalog _sagaCatalog = new ProcessorListCatalog();
        private readonly ActorSystem _system;

        public CommandPipeBuilder(ActorSystem system, IUnityContainer container)
        {
            _container = container;
            _system = system;
            _log = system.Log;
        }

        public IActorRef SagaProcessor { get; private set; }
        public IActorRef HandlersProcessor { get; private set; }
        public IActorRef CommandExecutor { get; private set; }

        public Task RegisterAggregate(IAggregateCommandsHandlerDescriptor descriptor)
        {
            var aggregateHubType = typeof(AggregateHubActor<>).MakeGenericType(descriptor.AggregateType);

            var aggregateActor = CreateActor(aggregateHubType, descriptor.AggregateType.BeautyName() + "_Hub");

            var processor = new Processor(aggregateActor);

            foreach (var aggregateCommandInfo in descriptor.RegisteredCommands)
                _aggregatesCatalog.Add(aggregateCommandInfo, processor);

            return Task.CompletedTask;
        }

        public Task RegisterSaga(ISagaDescriptor sagaDescriptor, string name = null)
        {
            var sagaActorType = typeof(SagaHubActor<,>).MakeGenericType(sagaDescriptor.SagaType, sagaDescriptor.StateType);

            var sagaActor = CreateActor(sagaActorType, name ?? sagaDescriptor.StateMachineType.BeautyName() + "_Hub");
            var processor = new Processor(sagaActor);

            foreach (var acceptMsg in sagaDescriptor.AcceptMessages)
                _sagaCatalog.Add(acceptMsg.MessageType, processor);

            return Task.CompletedTask;
        }

        public Task RegisterHandler<TMessage, THandler>(string correlationField) where TMessage : DomainEvent
                                                                                 where THandler : IHandler<TMessage>
        {
            return RegisterHandler<TMessage, THandler>(true);
        }

        /// <summary>
        /// </summary>
        /// <returns>Reference to pipe actor for command execution</returns>
        public async Task<IActorRef> Init()
        {
            _log.Debug("Command pipe is starting initialization");

            SagaProcessor = _system.ActorOf(Props.Create(() => new SagaPipeActor(_sagaCatalog)), nameof(SagaPipeActor));

            HandlersProcessor = _system.ActorOf(
                                                Props.Create(() => new HandlersPipeActor(_handlersCatalog, SagaProcessor)),
                                                nameof(HandlersPipeActor));

            CommandExecutor = _system.ActorOf(Props.Create(() => new AggregatesPipeActor(_aggregatesCatalog)),
                                              nameof(AggregatesPipeActor));

            _container.RegisterInstance(HandlersPipeActor.CustomHandlersProcessActorRegistrationName, HandlersProcessor);
            _container.RegisterInstance(SagaPipeActor.SagaProcessActorRegistrationName, SagaProcessor);

            await SagaProcessor.Ask<Initialized>(new Initialize(CommandExecutor));
            return CommandExecutor;
        }

        public Task RegisterHandler<TMessage, THandler>(bool sync = false) where THandler : IHandler<TMessage>
                                                                           where TMessage : DomainEvent
        {
            var handlerActorType = typeof(MessageProcessActor<TMessage, THandler>);
            var handlerActor = CreateActor(handlerActorType, handlerActorType.BeautyName());

            _handlersCatalog.Add<TMessage>(new Processor(handlerActor, new MessageProcessPolicy(sync)));
            return Task.CompletedTask;
        }

        private IActorRef CreateActor(Type actorType, string actorName, RouterConfig routeConfig = null)
        {
            var handleActorProps = _system.DI().Props(actorType);
            if (routeConfig != null)
                handleActorProps = handleActorProps.WithRouter(routeConfig);

            var handleActor = _system.ActorOf(handleActorProps, actorName);
            return handleActor;
        }
    }
}