using BusinessLogicsLayer.Account;
using BusinessLogicsLayer.API;
using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.Appt;
using BusinessLogicsLayer.ArmedCat;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCat;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.BloodGroup;
using BusinessLogicsLayer.Corps;
using BusinessLogicsLayer.Div;
using BusinessLogicsLayer.EncryptBySql;
using BusinessLogicsLayer.Formation;
using BusinessLogicsLayer.Home;
using BusinessLogicsLayer.IssuingAuthority;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.OROMapp;
using BusinessLogicsLayer.Posting;
using BusinessLogicsLayer.RecordOffice;
using BusinessLogicsLayer.Registration;
using BusinessLogicsLayer.ReportReturn;
using BusinessLogicsLayer.Token;
using BusinessLogicsLayer.TrnLoginLog;
using BusinessLogicsLayer.TrnMappingUnMappingLog;
using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Identitytable;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.Smo;

namespace BusinessLogicsLayer
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserBL, UserBL>();
            services.AddTransient<IUserDB, UserDB>();

            services.AddTransient<IComd, Comd>();
            services.AddTransient<IComdDB, ComdDB>();

            services.AddTransient<ICorpsBL, CorpsBL>();
            services.AddTransient<ICorpsDB, CorpsDB>();

            services.AddTransient<IBdeBL, BdeBL>();
            services.AddTransient<IBdeDB, BdeDB>();

            services.AddTransient<IDivBL, DivBL>();
            services.AddTransient<IDivDB, DivDB>();

            services.AddTransient<IUnitBL, UnitBL>();
            services.AddTransient<IUnitDB, UnitDB>();

            services.AddTransient<IMapUnitBL, MapUnitBL>();
            services.AddTransient<IMapUnitDB, MapUnitDB>();

            services.AddTransient<IFormationBL, FormationBL>();
            services.AddTransient<IFormationDB, FormationDB>();

            services.AddTransient<IApptBL, ApptBL>();
            services.AddTransient<IApptDB, ApptDB>();

            services.AddTransient<IArmedBL, ArmedBL>();
            services.AddTransient<IArmedDB, ArmedDB>();

            services.AddTransient<IRegimentalBL, RegimentalBL>();
            services.AddTransient<IRegimentalDB, RegimentalDB>();

            services.AddTransient<IRecordOfficeBL, RecordOfficeBL>();
            services.AddTransient<IRecordOfficeDB, RecordOfficeDB>();

            services.AddTransient<IRankBL, RankBL>();
            services.AddTransient<IRankDB, RankDB>();

            services.AddTransient<IUserProfileMappingBL, UserProfileMappingBL>();

            services.AddTransient<IRegistrationBL, RegistrationBL>();
            services.AddTransient<IRegistrationDB, RegistrationDB>();

            services.AddTransient<IBasicDetailBL, BasicDetailBL>();
            services.AddTransient<IBasicDetailDB, BasicDetailDB>();

            services.AddTransient<IBasicUploadBL, BasicUploadBL>();
            services.AddTransient<IBasicinfoBL, BasicinfoBL>();
            services.AddTransient<IBasicAddressBL, BasicAddressBL>();


           
            services.AddTransient<IBasicDetailTempBL,BasicDetailTempBL>();
            services.AddTransient<IBasicDetailTempDB, BasicDetailTempDB>();

            services.AddTransient<IUserProfileBL, UserProfileBL>();
            services.AddTransient<IUserProfileDB, UserProfileDB>();


            services.AddTransient<IPostingBL, PostingBL>();
            services.AddTransient<IPostingDB, PostingDB>();

            services.AddTransient<IApplCloseBL, ApplCloseBL>();
            services.AddTransient<IApplCloseDB, ApplCloseDB>();


            //services.AddTransient<IBasicDetailTempBL, BasicDetailTempBL>();
            //services.AddTransient<IBasicDetailTempDB, BasicDetailTempDB>();

            services.AddTransient<iGetTokenBL, GetTokenBL>();

            services.AddTransient<IStepCounterBL, StepCounterBL>();
            services.AddTransient<IStepCounterDB, StepCounterDB>();

            services.AddTransient<ITrnFwnBL, TrnFwnBL>();
            services.AddTransient<ITrnFwnDB, TrnFwnDB>();

            services.AddTransient<ITrnICardRequestBL, TrnICardRequestBL>();
            services.AddTransient<ITrnICardRequestDB, TrnICardRequestDB>();

            services.AddTransient<ITrnLoginLogBL, TrnLoginLogBL>();
            services.AddTransient<ITrnLoginLogDB, TrnLoginLogDB>();

            services.AddTransient<ITrnMappingUnMappingLogBL, TrnMappingUnMappingLogBL>();
            services.AddTransient<ITrnMappingUnMappingLogDB, TrnMappingUnMappingLogDB>();

            services.AddTransient<IAPIDataBL, APIDataBL>();
            services.AddTransient<IAPIDataDB, APIDataDB>();

            services.AddTransient<IDomainMapBL, DomainMapBL>();
            services.AddTransient<IDomainMapDB, DomainMapDB>();


            services.AddTransient<IChangeHierarchyMasterBL, ChangeHierarchyMasterBL>();
            services.AddTransient<IChangeHierarchyMasterDB, ChangeHierarchyMasterDB>();



            services.AddTransient<IArmedCatBL, ArmedCatBL>();

            //////Api Calll/////////////
            services.AddTransient<IAPIBL, APIBL>();

            services.AddTransient<INotificationBL, NotificationBL>();
            services.AddTransient<INotificationDB, NotificationDB>();

            services.AddTransient<IAccountBL, AccountBL>();
            services.AddTransient<IAccountDB, AccountDB>();

            services.AddTransient<IMasterBL, MasterBL>();
            services.AddTransient<IMasterDB, MasterDB>();

            services.AddTransient<IOROMappingBL, OROMappingBL>();
            services.AddTransient<IOROMappingDB, OROMappingDB>();

            services.AddTransient<IHomeBL, HomeBL>();
            services.AddTransient<IHomeDB, HomeDB>();

            services.AddTransient<IEncryptsqlDB, EncryptsqlDB>();
            services.AddTransient<IEncryptsqlBL, EncryptsqlBL>();

            services.AddTransient<IReportReturnDB, ReportReturnDB>();
            services.AddTransient<IReportReturnBL, ReportReturnBL>();

            services.AddTransient<IIssuingAuthorityDB, IssuingAuthorityDB>();
            services.AddTransient<IIssuingAuthorityBL, IssuingAuthorityBL>();

            services.AddTransient<IBloodGroupBL, BloodGroupBL>();


            //services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Server=SDG20\\SQLEXPRESS; Database=AFSAC;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"));
            //services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Server=.\\sqlexpress; Database=AFSAC;User Id=sa; Password=Admin@2018;MultipleActiveResultSets=True;TrustServerCertificate=True"));
            // services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer("Server=192.168.10.63; database=AFSAC_New; User Id=sa; Password=Admin@2018;Connect Timeout=30;TrustServerCertificate=True; MultipleActiveResultSets=true"));
            // services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            //{
            //    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            //    options.Lockout.MaxFailedAccessAttempts = 3;
            //    options.User.RequireUniqueEmail = false;
            //    options.SignIn.RequireConfirmedAccount = true;
            //    options.SignIn.RequireConfirmedEmail = false;
            //    options.Lockout.AllowedForNewUsers = true;

            //})
            // .AddDefaultUI()
            // .AddEntityFrameworkStores<ApplicationDbContext>()
            // .AddDefaultTokenProviders();



            // return services;
        }

    }
}