using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{
    public static class CommandExecutorExtensions
    {
        public static Task<T> Execute<T>(this ICommandExecutor node, ICommand command, params ExpectedMessage[] expectedMessage)
        {
            return node.Execute(new CommandPlan<T>(command, expectedMessage));
        }

        public static Task<T> Execute<T>(this ICommandExecutor node, ICommand command, ExpectedMessage<T> expectedMessage)
        {
            return node.Execute(CommandPlan.New(command, expectedMessage));
        }
    }
}