﻿using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using DataTransferObject.ViewModels;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.Master
{ 
    public class UserProfileBL : GenericRepositoryDL<MUserProfile>, IUserProfileBL
    {
        private readonly IUserProfileDB _iUserProfileDB;

        public UserProfileBL(ApplicationDbContext context, IUserProfileDB userProfileDB) : base(context)
        {
            _iUserProfileDB = userProfileDB;   
        }

        public Task<List<DTOUserProfileResponse>> GetAll(int DomainId, int UserId)
        {
            return _iUserProfileDB.GetAll(DomainId, UserId);
        }

        public Task<List<DTOFwdICardResponse>> GetDataForFwd(int StepId, int UnitId,string Name, int TypeId)
        {
            return _iUserProfileDB.GetDataForFwd(StepId, UnitId, Name,TypeId);
        }
        public async Task<DTOProfileResponse?> GetProfileByUserId(int UserId)
        {
            return await _iUserProfileDB.GetProfileByUserId(UserId);
        }

        public async Task<bool?> FindByArmyNoWithUserId(string ArmyNo, int UserId)
        {
          return  await _iUserProfileDB.FindByArmyNoWithUserId(ArmyNo, UserId);
        }

        public Task<DTOUserProfileResponse> GetByArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetByArmyNo(ArmyNo, UserId);
        }

        public Task<List<MUserProfile>> GetByMArmyNo(string ArmyNo, int UserId)
        {
            return _iUserProfileDB.GetByMArmyNo(ArmyNo, UserId);
        }

        public async Task<DTOProfileResponse?> GetUserProfileByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.GetUserProfileByArmyNo(ArmyNo);
        }
        public Task<List<BasicDetailVM>> GetByRequestId(int RequestId)
        {
            return _iUserProfileDB.GetByRequestId(RequestId);
        }
        public Task<DTOProfileResponse> CheckArmyNoInUserProfile(string ArmyNo, int AspNetUsersId)
        {
            return _iUserProfileDB.CheckArmyNoInUserProfile(ArmyNo, AspNetUsersId);
        }

        public Task<List<DTOFwdICardResponse>> GetOffrsByUnitMapId(int UnitId, int ISIO,int ISCO, int IntOffr)
        {
            return _iUserProfileDB.GetOffrsByUnitMapId(UnitId, ISIO, ISCO, IntOffr);
        }
        public async Task<DTOAllRelatedDataByArmyNoResponse?> GetAllRelatedDataByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.GetAllRelatedDataByArmyNo(ArmyNo);
        }
        public async Task<List<DTOAllRelatedDataByArmyNoResponse>?> GetTopByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.GetTopByArmyNo(ArmyNo);
        }
        public async Task<bool?> FindByArmyNo(string ArmyNo)
        {
            return await _iUserProfileDB.FindByArmyNo(ArmyNo);
        }
    }
}
