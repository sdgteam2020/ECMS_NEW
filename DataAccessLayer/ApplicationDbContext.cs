using DataTransferObject.Domain.Error;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<DocUpload> DocUploads { get; set; } = null!;
        public DbSet<Error> Errors { get; set; } = null!;
        public DbSet<Comd> MComd { get; set; }
        public DbSet<MCorps> MCorps { get; set; }
        public DbSet<MBde> MBde { get; set; }
        public DbSet<MDiv> MDiv { get; set; }
        public DbSet<MUnit> MUnit { get; set; }
        public DbSet<MapUnit> MapUnit { get; set; }
        public DbSet<MFormation> MFormation { get; set; }
        public DbSet<MAppointment> MAppointment { get; set; }
        public DbSet<MArmedType> MArmedType { get; set; }
        public DbSet<State> MStates { get; set; } = null!;
        public DbSet<District> MDistricts { get; set; } = null!;
        public DbSet<BasicDetail> BasicDetails { get; set; } = null!;
        public DbSet<ProfileData> ProfileDatas { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserRole<int>>(entity =>
            {
            });

            builder.Entity<IdentityUserClaim<int>>(entity =>
            {
            });

            builder.Entity<IdentityUserLogin<int>>(entity =>
            {
   
            });

            builder.Entity<IdentityRoleClaim<int>>(entity =>
            {
            });

            builder.Entity<IdentityUserToken<int>>(entity =>
            {
            });

            //Foreign key with NO ACTION ON DELETE

            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
