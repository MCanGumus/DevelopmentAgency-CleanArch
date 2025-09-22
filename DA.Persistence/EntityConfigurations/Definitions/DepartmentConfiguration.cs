using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.Departments).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);

            builder.Property(y => y.Name).IsRequired().HasColumnType("varchar").HasMaxLength(100);

            builder.HasMany(u => u.Permissions).WithOne(y => y.Department).HasForeignKey(y => y.IdDepartmentFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Employees).WithOne(y => y.Department).HasForeignKey(y => y.IdDepartmentFK).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
