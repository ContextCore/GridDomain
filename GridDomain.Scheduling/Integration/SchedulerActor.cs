using System;
using System.Collections.Generic;
using Akka;
using Akka.Actor;
using Akka.Event;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Integration
{
    public class RoutingActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            
        }
    }

    public interface IRequestHandlerRouter
    {
        void Route<TRequest, THandler>() where TRequest : ScheduledRequest where THandler : ScheduledTaskHandler<TRequest>;
        Type GetHandler(Type requestType);
    }

    public class RequestHandlerRouter : IRequestHandlerRouter
    {
        private readonly IDictionary<Type, Type> _routes = new Dictionary<Type, Type>();

        public void Route<TRequest, THandler>() where TRequest : ScheduledRequest where THandler : ScheduledTaskHandler<TRequest>
        {
            _routes.Add(typeof(TRequest), typeof(THandler));
        }

        public Type GetHandler(Type requestType)
        {
            return _routes[requestType];
        }
    }


    public class SchedulingAkkaEventBusTransport : IActorSubscriber, IPublisher
    {
        private readonly ActorSystem _system;
        private readonly IRequestHandlerRouter _requestHandlerRouter;
        private readonly EventStream _bus;

        public SchedulingAkkaEventBusTransport(ActorSystem system, IRequestHandlerRouter requestHandlerRouter)
        {
            _system = system;
            _requestHandlerRouter = requestHandlerRouter;
            _bus = system.EventStream;
        }

        public void Subscribe<TMessage>(IActorRef actor)
        {
            var handlerType = _requestHandlerRouter.GetHandler(typeof(TMessage));
            _system.ActorOf(Props.Create(handlerType));
            Subscribe(typeof(TMessage), actor);
        }

        public void Subscribe(Type messageType, IActorRef actor)
        {
            _bus.Subscribe(actor, messageType);
        }

        public void Publish<T>(T msg)
        {
            _bus.Publish(msg);
        }
    }

    public class SchedulerActor : ActorBase
    {
        private readonly IScheduler _scheduler;

        public SchedulerActor(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        protected override bool Receive(object message)
        {
            return message.Match()
                .With<AddTask>(AddTask)
                .With<RemoveTask>(RemoveTask)
                .WasHandled;
        }

        private void RemoveTask(RemoveTask msg)
        {
            try
            {
                var jobKey = new JobKey(msg.TaskId);
                _scheduler.DeleteJob(jobKey);
                Sender.Tell(new TaskRemoved(msg.TaskId));
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTime.UtcNow });
            }
        }

        private void AddTask(AddTask msg)
        {
            try
            {
                var job = QuartzJob.Create(msg.Request, msg.ExecutionTimeout).Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(msg.Request.TaskId)
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionFireNow())
                    .StartAt(msg.RunAt)
                    .Build();
                var fireTime = _scheduler.ScheduleJob(job, trigger);
                Sender.Tell(new TaskAdded(fireTime.UtcDateTime));
            }
            catch (Exception e)
            {
                Sender.Tell(new Failure { Exception = e, Timestamp = DateTime.UtcNow });
            }
        }

        protected override void PreStart()
        {
            _scheduler.Start();
            base.PreStart();
        }

        protected override void PostStop()
        {
            _scheduler.Shutdown();
            base.PostStop();
        }
    }
}