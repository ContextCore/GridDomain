using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GridDomain.Tools.Persistence.Microsoft.EntityFrameworkCore {
    public interface IEntityTypeConfiguration<TEntity>where TEntity : class
    {
        void Map(EntityTypeBuilder<TEntity> builder);
    }
}