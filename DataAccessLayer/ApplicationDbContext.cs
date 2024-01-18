using DataAccessLayer.ExtensionsClass;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Error;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Response.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
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
        public DbSet<MRegistration> MRegistration { get; set; } = null!;
        public DbSet <MArmedCat> MArmedCats { get; set; } = null!;
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
        public DbSet<MApplyFor> MApplyFor { get; set; }
        public DbSet<MPSO> MPso { get; set; }
        public DbSet<MFmnBranches> MFmnBranches { get; set; }
        public DbSet<MSubDte> MSubDte { get; set; }
        public DbSet<MBloodGroup> MBloodGroup { get; set; }



        /// <summary>
        /// ////Basic details with all mapiing
        /// </summary>
        public DbSet<BasicDetail> BasicDetails { get; set; } = null!;
        public DbSet<MTrnAddress> TrnAddress { get; set; } = null!;
        public DbSet<MTrnUpload> TrnUpload { get; set; } = null!;
        public DbSet<MTrnIdentityInfo> TrnIdentityInfo { get; set; } = null!;

        /// <summary>
        /// /end Basic details with all mapiing
        /// </summary>
        public DbSet<BasicDetailTemp> BasicDetailTemps { get; set; } = null!;
        public DbSet<MUserProfile> UserProfile { get; set; } = null!;
        public DbSet<MMappingProfile> MMappingProfile { get; set; } = null!;
        public DbSet<MRegimental> MRegimental { get; set; } = null!;
        public DbSet<MApiData> MApiData { get; set; }
        public DbSet<TrnDomainMapping> TrnDomainMapping { get; set; }

        public DbSet<MStepCounterStep> MStepCounterStep { get; set; }
        public DbSet<MTrnFwdType> MFwdType { get; set; }
        public DbSet<MICardType> MICardType { get; set; }

        public DbSet<MTrnNotification> TrnNotification { get; set; }
        public DbSet<MTrnNotificationDisplay> TrnNotificationDisplay { get; set; }

        public DbSet<MRemarkType> MRemarkType { get; set; }
        public DbSet<MRemarksApply> MRemarksApply { get; set; }
        public DbSet<MRemarks> MRemarks { get; set; }
        public DbSet<MPostingReason> MPostingReason { get; set; }
        public DbSet<TrnPostingOut> TrnPostingOut { get; set; }



        public DbSet<TrnUnregdUser> TrnUnregdUser { get; set; }
        //public DbSet<TrnLogin_Log> TrnLogin_Log { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Seed();
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
