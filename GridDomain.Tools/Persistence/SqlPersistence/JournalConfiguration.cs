using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class JournalConfiguration : EntityTypeConfiguration<JournalItem>
    {
        public JournalConfiguration() : this("dbo") {}

        public JournalConfiguration(string schema)
        {
            ToTable("Journal", schema);
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
            Property(x => x.Timestamp).HasColumnName(@"Timestamp").IsRequired().HasColumnType("bigint");
            Property(x => x.IsDeleted).HasColumnName(@"IsDeleted").IsRequired().HasColumnType("bit");
            Property(x => x.Manifest).HasColumnName(@"Manifest").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Payload).HasColumnName(@"Payload").IsRequired().HasColumnType("varbinary");
            Property(x => x.Tags).HasColumnName(@"Tags").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
        }
    }
}