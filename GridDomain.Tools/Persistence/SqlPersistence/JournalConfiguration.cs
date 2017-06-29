using System.ComponentModel.DataAnnotations.Schema;
using GridDomain.Tools.Persistence.Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class JournalConfiguration : IEntityTypeConfiguration<JournalItem>
    {
        public void Map(EntityTypeBuilder<JournalItem> builder)
        {
            builder.ToTable("Journal");
            builder.HasKey(x => new {x.PersistenceId, x.SequenceNr});

            builder.Property(x => x.PersistenceId).HasColumnName(@"PersistenceId").IsRequired().HasColumnType("nvarchar").HasMaxLength(255);
            builder.Property(x => x.SequenceNr).HasColumnName(@"SequenceNr").IsRequired().HasColumnType("bigint");
            builder.Property(x => x.Timestamp).HasColumnName(@"Timestamp").IsRequired().HasColumnType("bigint");
            builder.Property(x => x.IsDeleted).HasColumnName(@"IsDeleted").IsRequired().HasColumnType("bit");
            builder.Property(x => x.Manifest).HasColumnName(@"Manifest").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            builder.Property(x => x.Payload).HasColumnName(@"Payload").IsRequired().HasColumnType("varbinary");
            builder.Property(x => x.Tags).HasColumnName(@"Tags").IsRequired(false).HasColumnType("nvarchar").HasMaxLength(100);
        }
    }
}