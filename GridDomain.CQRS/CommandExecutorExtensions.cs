using System.Threading.Tasks;

namespace GridDomain.CQRS {
    public static class CommandExecutorExtensions
    {
        public static async Task Execute(this ICommandExecutor executor, params ICommand[] commands)
        {
            foreach (var c in commands)
                await executor.Execute(c);
        }

        public static Task Execute(this ICommandExecutor executor, ICommand cmd, CommandConfirmationMode mode)
        {
            return executor.Execute(cmd, null, mode);
        }
    }
}