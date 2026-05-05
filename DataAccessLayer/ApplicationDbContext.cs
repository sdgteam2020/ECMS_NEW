using DataTransferObject.Domain;
using DataTransferObject.Domain.Error;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    /// <summary>
    /// Application database context class for Entity Framework Core, integrating ASP.NET Core Identity.
    /// implements IdentityDbContext with custom ApplicationUser and ApplicationRole entities using int as the primary key type.
    /// and includes DbSet properties for various domain models.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        public DbSet<ClaimsStore> ClaimsStore { get; set; } = null!;// For storing claims
        public DbSet <MTrnICardHold> MTrnICardHold { get; set; } = null!;// For ICard hold details
        public DbSet <MTrnICardStatus> MTrnICardStatus { get; set; } = null!;// For ICard status details
        public DbSet<AfsacCellMapping> AfsacCellMapping { get; set; } = null!;// For Afsac cell mapping details
        public DbSet<MTrnFwdStatus> MTrnFwdStatus { get; set; } = null!;// For forward status details
        public DbSet<OROMapping> OROMapping { get; set; } = null!;// For ORO mapping details
        public DbSet<MRegistration> MRegistration { get; set; } = null!;// For registration details
        public DbSet <MArmedCat> MArmedCats { get; set; } = null!;// For armed category details
        public DbSet<MTrnFwd> TrnFwds { get; set; } = null!;// For forward details
        public DbSet<Error> Errors { get; set; } = null!;// For error logging
        public DbSet<MComd> MComd { get; set; } = null!;// For command details
        public DbSet<MCorps> MCorps { get; set; } = null!;// For corps details
        public DbSet<MBde> MBde { get; set; } = null!;// For brigade details
        public DbSet<MDiv> MDiv { get; set; } = null!;// For division details
        public DbSet<MUnit> MUnit { get; set; } = null!;// For unit details
        public DbSet<MapUnit> MapUnit { get; set; } = null!;// For mapped unit details
        public DbSet<MFormation> MFormation { get; set; } = null!;//    For formation details
        public DbSet<MAppointment> MAppointment { get; set; } = null!;// For appointment details
        public DbSet<MArmedType> MArmedType { get; set; } = null!;// For armed type details
        public DbSet<MRank> MRank { get; set; } = null!;// For rank details
        public DbSet<MStepCounter> TrnStepCounter { get; set; } = null!;// For step counter details
        public DbSet<MTrnICardRequest> TrnICardRequest { get; set; } = null!;// For ICard request details
        public DbSet<MApplyFor> MApplyFor { get; set; } = null!;// For application type details
        public DbSet<MPSO> MPso { get; set; } = null!;// For PSO details
        public DbSet<MFmnBranches> MFmnBranches { get; set; } = null!;// For formation branches details
        public DbSet<MSubDte> MSubDte { get; set; } = null!;// For sub-dte details
        public DbSet<MBloodGroup> MBloodGroup { get; set; } = null!;// For blood group details
        public DbSet<CSVImport> CSVImports { get; set; } = null!;// For CSV import details



        /// <summary>
        /// ////Basic details with all mapiing
        /// </summary>
        public DbSet<BasicDetail> BasicDetails { get; set; } = null!;// For basic details
        public DbSet<MTrnAddress> TrnAddress { get; set; } = null!;// For address details
        public DbSet<MTrnUpload> TrnUpload { get; set; } = null!;// For upload details
        public DbSet<MTrnIdentityInfo> TrnIdentityInfo { get; set; } = null!;// For identity information details

        /// <summary>
        /// /end Basic details with all mapiing
        /// </summary>
        public DbSet<BasicDetailTemp> BasicDetailTemps { get; set; } = null!;// For temporary basic details
        public DbSet<MUserProfile> UserProfile { get; set; } = null!;// For user profile details
        public DbSet<MRegimental> MRegimental { get; set; } = null!;// For regimental details
        public DbSet<MRecordOffice> MRecordOffice { get; set; } = null!;    // For record office details
        public DbSet<MApiData> MApiData { get; set; }// For API data details
        public  DbSet<MApiDataOffrs> MApiDataOffrs {  get; set; } = null!;  // For API data officers details  
        public DbSet<TrnDomainMapping> TrnDomainMapping { get; set; }// For domain mapping details

        public DbSet<MStepCounterStep> MStepCounterStep { get; set; }// For step counter step details
        public DbSet<MTrnFwdType> MFwdType { get; set; } = null!;// For forward type details
        public DbSet<MICardType> MICardType { get; set; } = null!;// For ICard type details

        public DbSet<MTrnNotification> TrnNotification { get; set; } = null!;// For notification details
        public DbSet<MTrnNotificationDisplay> TrnNotificationDisplay { get; set; } = null!;// For notification display details

        public DbSet<MRemarkType> MRemarkType { get; set; } = null!;// For remark type details
        public DbSet<MRemarksApply> MRemarksApply { get; set; } = null!;// For remarks application details
        public DbSet<MRemarks> MRemarks { get; set; } = null!;// For remarks details
        public DbSet<MPostingReason> MPostingReason { get; set; } = null!;// For posting reason details
        public DbSet<TrnPostingOut> TrnPostingOut { get; set; } = null!;// For posting out details
        public DbSet<TrnApplClose> TrnApplClose { get; set; } = null!;// For application close details
        public DbSet<TrnUnregdUser> TrnUnregdUser { get; set; }// For unregistered user details
        public DbSet<MIssuingAuthority> MIssuingAuthority { get; set; } = null!;// For issuing authority details
        public DbSet<IAMSetting> IAMSetting { get; set; } = null!;// For IAM setting details
        public DbSet<TrnFaultyCard> TrnFaultyCard { get; set; } = null!;// For faulty card details
        public DbSet<MCategory> MCategory { get; set; } = null!;// For category details
        public DbSet<TrnHotlistCard> TrnHotlistCards { get; set; } = null!;// For hotlist card details
        public DbSet<TrnLostCard> TrnLostCards { get; set; } = null!;// For lost card details
        public DbSet<TrnDistributeCard> TrnDistributeCards { get; set; } = null!;// For distribute card details
        public DbSet<TrnDestructionCard> TrnDestructionCards { get; set; } = null!;// For destruction card details
        public DbSet<CompletedICardRequest> CompletedICardRequests { get; set; } = null!;// For completed ICard request details
        public DbSet<TrnMapUnitChangeRequest> TrnMapUnitChangeRequest { get; set; } = null!;// For map unit change request details
        public DbSet<MEncryptionSetting> MEncryptionSetting { get; set; } = null!;// For encryption setting details
        public DbSet<TrnDispatchCard> TrnDispatchCard { get; set; } = null!;// For dispatch card details
        public DbSet<TrnDispatchCardMapping> TrnDispatchCardMapping { get; set; } = null!;// For dispatch card mapping details
        public DbSet<MDispatchMode> MDispatchMode { get; set; } = null!;// For dispatch mode details
        //public DbSet<TrnLogin_Log> TrnLogin_Log { get; set; }
        public DbSet<MArmyPrefixRule> MArmyPrefixRule { get; set; } = null!;// For army prefix rule details

        protected override void OnModelCreating(ModelBuilder builder)// Fluent API configurations
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
