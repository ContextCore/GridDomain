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

        public async Task<IWaitResults> Execute(params ICommand[] commands)
        {

            foreach (var command in commands)
                _waiter.ExpectBuilder.Or(Fault.TypeFor(command),f => ((IFault<ICommand>)f).Message.Id == command.Id);

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
    }
}