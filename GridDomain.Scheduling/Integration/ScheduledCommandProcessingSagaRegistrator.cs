using GridDomain.EventSourcing.Sagas;
using Microsoft.Practices.Unity;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandProcessingSagaRegistrator
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<ISagaFactory<ScheduledCommandProcessingSaga, ScheduledCommandProcessingSagaState>, ScheduledCommandProcessingSagaFactory>();
            container.RegisterType<ISagaFactory<ScheduledCommandProcessingSaga, ScheduledMessageProcessingStarted>, ScheduledCommandProcessingSagaFactory>();
            container.RegisterType<IEmptySagaFactory<ScheduledCommandProcessingSaga>, ScheduledCommandProcessingSagaFactory>();
        }
    }
}