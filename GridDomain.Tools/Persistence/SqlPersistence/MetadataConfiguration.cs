using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class MetadataConfiguration : EntityTypeConfiguration<Metadata>
    {
        public MetadataConfiguration() : this("dbo") {}

        public MetadataConfiguration(string schema)
        {
            ToTable("Metadata", schema);
            HasKey(x => new {x.PersistenceId, x.SequenceNr});

            Property(x => x.PersistenceId)
                .HasColumnName(@"PersistenceId")
                .IsRequired()
                .HasColumnType("nvarchar")
                .HasMaxLength(255)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.SequenceNr)
                .HasColumnName(@"SequenceNr")
                .IsRequired()
                .HasColumnType("bigint")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }
    }
}