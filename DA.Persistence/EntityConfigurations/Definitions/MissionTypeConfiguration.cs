using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class MissionTypeConfiguration : IEntityTypeConfiguration<MissionType>
    {
        public void Configure(EntityTypeBuilder<MissionType> builder)
        {
            builder.HasKey(t => t.Id);


            builder.Property(y => y.TypeName).IsRequired().HasColumnType("varchar").HasMaxLength(50);


        }
    }
}
