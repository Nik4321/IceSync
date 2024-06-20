using Microsoft.EntityFrameworkCore;
using IceSync.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace IceSync.Data.Configurations
{
    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.HasKey(x => x.WorkflowId);

            builder.ToTable("Workflows");

            builder.Property(x => x.WorkflowId).ValueGeneratedNever();
        }
    }
}
