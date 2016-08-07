namespace GridDomain.Tools.Persistence.SqlPersistence
{
  
    public class MetadataConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Metadata>
    {
        public MetadataConfiguration()
            : this("dbo")
        {
        }

        public MetadataConfiguration(string schema)
        {
            ToTable("Metadata", schema);
            HasKey(x => new { x.PersistenceId, x.SequenceNr });

            Property(x => x.PersistenceId).HasColumnName(@"PersistenceId").IsRequired().HasColumnType("nvarchar").HasMaxLength(255).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.SequenceNr).HasColumnName(@"SequenceNr").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
        }
    }
}