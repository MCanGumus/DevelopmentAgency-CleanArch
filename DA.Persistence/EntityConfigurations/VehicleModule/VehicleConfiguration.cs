using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(t => t.Id);


            builder.Property(y => y.Plate).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            builder.Property(y => y.IsTemporary).IsRequired().HasColumnType("bit");
            builder.Property(y => y.IsActive).IsRequired().HasColumnType("bit");
            builder.Property(y => y.Capacity).IsRequired().HasColumnType("int");

            builder.HasMany(u => u.VehicleRequests).WithOne(y => y.Vehicle).HasForeignKey(y => y.IdVehicleFK).OnDelete(DeleteBehavior.Restrict);
           
        }
    }
}
