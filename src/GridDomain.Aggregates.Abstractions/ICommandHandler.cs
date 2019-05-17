using System.Threading.Tasks;

namespace GridDomain.Aggregates.Abstractions
{
    public interface ICommandHandler<in TCommand, TResult>
    {
        Task<TResult> Execute(TCommand command);
    }
    
    
    public interface ICommandHandler<in TCommand>
    {
        Task<object> Execute(TCommand command);
    }
}