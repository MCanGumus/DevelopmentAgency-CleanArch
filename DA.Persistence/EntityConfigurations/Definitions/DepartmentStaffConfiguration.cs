using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;
using DA.Domain.Entities;

namespace Definitions.Persistence.EntityConfiguration
{
    public class DepartmentStaffConfiguration : IEntityTypeConfiguration<DepartmentStaff>
    {
        public void Configure(EntityTypeBuilder<DepartmentStaff> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.DepartmentStaffs).HasForeignKey(y => y.IdEmployeeFK);
            builder.HasOne(y => y.Apellation).WithMany(u => u.DepartmentStaffs).HasForeignKey(y => y.IdApellationFK);


        }
    }
}
