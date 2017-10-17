using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class JournalConfiguration : IEntityTypeConfiguration<JournalItem>
    {
        public void Map(EntityTypeBuilder<JournalItem> builder)
        {
            builder.ToTable("Journal");
            builder.HasKey(x => new {x.Ordering});
            builder.Property(x => x.PersistenceId).HasColumnName(@"PersistenceId").IsRequired().HasColumnType("nvarchar(255)");
            builder.Property(x => x.SequenceNr).HasColumnName(@"SequenceNr").IsRequired().HasColumnType("bigint");
            builder.Property(x => x.Timestamp).HasColumnName(@"Timestamp").IsRequired().HasColumnType("bigint");
            builder.Property(x => x.IsDeleted).HasColumnName(@"IsDeleted").IsRequired().HasColumnType("bit");
            builder.Property(x => x.Manifest).HasColumnName(@"Manifest").IsRequired().HasColumnType("nvarchar(500)");
            builder.Property(x => x.Payload).HasColumnName(@"Payload").IsRequired().HasColumnType("varbinary(max)");
            builder.Property(x => x.Tags).HasColumnName(@"Tags").IsRequired(false).HasColumnType("nvarchar(100)");
            builder.Property(x => x.SerializerId).HasColumnName(@"SerializerId").IsRequired(false).HasColumnType("int, null");
            builder.Property(x => x.Ordering).HasColumnName(@"Ordering").IsRequired().HasColumnType("bigint");
        }
    }
}