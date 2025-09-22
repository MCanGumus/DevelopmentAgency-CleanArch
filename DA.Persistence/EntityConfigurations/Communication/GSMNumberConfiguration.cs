using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class GSMNumberConfiguration : IEntityTypeConfiguration<GSMNumber>
    {
        public void Configure(EntityTypeBuilder<GSMNumber> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.GSMNumbers).HasForeignKey(y => y.IdEmployeeFK);

            builder.Property(y => y.GSM).IsRequired().HasColumnType("varchar").HasMaxLength(20);


        }
    }
}
