using GridDomain.Common;

namespace GridDomain.Node.Actors.ProcessManagers
{
    internal static class ProcessManagerActorConstants
    {
        public const string CreatedFaultForProcessTransit = "created fault for process transit";
        public const string ProcessTransitError = "process transit cased an error";
        public const string PublishingCommand = "publishing command";
        public const string ProcessProducedACommand = "process produced a command";

        public static ProcessEntry ExceptionOnTransit(string name)
        {
            return new ProcessEntry(name,
                                    ProcessManagerActorConstants.CreatedFaultForProcessTransit,
                                    ProcessManagerActorConstants.ProcessTransitError);
        }

        public static ProcessEntry ProcessProduceCommands(string name)
        {
            return new ProcessEntry(name,
                                    ProcessManagerActorConstants.PublishingCommand,
                                    ProcessManagerActorConstants.ProcessProducedACommand);
        }
    }
}