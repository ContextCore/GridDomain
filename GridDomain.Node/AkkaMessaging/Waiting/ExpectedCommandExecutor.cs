using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class ExpectedCommandExecutor: IExpectedCommandExecutor
    {
        private readonly LocalMessagesWaiter<IExpectedCommandExecutor> _waiter;
        private readonly bool _failOnFaults;

        public ICommandExecutor Executor { get; }

        public ExpectedCommandExecutor(ICommandExecutor executor, 
                                       LocalMessagesWaiter<IExpectedCommandExecutor> waiter,
                                       bool failOnFaults)
        {
            _failOnFaults = failOnFaults;
            Executor = executor;
            _waiter = waiter;
        }

        public async Task<IWaitResults> Execute<T>(params T[] commands) where T : ICommand
        {

            foreach (var command in commands)
                _waiter.ExpectBuilder.Or<IFault<T>>(f => f.Message.Id == command.Id);

            var task = _waiter.Start();

            foreach (var command in commands)
                Executor.Execute(command);

            var res = await task;

            if (!_failOnFaults) return res;
            var faults = res.All.OfType<IFault>().ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }

        public async Task<IWaitResults> Execute<T>(T command, IMessageMetadata metadata) where T : ICommand
        {

             _waiter.ExpectBuilder.Or<IFault<T>>(f => f.Message.Id == command.Id);
             _waiter.ExpectBuilder.Or<Fault<T>>(f => f.Message.Id == command.Id);
             _waiter.ExpectBuilder.Or<IMessageMetadataEnvelop<IFault<T>>>(f => f.Message.Message.Id == command.Id);
             _waiter.ExpectBuilder.Or<IMessageMetadataEnvelop<Fault<T>>>(f => f.Message.Message.Id == command.Id);

            var task = _waiter.Start();

            Executor.Execute(command, metadata);

            var res = await task;

            if (!_failOnFaults) return res;
            var faults = res.All.OfType<IFault>().ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }
    }

    class CommandWaiter : ICommandWaiter
    {
        private readonly ICommand _cmd;
        private readonly IMessageMetadata _metadata;
        private readonly AkkaMessageLocalWaiter _waiter;
        private readonly ICommandExecutor _executor;

        public CommandWaiter(ICommand cmd, 
                             ActorSystem system, 
                             IActorTransport transport,
                             IMessageMetadata metadata,
                             TimeSpan defaultTimeout)
        {
            _executor = new AkkaCommandExecutor(system, transport);
            _waiter = new AkkaMessageLocalWaiter( system, transport, defaultTimeout);
            _metadata = metadata;
            _cmd = cmd;
        }

        public IExpectBuilder<Task<IWaitResults>> Expect<TMsg>(Predicate<TMsg> filter = null)
        {
            return _waiter.Expect(filter);
        }

        public IExpectBuilder<Task<IWaitResults>> Expect(Type type, Func<object, bool> filter = null)
        {
            return _waiter.Expect(type,filter);
        }

        public Task<IWaitResults> Execute(TimeSpan? timeout = null, bool failOnAnyFault = true)
        {
            _waiter.ExpectBuilder.Or<IFault<T>>(f => f.Message.Id == command.Id);
            _waiter.ExpectBuilder.Or<Fault<T>>(f => f.Message.Id == command.Id);
            _waiter.ExpectBuilder.Or<IMessageMetadataEnvelop<IFault<T>>>(f => f.Message.Message.Id == command.Id);
            _waiter.ExpectBuilder.Or<IMessageMetadataEnvelop<Fault<T>>>(f => f.Message.Message.Id == command.Id);

            var task = _waiter.Start();

            Executor.Execute(command, metadata);

            var res = await task;

            if (!_failOnFaults) return res;
            var faults = res.All.OfType<IFault>().ToArray();
            if (faults.Any())
                throw new AggregateException(faults.Select(f => f.Exception));

            return res;
        }
    }
}