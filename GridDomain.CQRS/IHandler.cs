using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IHandler<in T>
    {
        void Handle(T msg);
    }

    public interface ICommandExecutor
    {
        void Execute(params ICommand[] commands);

        Task<object> Execute(ICommand command, ExpectedMessage[] expectedMessage, TimeSpan? timeout = null);
    }

}