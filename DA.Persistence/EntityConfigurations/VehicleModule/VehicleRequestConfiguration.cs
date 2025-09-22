using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class VehicleRequestConfiguration : IEntityTypeConfiguration<VehicleRequest>
    {
        public void Configure(EntityTypeBuilder<VehicleRequest> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Vehicle).WithMany(u => u.VehicleRequests).HasForeignKey(y => y.IdVehicleFK);
            builder.HasOne(y => y.Mission).WithMany(u => u.VehicleRequests).HasForeignKey(y => y.IdMissionFK);

            builder.Property(y => y.Description).IsRequired(false).HasColumnType("varchar").HasMaxLength(500);
            builder.Property(y => y.DateOfStart).IsRequired().HasColumnType("datetime");
            builder.Property(y => y.DateOfEnd).IsRequired().HasColumnType("datetime");

            builder.HasMany(u => u.VehiclePassengers).WithOne(y => y.VehicleRequest).HasForeignKey(y => y.IdVehicleRequestFK).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
