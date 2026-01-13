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
using BusinessLogicsLayer.FaultyStage;
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
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddTransient<IapiDataBl, ApiDataBl>();
            services.AddTransient<IAPIDataDB, APIDataDB>();

            services.AddTransient<IDomainMapBL, DomainMapBL>();
            services.AddTransient<IDomainMapDB, DomainMapDB>();


            services.AddTransient<IChangeHierarchyMasterBL, ChangeHierarchyMasterBL>();
            services.AddTransient<IChangeHierarchyMasterDB, ChangeHierarchyMasterDB>();



            services.AddTransient<IArmedCatBL, ArmedCatBL>();

            //////Api Calll/////////////
            services.AddTransient<IaPiBl, Apibl>();

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

            services.AddTransient<IReportReturnDB, ReportReturnDB>();
            services.AddTransient<IReportReturnBL, ReportReturnBL>();

            services.AddTransient<IIssuingAuthorityDB, IssuingAuthorityDB>();
            services.AddTransient<IIssuingAuthorityBL, IssuingAuthorityBL>();

            services.AddTransient<IBloodGroupBL, BloodGroupBL>();

            services.AddTransient<IAfsacCellMappingDB, AfsacCellMappingDB>();
            services.AddTransient<IAfsacCellMappingBL, AfsacCellMappingBL>();

            services.AddTransient<IICardHoldDB, ICardHoldDB>();
            services.AddTransient<IICardHoldBL, ICardHoldBL>();

            services.AddTransient<IIAMSettingBL,IAMSettingBL>();
            services.AddTransient<IcsvImportBl, CsvImportBl>();

            services.AddTransient<ICategoryBL, CategoryBL>();

            services.AddTransient<IFaultyCardBL, FaultyCardBL>();
            services.AddTransient<IFaultyCardDB, FaultyCardDB>();

            services.AddTransient<IHotlistCardBL, HotlistCardBL>();
            services.AddTransient<IHotlistCardDB, HotlistCardDB>();

            services.AddTransient<IMapUnitChangeBL, MapUnitChangeBL>();
            services.AddTransient<IMapUnitChangeDB, MapUnitChangeDB>();

            services.AddTransient<ILostCardDB, LostCardDB>();
            services.AddTransient<ILostCardBL, LostCardBL>();

            services.AddTransient<IDistributeCardDB, DistributeCardDB>();
            services.AddTransient<IDistributeCardBL, DistributeCardBL>();

            services.AddTransient<IDestructionCardBL, DestructionCardBL>();
            services.AddTransient<IDestructionCardDB, DestructionCardDB>();

            services.AddTransient<IDispatchModeBL, DispatchModeBL>();

            services.AddTransient<IDispatchCardBL, DispatchCardBL>();

            services.AddTransient<IDispatchCardMappingBL, DispatchCardMappingBL>();
            services.AddTransient<IDispatchCardMappingDB, DispatchCardMappingDB>();

            services.AddTransient<IEncryptionSettingBL, EncryptionSettingBL>();

        }

    }
}