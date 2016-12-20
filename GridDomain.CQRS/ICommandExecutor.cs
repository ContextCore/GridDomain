using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public interface ICommandExecutor
    {
        void Execute(params ICommand[] commands);
        [Obsolete("Please use ICommandWaiterFactory.PrepareCommand instead")]
        Task<object> Execute(CommandPlan plan);
        [Obsolete("Please use ICommandWaiterFactory.PrepareCommand instead")]
        Task<T> Execute<T>(CommandPlan<T> plan);
        void Execute<T>(T command, IMessageMetadata metadata) where T : ICommand;
    }
}