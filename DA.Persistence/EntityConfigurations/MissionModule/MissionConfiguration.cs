using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class MissionConfiguration : IEntityTypeConfiguration<Mission>
    {
        public void Configure(EntityTypeBuilder<Mission> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.Missions).HasForeignKey(y => y.IdEmployeeFK);
            builder.HasOne(y => y.WhoAccepted).WithMany(u => u.WhoAccepts).HasForeignKey(y => y.IdWhoAcceptedFK);
            builder.HasOne(y => y.Proxy).WithMany(u => u.Proxies).HasForeignKey(y => y.IdProxyFK);

            builder.Property(y => y.DocumentId).IsRequired().HasColumnType("varchar").HasMaxLength(13);
            builder.Property(y => y.Area).IsRequired().HasColumnType("varchar").HasMaxLength(200);
            builder.Property(y => y.Subject).IsRequired().HasColumnType("varchar").HasMaxLength(500);
            builder.Property(y => y.IsAdvanceRequested).IsRequired().HasColumnType("bit");
            builder.Property(y => y.AdvanceAmount).IsRequired(false).HasColumnType("int");
            builder.Property(y => y.Notes).IsRequired(false).HasColumnType("varchar").HasMaxLength(500);
            builder.Property(y => y.DateOfStart).IsRequired().HasColumnType("datetime");
            builder.Property(y => y.DateOfEnd).IsRequired().HasColumnType("datetime");

            builder.HasMany(u => u.VehicleRequests).WithOne(y => y.Mission).HasForeignKey(y => y.IdMissionFK);

        }
    }
}
