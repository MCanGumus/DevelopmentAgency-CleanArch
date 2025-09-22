using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.InteropServices;
using DA.Domain.Entities;

namespace Definitions.Persistence.EntityConfiguration
{
    public class EducationalInstitutionConfiguration : IEntityTypeConfiguration<EducationalInstitution>
    {
        public void Configure(EntityTypeBuilder<EducationalInstitution> builder)
        {
            builder.HasKey(t => t.Id);


            builder.Property(y => y.Name).IsRequired().HasColumnType("varchar").HasMaxLength(200);


        }
    }
}
