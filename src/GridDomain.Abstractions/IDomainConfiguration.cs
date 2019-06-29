using System.Threading.Tasks;

namespace GridDomain.Abstractions
{
    public interface IDomainConfiguration
    {
        Task Configure(IDomainBuilder builder);
    }
    
    public abstract class DomainConfiguration<T>:IDomainConfiguration where T: class, IDomainPartBuilder
    {
        public Task Configure(IDomainBuilder builder)
        {
            return ConfigurePart(builder.GetPart<T>());
        }

        protected abstract Task ConfigurePart(T partBuilder);
    }
}