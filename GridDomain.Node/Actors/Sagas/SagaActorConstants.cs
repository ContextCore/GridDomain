using GridDomain.Common;

namespace GridDomain.Node.Actors.Sagas
{
    internal static class SagaActorConstants
    {
        public const string CreatedFaultForSagaTransit = "created fault for saga transit";
        public const string SagaTransitCasedAndError = "saga transit cased and error";
        public const string PublishingCommand = "publishing command";
        public const string SagaProducedACommand = "saga produced a command";

        public static ProcessEntry ExceptionOnTransit(string name)
        {
            return new ProcessEntry(name,
                                    SagaActorConstants.CreatedFaultForSagaTransit,
                                    SagaActorConstants.SagaTransitCasedAndError);
        }

        public static ProcessEntry SagaProduceCommands(string name)
        {
            return new ProcessEntry(name,
                                    SagaActorConstants.PublishingCommand,
                                    SagaActorConstants.SagaProducedACommand);
        }
    }
}