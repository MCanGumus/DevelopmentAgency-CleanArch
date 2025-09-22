using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.Addresses).HasForeignKey(y => y.IdEmployeeFK);

            builder.Property(y => y.AddressTitle).IsRequired().HasColumnType("varchar").HasMaxLength(50);
            builder.Property(y => y.FullAddress).IsRequired().HasColumnType("varchar").HasMaxLength(250);


        }
    }
}
