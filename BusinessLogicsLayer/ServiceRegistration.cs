using BusinessLogicsLayer.Account;
using BusinessLogicsLayer.AfsacCellMapp;
using BusinessLogicsLayer.API;
using BusinessLogicsLayer.APIData;
using BusinessLogicsLayer.Appt;
using BusinessLogicsLayer.ArmedCat;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCate;
using BusinessLogicsLayer.BloodGroup;
using BusinessLogicsLayer.CompletedICard;
using BusinessLogicsLayer.Corps;
using BusinessLogicsLayer.CSVImports;
using BusinessLogicsLayer.DestructionCard;
using BusinessLogicsLayer.DispatchCard;
using BusinessLogicsLayer.DispatchCardMapping;
using BusinessLogicsLayer.DispatchMode;
using BusinessLogicsLayer.DistributeCard;
using BusinessLogicsLayer.Div;
using BusinessLogicsLayer.EncryptionSetting;
using BusinessLogicsLayer.FaultyCard;
using BusinessLogicsLayer.Category;
using BusinessLogicsLayer.Formation;
using BusinessLogicsLayer.Home;
using BusinessLogicsLayer.HotlistCard;
using BusinessLogicsLayer.IAMSetting;
using BusinessLogicsLayer.IssuingAuthority;
using BusinessLogicsLayer.LostCard;
using BusinessLogicsLayer.MapUnitChange;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.OROMapp;
using BusinessLogicsLayer.Posting;
using BusinessLogicsLayer.RecordOffice;
using BusinessLogicsLayer.Registration;
using BusinessLogicsLayer.ReportReturn;
using BusinessLogicsLayer.TrnICardHold;
using BusinessLogicsLayer.TrnLoginLog;
using BusinessLogicsLayer.TrnMappingUnMappingLog;
using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using BusinessLogicsLayer.Rank;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicsLayer
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<IUserDB, UserDB>();

            services.AddScoped<IComdBL, ComdBL>();
            services.AddScoped<IComdDB, ComdDB>();

            services.AddScoped<ICorpsBL, CorpsBL>();
            services.AddScoped<ICorpsDB, CorpsDB>();

            services.AddScoped<IBdeBL, BdeBL>();
            services.AddScoped<IBdeDB, BdeDB>();

            services.AddScoped<IDivBL, DivBL>();
            services.AddScoped<IDivDB, DivDB>();

            services.AddScoped<IUnitBL, UnitBL>();
            services.AddScoped<IUnitDB, UnitDB>();

            services.AddScoped<IMapUnitBL, MapUnitBL>();
            services.AddScoped<IMapUnitDB, MapUnitDB>();

            services.AddScoped<IFormationBL, FormationBL>();
            services.AddScoped<IFormationDB, FormationDB>();

            services.AddScoped<IApptBL, ApptBL>();
            services.AddScoped<IApptDB, ApptDB>();

            services.AddScoped<IArmedBL, ArmedBL>();
            services.AddScoped<IArmedDB, ArmedDB>();

            services.AddScoped<IRegimentalBL, RegimentalBL>();
            services.AddScoped<IRegimentalDB, RegimentalDB>();

            services.AddScoped<IRecordOfficeBL, RecordOfficeBL>();
            services.AddScoped<IRecordOfficeDB, RecordOfficeDB>();

            services.AddScoped<IRankBL, RankBL>();
            services.AddScoped<IRankDB, RankDB>();

            services.AddScoped<IRegistrationBL, RegistrationBL>();
            services.AddScoped<IRegistrationDB, RegistrationDB>();

            services.AddScoped<IBasicDetailBL, BasicDetailBL>();
            services.AddScoped<IBasicDetailDB, BasicDetailDB>();

            services.AddScoped<IBasicUploadBL, BasicUploadBL>();
            services.AddScoped<IBasicinfoBL, BasicinfoBL>();
            services.AddScoped<IBasicAddressBL, BasicAddressBL>();


           
            services.AddScoped<IBasicDetailTempBL,BasicDetailTempBL>();
            services.AddScoped<IBasicDetailTempDB, BasicDetailTempDB>();

            services.AddScoped<IUserProfileBL, UserProfileBL>();
            services.AddScoped<IUserProfileDB, UserProfileDB>();


            services.AddScoped<IPostingBL, PostingBL>();
            services.AddScoped<IPostingDB, PostingDB>();

            services.AddScoped<IApplCloseBL, ApplCloseBL>();
            services.AddScoped<IApplCloseDB, ApplCloseDB>();


            services.AddScoped<IStepCounterBL, StepCounterBL>();
            services.AddScoped<IStepCounterDB, StepCounterDB>();

            services.AddScoped<ITrnFwnBL, TrnFwnBL>();
            services.AddScoped<ITrnFwnDB, TrnFwnDB>();

            services.AddScoped<ITrnICardRequestBL, TrnICardRequestBL>();
            services.AddScoped<ITrnICardRequestDB, TrnICardRequestDB>();

            services.AddScoped<ITrnLoginLogBL, TrnLoginLogBL>();
            services.AddScoped<ITrnLoginLogDB, TrnLoginLogDB>();

            services.AddScoped<ITrnMappingUnMappingLogBL, TrnMappingUnMappingLogBL>();
            services.AddScoped<ITrnMappingUnMappingLogDB, TrnMappingUnMappingLogDB>();

            services.AddScoped<IapiDataBl, ApiDataBl>();
            services.AddScoped<IAPIDataDB, APIDataDB>();

            services.AddScoped<IDomainMapBL, DomainMapBL>();
            services.AddScoped<IDomainMapDB, DomainMapDB>();


            services.AddScoped<IChangeHierarchyMasterBL, ChangeHierarchyMasterBL>();
            services.AddScoped<IChangeHierarchyMasterDB, ChangeHierarchyMasterDB>();



            services.AddScoped<IArmedCatBL, ArmedCatBL>();

            //////Api Calll/////////////
            services.AddScoped<IaPiBl, Apibl>();

            services.AddScoped<INotificationBL, NotificationBL>();
            services.AddScoped<INotificationDB, NotificationDB>();

            services.AddScoped<IAccountBL, AccountBL>();
            services.AddScoped<IAccountDB, AccountDB>();

            services.AddScoped<IMasterBL, MasterBL>();
            services.AddScoped<IMasterDB, MasterDB>();

            services.AddScoped<IOROMappingBL, OROMappingBL>();
            services.AddScoped<IOROMappingDB, OROMappingDB>();

            services.AddScoped<IHomeBL, HomeBL>();
            services.AddScoped<IHomeDB, HomeDB>();

            services.AddScoped<IReportReturnDB, ReportReturnDB>();
            services.AddScoped<IReportReturnBL, ReportReturnBL>();

            services.AddScoped<IIssuingAuthorityDB, IssuingAuthorityDB>();
            services.AddScoped<IIssuingAuthorityBL, IssuingAuthorityBL>();

            services.AddScoped<IBloodGroupBL, BloodGroupBL>();

            services.AddScoped<IAfsacCellMappingDB, AfsacCellMappingDB>();
            services.AddScoped<IAfsacCellMappingBL, AfsacCellMappingBL>();

            services.AddScoped<IICardHoldDB, ICardHoldDB>();
            services.AddScoped<IICardHoldBL, ICardHoldBL>();

            services.AddScoped<IIAMSettingBL,IAMSettingBL>();
            services.AddScoped<IcsvImportBl, CsvImportBl>();

            services.AddScoped<ICategoryBL, CategoryBL>();

            services.AddScoped<IFaultyCardBL, FaultyCardBL>();
            services.AddScoped<IFaultyCardDB, FaultyCardDB>();

            services.AddScoped<IHotlistCardBL, HotlistCardBL>();
            services.AddScoped<IHotlistCardDB, HotlistCardDB>();

            services.AddScoped<IMapUnitChangeBL, MapUnitChangeBL>();
            services.AddScoped<IMapUnitChangeDB, MapUnitChangeDB>();

            services.AddScoped<ILostCardDB, LostCardDB>();
            services.AddScoped<ILostCardBL, LostCardBL>();

            services.AddScoped<IDistributeCardDB, DistributeCardDB>();
            services.AddScoped<IDistributeCardBL, DistributeCardBL>();

            services.AddScoped<IDestructionCardBL, DestructionCardBL>();
            services.AddScoped<IDestructionCardDB, DestructionCardDB>();

            services.AddScoped<IDispatchModeBL, DispatchModeBL>();

            services.AddScoped<IDispatchCardBL, DispatchCardBL>();

            services.AddScoped<IDispatchCardMappingBL, DispatchCardMappingBL>();
            services.AddScoped<IDispatchCardMappingDB, DispatchCardMappingDB>();

            services.AddScoped<IEncryptionSettingBL, EncryptionSettingBL>();

            services.AddScoped<ICompletedICardRequestBL, CompletedICardRequestBL>();
            services.AddScoped<ICompletedICardRequestDB, CompletedICardRequestDB>();

        }

    }
}