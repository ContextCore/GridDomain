namespace GridDomain.Tools.SqlPersistence
{
  
    public class JournalConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<JournalEntry>
    {
        public JournalConfiguration()
            : this("dbo")
        {
        }

        public JournalConfiguration(string schema)
        {
            ToTable("Journal", schema);
            HasKey(x => new { x.PersistenceId, x.SequenceNr });

            Property(x => x.PersistenceId).HasColumnName(@"PersistenceId").IsRequired().HasColumnType("nvarchar").HasMaxLength(255).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.SequenceNr).HasColumnName(@"SequenceNr").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Timestamp).HasColumnName(@"Timestamp").IsRequired().HasColumnType("bigint");
            Property(x => x.IsDeleted).HasColumnName(@"IsDeleted").IsRequired().HasColumnType("bit");
            Property(x => x.Manifest).HasColumnName(@"Manifest").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Payload).HasColumnName(@"Payload").IsRequired().HasColumnType("varbinary");
            Property(x => x.Tags).HasColumnName(@"Tags").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
        }
    }
}