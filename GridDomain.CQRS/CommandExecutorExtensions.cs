namespace GridDomain.CQRS
{
    public static class CommandExecutorExtensions
    {
        public static void Execute(this ICommandExecutor executor, params ICommand[] commands)
        {
            foreach (var cmd in commands) { executor.Execute(cmd); }
        }
    }
}