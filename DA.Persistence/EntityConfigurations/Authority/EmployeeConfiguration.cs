using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Department).WithMany(u => u.Employees).HasForeignKey(y => y.IdDepartmentFK).OnDelete(DeleteBehavior.Restrict);

            builder.Property(y => y.IdentificationNumber).IsRequired().HasColumnType("varchar").HasMaxLength(11);
            builder.Property(y => y.PhoneNumber).IsRequired(false).HasColumnType("varchar").HasMaxLength(20);
            builder.Property(y => y.Apellation).IsRequired(false).HasColumnType("varchar").HasMaxLength(150);
            builder.Property(y => y.Name).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            builder.Property(y => y.Surname).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            builder.Property(y => y.MotherName).IsRequired().HasColumnType("varchar").HasMaxLength(150);
            builder.Property(y => y.FatherName).IsRequired().HasColumnType("varchar").HasMaxLength(150);
            builder.Property(y => y.PlaceOfBirth).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.DateOfBirth).IsRequired(false).HasColumnType("datetime");
            builder.Property(y => y.DateOfStart).IsRequired().HasColumnType("datetime");
            builder.Property(y => y.Email).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            builder.Property(y => y.Password).IsRequired().HasColumnType("varchar").HasMaxLength(256);
            builder.Property(y => y.PasswordSalt).IsRequired().HasColumnType("varchar").HasMaxLength(100);
            builder.Property(y => y.TotalYearlyLeave).IsRequired().HasColumnType("int");
            builder.Property(y => y.TotalUnpaidLeave).IsRequired().HasColumnType("int");
            builder.Property(y => y.TotalExcusedLeave).IsRequired().HasColumnType("int");
            builder.Property(y => y.TotalEqualizationLeave).IsRequired().HasColumnType("int");
            builder.Property(y => y.RegistrationNumber).IsRequired(false).HasColumnType("varchar").HasMaxLength(50);
            builder.Property(y => y.Chief).IsRequired(false).HasColumnType("uniqueidentifier");

            builder.HasMany(u => u.Addresses).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK);
            builder.HasMany(u => u.GSMNumbers).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK);
            builder.HasMany(u => u.EMails).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK);
            builder.HasMany(u => u.Departments).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.DepartmentStaffs).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Permissions).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.PermissionDelegates).WithOne(y => y.Delegate).HasForeignKey(y => y.IdDelegateFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.PermissionProxies).WithOne(y => y.Proxy).HasForeignKey(y => y.IdProxyFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.AcademyInfos).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.VehiclePassengers).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Missions).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.Proxies).WithOne(y => y.Proxy).HasForeignKey(y => y.IdProxyFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.WhoAccepts).WithOne(y => y.WhoAccepted).HasForeignKey(y => y.IdWhoAcceptedFK).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(u => u.FamilyMembers).WithOne(y => y.Employee).HasForeignKey(y => y.IdEmployeeFK).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
