using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public static class CommandingExtensions
    {
        public static Task<T> Execute<T>(this ICommandExecutor node, ICommand command, params ExpectedMessage[] expectedMessage)
        {
            return node.Execute(new CommandPlan<T>(command, expectedMessage));
        }

        public static Task<T> Execute<T>(this ICommandExecutor node, ICommand command, ExpectedMessage<T> expectedMessage)
        {
            return node.Execute<T>(CommandPlan.New(command, expectedMessage));
        }

        public static T ExecuteSync<T>(this ICommandExecutor node, ICommand command, TimeSpan timeout, ExpectedMessage<T> expectedMessage)
        {
            return ExecuteSync(node, CommandPlan.New(command, timeout, expectedMessage));
        }

        public static T ExecuteSync<T>(this ICommandExecutor node, CommandPlan<T> plan)
        {
            var task = node.Execute(plan);
            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                var domainException = ex.UnwrapSingle();
                ExceptionDispatchInfo.Capture(domainException).Throw();
            }
            catch (TimeoutException ex)
            {
                var domainException = ex.UnwrapSingle();
                ExceptionDispatchInfo.Capture(domainException).Throw();
            }

            throw new InvalidOperationException();
        }
    }
}