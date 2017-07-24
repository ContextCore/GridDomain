using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class SnapshotConfiguration : IEntityTypeConfiguration<SnapshotItem>
    {
        public void Map(EntityTypeBuilder<SnapshotItem> builder)
        {
            builder.ToTable("Snapshots");
            builder.HasKey(x => new { x.PersistenceId, x.SequenceNr });

            builder.Property(x => x.PersistenceId).
                    HasColumnName(@"PersistenceId").
                    IsRequired().
                    HasColumnType("nvarchar(255)");

            builder.Property(x => x.SequenceNr).
                    HasColumnName(@"SequenceNr").
                    IsRequired().
                    HasColumnType("bigint");

            builder.Property(x => x.Timestamp).HasColumnName(@"Timestamp").IsRequired().HasColumnType("datetime2");
            builder.Property(x => x.Manifest).HasColumnName(@"Manifest").IsRequired().HasColumnType("nvarchar(500)");
            builder.Property(x => x.Snapshot).HasColumnName(@"Snapshot").IsRequired().HasColumnType("varbinary(max)");
        }
    }
}