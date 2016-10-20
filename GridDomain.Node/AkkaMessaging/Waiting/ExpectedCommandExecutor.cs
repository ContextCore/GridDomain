using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    class ExpectedCommandExecutor : IExpectedCommandExecutor
    {
        public ICommandExecutor Executor { get; }
        public Task<IWaitResults> Awaiter { get; }

        public ExpectedCommandExecutor(ICommandExecutor executor, Task<IWaitResults> awaiter )
        {
            Awaiter = awaiter;
            Executor = executor;
        }

        public Task<IWaitResults> Execute(ICommand command)
        {
            Executor.Execute(command);
            return Awaiter;
        }
    }
}