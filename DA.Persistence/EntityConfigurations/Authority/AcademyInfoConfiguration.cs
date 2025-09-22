using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class AcademyInfoConfiguration : IEntityTypeConfiguration<AcademyInfo>
    {
        public void Configure(EntityTypeBuilder<AcademyInfo> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.AcademyInfos).HasForeignKey(y => y.IdEmployeeFK);

            builder.Property(y => y.University).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.Faculty).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.Department).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.ThesisTopic).IsRequired(false).HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.StartDate).IsRequired().HasColumnType("datetime");
            builder.Property(y => y.EndDate).IsRequired(false).HasColumnType("datetime");


        }
    }
}
