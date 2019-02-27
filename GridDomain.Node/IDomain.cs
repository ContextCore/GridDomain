using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors.Aggregates;

namespace GridDomain.Node
{
    public interface IDomain
    {
        ICommandHandler<ICommand> CommandExecutor { get; }
        IAggregatesLifetime AggregatesLifetime { get; }
    }

    public class Domain:IDomain
    {
        public Domain(ICommandHandler<ICommand> commandExecutor, IAggregatesLifetime aggregatesLifetime)
        {
            CommandExecutor = commandExecutor;
            AggregatesLifetime = aggregatesLifetime;
        }

        public ICommandHandler<ICommand> CommandExecutor { get; }
        public IAggregatesLifetime AggregatesLifetime { get; }
    }
    
    public interface IAggregatesLifetime
    {
       // Task WakeUp(string aggregateId);
      //  Task Shutdown(Type aggregateType, string aggregateId);
      //  Task<bool> IsActive(Type type, string catName, TimeSpan? timeout =null);
        Task<AggregateHealthReport> GetHealth(IAggregateAddress address, TimeSpan? timeout = null);
    }

    public static class AggregatesLifetimeExtensions
    {
        public static Task<AggregateHealthReport> GetHealth<TAggregate>(this IAggregatesLifetime lifetime, string id, TimeSpan? timeout = null)
        {
            return lifetime.GetHealth(id.AsAddressFor<TAggregate>(), timeout);
        }
    }

}