namespace GridDomain.Node.Actors.Aggregates
{
    public static class AggregateActorConstants
    {
        public const string CreatedFault = "created fault";
        public const string CommandRaisedAnError = "command raised an error";
        public const string PublishingEvent = "Publishing event";
        public const string CommandExecutionFinished = "Finished command execution";
        public const string ExecutedCommand = "Command successfully executed";

        public const string CommandExecutionCreatedAnEvent = "Command execution created an event";

        public const string ErrorOnEventApplyText = "Aggregate {id} raised errors on events apply after persist while executing command {@command}  \r\n" +
                                                    "State is supposed to be corrupted.  \r\n" +
                                                    "Events will be persisted.\r\n" +
                                                    "Aggregate will be stopped immediately, all pending commands will be dropped.";
        
    }
}