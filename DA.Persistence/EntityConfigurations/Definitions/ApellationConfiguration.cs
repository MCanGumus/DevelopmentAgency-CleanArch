using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class ApellationConfiguration : IEntityTypeConfiguration<Apellation>
    {
        public void Configure(EntityTypeBuilder<Apellation> builder)
        {
            builder.HasKey(t => t.Id);


            builder.Property(y => y.Name).IsRequired().HasColumnType("varchar").HasMaxLength(100);

            builder.HasMany(u => u.DepartmentStaffs).WithOne(y => y.Apellation).HasForeignKey(y => y.IdApellationFK).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
