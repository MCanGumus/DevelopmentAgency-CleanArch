using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class VehiclePassengerConfiguration : IEntityTypeConfiguration<VehiclePassenger>
    {
        public void Configure(EntityTypeBuilder<VehiclePassenger> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.VehicleRequest).WithMany(u => u.VehiclePassengers).HasForeignKey(y => y.IdVehicleRequestFK);
            builder.HasOne(y => y.Employee).WithMany(u => u.VehiclePassengers).HasForeignKey(y => y.IdEmployeeFK);

            builder.Property(y => y.IsDriver).IsRequired().HasColumnType("bit");


        }
    }
}
