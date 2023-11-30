using DataTransferObject.Domain;
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
        public DbSet<MTrnFwd> TrnFwds { get; set; } = null!;
        public DbSet<DocUpload> DocUploads { get; set; } = null!;
        public DbSet<Error> Errors { get; set; } = null!;
        public DbSet<MComd> MComd { get; set; }
        public DbSet<MCorps> MCorps { get; set; }
        public DbSet<MBde> MBde { get; set; }
        public DbSet<MDiv> MDiv { get; set; }
        public DbSet<MUnit> MUnit { get; set; }
        public DbSet<MapUnit> MapUnit { get; set; }
        public DbSet<MFormation> MFormation { get; set; }
        public DbSet<MAppointment> MAppointment { get; set; }
        public DbSet<MArmedType> MArmedType { get; set; }
        public DbSet<MRank> MRank { get; set; }
        public DbSet<MStepCounter> TrnStepCounter { get; set; }
        public DbSet<MTrnICardRequest> TrnICardRequest { get; set; }
        public DbSet<BasicDetail> BasicDetails { get; set; } = null!;
        public DbSet<BasicDetailTemp> BasicDetailTemps { get; set; } = null!;
        public DbSet<MUserProfile> UserProfile { get; set; } = null!;
        public DbSet<MMappingProfile> MMappingProfile { get; set; } = null!;
        public DbSet<MRegimental> MRegimental { get; set; } = null!;
        public DbSet<MApiData> MApiData { get; set; }
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
