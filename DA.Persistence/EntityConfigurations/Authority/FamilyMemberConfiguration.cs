using DA.Domain.Entities.Authority;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.EntityConfigurations.Authority
{
    internal class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
    {
        public void Configure(EntityTypeBuilder<FamilyMember> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(f => f.Employee)
               .WithMany(e => e.FamilyMembers)
               .HasForeignKey(f => f.IdEmployeeFK)
               .OnDelete(DeleteBehavior.Cascade); // Eğer çalışan silinirse, aile üyeleri de silinsin

            builder.Property(f => f.NameSurname)
                   .IsRequired()
                   .HasMaxLength(70);

            builder.Property(f => f.NationalIdentityNumber)
                   .IsRequired()
                   .HasMaxLength(11);

            builder.Property(f => f.DateOfBirth)
                   .IsRequired();

            builder.Property(f => f.FatherName)
                .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(f => f.MotherName)
                .IsRequired(false)
                   .HasMaxLength(100);

            builder.Property(f => f.SchoolName)
                .IsRequired(false)
                   .HasMaxLength(150);

            builder.Property(f => f.Class)
                .IsRequired(false)
                   .HasMaxLength(50);

            builder.Property(f => f.Description)
                .IsRequired(false)
                   .HasMaxLength(50);
        }
    }
}
