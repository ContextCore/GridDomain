using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Node.Actors
{
    class SaveEventsAsync
    {
        private SaveEventsAsync()
        {

        }
        public static SaveEventsAsync Instance { get; } = new SaveEventsAsync();
    }

    public class ProducedEventsPersisted
    {
        private ProducedEventsPersisted()
        {
            
        }
        public static ProducedEventsPersisted Instance { get; } = new ProducedEventsPersisted();
    }
    public class EventPersistingInProgress
    {
        public EventPersistingInProgress()
        {

        }
        public static EventPersistingInProgress Instance { get; } = new EventPersistingInProgress();
    }

    public class CommandHandlerExecuted
    {
        private CommandHandlerExecuted()
        {

        }
        public static CommandHandlerExecuted Instance { get; } = new CommandHandlerExecuted();
    }

    public class CommandExecuted
    {
        private CommandExecuted()
        {

        }
        public static CommandExecuted Instance { get; } = new CommandExecuted();
    }
}