using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.Permissions).HasForeignKey(y => y.IdEmployeeFK);
            builder.HasOne(y => y.Department).WithMany(u => u.Permissions).HasForeignKey(y => y.IdDepartmentFK);
            builder.HasOne(y => y.Delegate).WithMany(u => u.PermissionDelegates).HasForeignKey(y => y.IdDelegateFK);
            builder.HasOne(y => y.Proxy).WithMany(u => u.PermissionProxies).HasForeignKey(y => y.IdProxyFK);

            builder.Property(y => y.DocumentId).IsRequired().HasColumnType("varchar").HasMaxLength(20);
            builder.Property(y => y.PermissionReason).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.ExcusedLeave).IsRequired(false).HasColumnType("varchar").HasMaxLength(150);
            builder.Property(y => y.PermissionAddress).IsRequired().HasColumnType("varchar").HasMaxLength(300);
            builder.Property(y => y.StartDate).IsRequired().HasColumnType("datetime");
            builder.Property(y => y.EndDate).IsRequired().HasColumnType("datetime");
            builder.Property(y => y.Description).IsRequired(false).HasColumnType("varchar").HasMaxLength(500);
            builder.Property(y => y.RejectReason).IsRequired(false).HasColumnType("varchar").HasMaxLength(200);


        }
    }
}
