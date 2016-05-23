using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster.Routing;
using Akka.DI.Core;
using Akka.Routing;
using CommonDomain.Core;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.AkkaMessaging
{

    public class BalanceHostActor : AggregateHostActor<MoneyBalance, BalanceActor>,
                                    IHandler<CreateBalanceCommand>,
                                    IHandler<WithdrawalBalanceCommand>,
                                    IHandler<ReplenishBalanceCommand>
    {
        //public BalanceHostActor()
        //{
        //    RegisterCorrelation<CreateBalanceCommand>(c => c.BalanceId);
        //    RegisterCorrelation<WithdrawalBalanceCommand>(c => c.BalanceId);
        //    RegisterCorrelation<ReplenishBalanceCommand>(c => c.BalanceId);
        //}
        public void Handle(CreateBalanceCommand e)
        {
           TellTo(e, e.BalanceId);
        }

        public void Handle(WithdrawalBalanceCommand e)
        {
            TellTo(e, e.BalanceId);
        }

        public void Handle(ReplenishBalanceCommand e)
        {
            TellTo(e, e.BalanceId);
        }
    }

    public class AggregateHostActor<TAggregate,TAggregateActor> : ReceiveActor
                                                       where TAggregateActor : AggregateActor<TAggregate>
                                                       where TAggregate: AggregateBase
    {
      //  protected IActorRef AggregateActor;

        public AggregateHostActor()
        {
            var router = new ClusterRouterPool(
                                new ConsistentHashingPool(Environment.ProcessorCount),
                                new ClusterRouterPoolSettings(10, true, 2));

            var withRouter = Context.DI().Props<TAggregateActor>().WithRouter(router);
            AggregateActor = Context.ActorOf(withRouter);
        }

       // readonly IDictionary<Type,Func<object,Guid>> _correlationLocators = new Dictionary<Type, Func<object, Guid>>();

        //protected void RegisterCorrelation<TCommand>(Func<TCommand, Guid> correlator)
        //{
        //    _correlationLocators[typeof(TCommand)] = cmd => correlator((TCommand)cmd);
        //}

        protected void TellTo<T>(T message, Guid correlationId)
        {
             AggregateActor.Tell(new ConsistentHashableEnvelope(message, correlationId));
        }

       // protected override void OnReceive(object message)
       // {
       //     var commandType = message.GetType();
       //     Func<object,Guid> handler;
       //     if (!_correlationLocators.TryGetValue(commandType, out handler))
       //         throw new UnknownMessageRecievedException();

       //     Guid correlationId = handler.Invoke(message);

       //    Tell(message, correlationId);
       //}
    }

    public class UnknownMessageRecievedException : Exception
    {
    }

    public class BalanceActor : AggregateActor<MoneyBalance>,
                                IHandler<CreateBalanceCommand>,
                                IHandler<WithdrawalBalanceCommand>,
                                IHandler<ReplenishBalanceCommand>
    {
        public BalanceActor(Guid id, AggregateFactory factory) : base(id, factory)
        {
            RegisterCommand<CreateBalanceCommand>(cmd => Aggregate = new MoneyBalance(cmd.BalanceId, cmd.BusinessId));
            RegisterCommand<ReplenishBalanceCommand>(cmd => Aggregate.Replenish(cmd.Amount));
            RegisterCommand<WithdrawalBalanceCommand>(cmd => Aggregate.Withdrawal(cmd.Amount));
        }

        public void Handle(CreateBalanceCommand e)
        {
            var actro
        }

        public void Handle(WithdrawalBalanceCommand e)
        {
        }

        public void Handle(ReplenishBalanceCommand e)
        {
        }
    }
}