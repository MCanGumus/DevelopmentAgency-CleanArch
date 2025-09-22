using Definitions.Persistence.EntityConfiguration;
using DA.Domain.Entities;
using DA.Domain.Entities.Authority;
using DA.Domain.Entities.OldDatas;
using DA.Persistence.EntityConfiguration;
using DA.Persistence.EntityConfigurations.Authority;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Persistence.Context
{
    public class DAContext : DbContext
    {

        public DAContext(DbContextOptions<DAContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new ExamConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new ApellationConfiguration());
            modelBuilder.ApplyConfiguration(new PublicHolidayConfiguration());
            modelBuilder.ApplyConfiguration(new MissionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new EducationalInstitutionConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new GSMNumberConfiguration());
            modelBuilder.ApplyConfiguration(new EMailConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentStaffConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleConfiguration());
            modelBuilder.ApplyConfiguration(new AcademyInfoConfiguration());
            modelBuilder.ApplyConfiguration(new MissionConfiguration());
            modelBuilder.ApplyConfiguration(new VehicleRequestConfiguration());
            modelBuilder.ApplyConfiguration(new VehiclePassengerConfiguration());
            modelBuilder.ApplyConfiguration(new FamilyMemberConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        // DbSet, veritabanı tablosu üzerinde CRUD işlemlerini gerçekleştirmeyi sağlar
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Apellation> Apellations { get; set; }
        public DbSet<PublicHoliday> PublicHolidays { get; set; }
        public DbSet<MissionType> MissionTypes { get; set; }
        public DbSet<EducationalInstitution> EducationalInstitutions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<GSMNumber> GSMNumbers { get; set; }
        public DbSet<EMail> EMails { get; set; }
        public DbSet<DepartmentStaff> DepartmentStaffs { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<AcademyInfo> AcademyInfos { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<VehicleRequest> VehicleRequests { get; set; }
        public DbSet<VehiclePassenger> VehiclePassengers { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        public DbSet<OldEmployees> OldEmployees{ get; set; }
        public DbSet<OldPermissions> OldPermissions{ get; set; }
        public DbSet<OldMissions> OldMissions { get; set; }

    }
}

