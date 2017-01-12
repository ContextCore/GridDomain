using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommandExecutor
    {
        void Execute(params ICommand[] commands);
        void Execute<T>(T command, IMessageMetadata metadata) where T : ICommand;
    }
}