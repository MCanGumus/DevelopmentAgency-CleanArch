using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DA.Domain.Entities;
using DA.Domain.Enums;
using System.Runtime.InteropServices;

namespace DA.Persistence.EntityConfiguration
{
    public class EMailConfiguration : IEntityTypeConfiguration<EMail>
    {
        public void Configure(EntityTypeBuilder<EMail> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(y => y.Employee).WithMany(u => u.EMails).HasForeignKey(y => y.IdEmployeeFK);

            builder.Property(y => y.EMailAddress).IsRequired().HasColumnType("varchar").HasMaxLength(50);


        }
    }
}
