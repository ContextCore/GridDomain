using GridDomain.Tools.Persistence.Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class MetadataConfiguration : IEntityTypeConfiguration<Metadata>
    {
        public void Map(EntityTypeBuilder<Metadata> builder)
        {
            builder.ToTable("Metadata");
            builder.HasKey(x => new {x.PersistenceId, x.SequenceNr});

            builder.Property(x => x.PersistenceId).
                    HasColumnName(@"PersistenceId").
                    IsRequired().
                    HasColumnType("nvarchar").
                    HasMaxLength(255);

            builder.Property(x => x.SequenceNr).
                    HasColumnName(@"SequenceNr").
                    IsRequired().
                    HasColumnType("bigint");
        }
    }
}