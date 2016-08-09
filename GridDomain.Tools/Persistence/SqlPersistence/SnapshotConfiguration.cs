namespace GridDomain.Tools.Persistence.SqlPersistence
{
  
    public class SnapshotConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Snapshot>
    {
        public SnapshotConfiguration()
            : this("dbo")
        {
        }

        public SnapshotConfiguration(string schema)
        {
            ToTable("Snapshots", schema);
            HasKey(x => new { x.PersistenceId, x.SequenceNr });

            Property(x => x.PersistenceId).HasColumnName(@"PersistenceId").IsRequired().HasColumnType("nvarchar").HasMaxLength(255).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.SequenceNr).HasColumnName(@"SequenceNr").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Timestamp).HasColumnName(@"Timestamp").IsRequired().HasColumnType("datetime2");
            Property(x => x.Manifest).HasColumnName(@"Manifest").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Snapshot_).HasColumnName(@"Snapshot").IsRequired().HasColumnType("varbinary");
        }
    }
}