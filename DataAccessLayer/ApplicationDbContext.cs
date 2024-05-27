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
        public DbSet<MTrnFwdStatus> MTrnFwdStatus { get; set; } = null!;
        public DbSet<OROMapping> OROMapping { get; set; } = null!;
        public DbSet<MRegistration> MRegistration { get; set; } = null!;
        public DbSet <MArmedCat> MArmedCats { get; set; } = null!;
        public DbSet<MTrnFwd> TrnFwds { get; set; } = null!;
        public DbSet<DocUpload> DocUploads { get; set; } = null!;
        public DbSet<Error> Errors { get; set; } = null!;
        public DbSet<MComd> MComd { get; set; } = null!;
        public DbSet<MCorps> MCorps { get; set; } = null!;
        public DbSet<MBde> MBde { get; set; } = null!;
        public DbSet<MDiv> MDiv { get; set; } = null!;
        public DbSet<MUnit> MUnit { get; set; } = null!;
        public DbSet<MapUnit> MapUnit { get; set; } = null!;
        public DbSet<MFormation> MFormation { get; set; } = null!;
        public DbSet<MAppointment> MAppointment { get; set; } = null!;
        public DbSet<MArmedType> MArmedType { get; set; } = null!;
        public DbSet<MRank> MRank { get; set; } = null!;
        public DbSet<MStepCounter> TrnStepCounter { get; set; } = null!;
        public DbSet<MTrnICardRequest> TrnICardRequest { get; set; } = null!;
        public DbSet<MApplyFor> MApplyFor { get; set; } = null!;
        public DbSet<MPSO> MPso { get; set; } = null!;
        public DbSet<MFmnBranches> MFmnBranches { get; set; } = null!;
        public DbSet<MSubDte> MSubDte { get; set; } = null!;
        public DbSet<MBloodGroup> MBloodGroup { get; set; } = null!;



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
        public DbSet<MRecordOffice> MRecordOffice { get; set; } = null!;    
        public DbSet<MApiData> MApiData { get; set; }
        public  DbSet<MApiDataOffrs> MApiDataOffrs {  get; set; } = null!;    
        public DbSet<TrnDomainMapping> TrnDomainMapping { get; set; }

        public DbSet<MStepCounterStep> MStepCounterStep { get; set; }
        public DbSet<MTrnFwdType> MFwdType { get; set; } = null!;
        public DbSet<MICardType> MICardType { get; set; } = null!;

        public DbSet<MTrnNotification> TrnNotification { get; set; } = null!;
        public DbSet<MTrnNotificationDisplay> TrnNotificationDisplay { get; set; } = null!;

        public DbSet<MRemarkType> MRemarkType { get; set; } = null!;
        public DbSet<MRemarksApply> MRemarksApply { get; set; } = null!;
        public DbSet<MRemarks> MRemarks { get; set; } = null!;
        public DbSet<MPostingReason> MPostingReason { get; set; } = null!;
        public DbSet<TrnPostingOut> TrnPostingOut { get; set; } = null!;
        public DbSet<TrnApplClose> TrnApplClose { get; set; } = null!;





        public DbSet<TrnUnregdUser> TrnUnregdUser { get; set; }
        //public DbSet<TrnLogin_Log> TrnLogin_Log { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
           

            base.OnModelCreating(builder);
            //builder.Seed();
            
            //builder.Entity<MRecordOffice>()
            //        .HasIndex(x => new { x.ArmedId, x.TDMId })
            //        .IsUnique();
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
