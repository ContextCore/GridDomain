using Microsoft.EntityFrameworkCore;

namespace GridDomain.Tools.Persistence.Microsoft.EntityFrameworkCore {
    public static class ModelBuilderExtensions
    {
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, IEntityTypeConfiguration<TEntity> configuration)
            where TEntity : class
        {
            configuration.Map(modelBuilder.Entity<TEntity>());
        }
    }
}