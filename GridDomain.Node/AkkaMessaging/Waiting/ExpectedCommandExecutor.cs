using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class ExpectedCommandExecutor: IExpectedCommandExecutor
    {
        private readonly LocalMessagesWaiter<IExpectedCommandExecutor> _waiter;
        private TimeSpan _timeout;

        public ICommandExecutor Executor { get; }

        public ExpectedCommandExecutor(ICommandExecutor executor, LocalMessagesWaiter<IExpectedCommandExecutor> waiter, TimeSpan timeout)
        {
            Executor = executor;
            _waiter = waiter;
            _timeout = timeout;
        }

        public Task<IWaitResults> Execute<T>(T command, bool failOnFaults = true) where T : ICommand
        {
            _waiter.ExpectBuilder.Or<IFault<T>>(f => f.Message.Id == command.Id);

            Executor.Execute(command);
            return _waiter.Start(_timeout).ContinueWith(t =>
            {
                if(t.IsFaulted)
                    ExceptionDispatchInfo.Capture(t.Exception).Throw();

                if (!failOnFaults) return t.Result;

                var faults = t.Result.All.OfType<IFault>().ToArray();
                if (faults.Any())
                    throw new AggregateException(faults.Select(f => f.Exception));
                return t.Result;
            });
        }
    }
}