using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System.Data;


namespace DataAccessLayer
{
    public class BasicDetailTempDB : GenericRepositoryDL<BasicDetailTemp>, IBasicDetailTempDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<BasicDetailTempDB> _logger;
        public BasicDetailTempDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<BasicDetailTempDB> logger) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }

        /// <summary>
        /// Retrieves all basic details from the temporary table for a given user, based on their TypeId, and forwards condition.
        /// </summary>
        /// <param name="UserId">The ID of the user for which the data is to be fetched.</param>
        /// <param name="TypeId">The type of the record (e.g., 1 for specific user details).</param>
        /// <param name="dTOApplFwdCondition">Conditions for forwarding application.</param>
        /// <param name="ArmedIdForORO">The armed ID for the ORO mapping.</param>
        /// <returns>A list of DTOBasicDetailTempRequest containing the basic details of the user.</returns>
        public async Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId, int TypeId, DTOApplFwdConditionRequest dTOApplFwdCondition, short ArmedIdForORO)
        {
            try
            {
                if (TypeId == 1)
                {
                    //var BasicDetailTempList = _context.BasicDetailTemps.Where(x => x.Updatedby == UserId).ToList();
                    var query = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                                " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                                " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps  Temps" +
                                " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                                " WHERE Temps.Updatedby=@UserId AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";
                    using (var connection = _contextDP.CreateConnection())
                    {
                        var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(query, new { UserId });
                        int sno = 1;
                        var allrecord = (from e in BasicDetailTempList
                                         select new DTOBasicDetailTempRequest()
                                         {
                                             EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                             Sno = sno++,
                                             FName = e.FName,
                                             LName = e.LName,
                                             ServiceNo = e.ServiceNo,
                                             DOB = e.DOB,
                                             DateOfCommissioning = e.DateOfCommissioning,
                                             District = e.District,
                                             PO = e.PO,
                                             PS = e.PS,
                                             PinCode = e.PinCode,
                                             State = e.State,
                                             Tehsil = e.Tehsil,
                                             Village = e.Village,
                                             Observations = e.Observations,
                                             Remarks2 = e.Remarks2,
                                             RankName = e.RankName,
                                             UpdatedOn = e.UpdatedOn,
                                             RegistrationId = e.RegistrationId,
                                             TypeId = e.TypeId,
                                             ApplyForId = e.ApplyForId


                                         }).ToList();
                        return await Task.FromResult(allrecord);
                    }
                }
                else
                {
                    var query = " SELECT CASE WHEN ISNULL(RECO.TDMId,0) >0 THEN RECO.TDMId ELSE ORO.TDMId END TDMId, " +
                                " (select AspNetUsersId from TrnDomainMapping where id =(CASE WHEN ISNULL(RECO.TDMId,0) >0 THEN RECO.TDMId ELSE ORO.TDMId END )) as AspNetUsersId, " +
                                " RECO.Name,RECO.ArmedId " +
                                " into #temp " +
                                " FROM MRecordOffice RECO " +
                                " LEFT JOIN OROMapping ORO ON RECO.RecordOfficeId=ORO.RecordOfficeId " +
                                " SELECT * from #temp where AspNetUsersId=@UserId " +
                                " drop table #temp ";
                    using (var connection = _contextDP.CreateConnection())
                    {
                        var result = await connection.QueryAsync<DTOBasicDetailTempObsRequest>(query, new { UserId });
                        DTOBasicDetailTempObsRequest? dTOBasicDetailTempObsRequest = result.FirstOrDefault();
                        if (dTOBasicDetailTempObsRequest == null)
                        {
                            List<DTOBasicDetailTempRequest> dTOBasicDetailTempRequests = new List<DTOBasicDetailTempRequest>();
                            return await Task.FromResult(dTOBasicDetailTempRequests);
                        }
                        else if (dTOBasicDetailTempObsRequest.ArmedId != ArmedIdForORO)
                        {
                            var QueryFinal = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                                        " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                                        " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM TrnDomainMapping tdm" +
                                        " inner join MRecordOffice mrec on mrec.TDMId = tdm.Id " +
                                        " inner join BasicDetailTemps Temps on Temps.ArmedId = mrec.ArmedId " +
                                        " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                                        " WHERE tdm.AspNetUsersId=@UserId AND Temps.ApplyForId = 2 AND Temps.IsActive = 1 ORDER BY Temps.UpdatedOn DESC";

                            var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(QueryFinal, new { UserId });
                            int sno = 1;
                            var allrecord = (from e in BasicDetailTempList
                                             select new DTOBasicDetailTempRequest()
                                             {
                                                 EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                                 Sno = sno++,
                                                 FName = e.FName,
                                                 LName = e.LName,
                                                 ServiceNo = e.ServiceNo,
                                                 DOB = e.DOB,
                                                 DateOfCommissioning = e.DateOfCommissioning,
                                                 District = e.District,
                                                 PO = e.PO,
                                                 PS = e.PS,
                                                 PinCode = e.PinCode,
                                                 State = e.State,
                                                 Tehsil = e.Tehsil,
                                                 Village = e.Village,
                                                 Observations = e.Observations,
                                                 Remarks2 = e.Remarks2,
                                                 RankName = e.RankName,
                                                 UpdatedOn = e.UpdatedOn,
                                                 RegistrationId = e.RegistrationId,
                                                 TypeId = e.TypeId,
                                                 ApplyForId = e.ApplyForId


                                             }).ToList();
                            return await Task.FromResult(allrecord);
                        }
                        else
                        {
                            int TDMId = dTOBasicDetailTempObsRequest.TDMId;
                            if (dTOBasicDetailTempObsRequest.Name == dTOApplFwdCondition.MPRSO.Name) //"MPRSO"
                            {
                                var QueryFinal = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                                                " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                                                " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps Temps" +
                                                " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                                                " inner join MArmedType at on at.ArmedId = Temps.ArmedId" +
                                                " WHERE Temps.ApplyForId=1 AND at.Abbreviation in @MPRSO_ArmedAbbreviation AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";
                                
                                var parameters = new DynamicParameters();
                                parameters.Add("@MPRSO_ArmedAbbreviation", dTOApplFwdCondition.MPRSO.ArmedAbbreviation);
                                
                                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(QueryFinal, parameters);
                                int sno = 1;
                                var allrecord = (from e in BasicDetailTempList
                                                 select new DTOBasicDetailTempRequest()
                                                 {
                                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                                     Sno = sno++,
                                                     FName = e.FName,
                                                     LName = e.LName,
                                                     ServiceNo = e.ServiceNo,
                                                     DOB = e.DOB,
                                                     DateOfCommissioning = e.DateOfCommissioning,
                                                     District = e.District,
                                                     PO = e.PO,
                                                     PS = e.PS,
                                                     PinCode = e.PinCode,
                                                     State = e.State,
                                                     Tehsil = e.Tehsil,
                                                     Village = e.Village,
                                                     Observations = e.Observations,
                                                     Remarks2 = e.Remarks2,
                                                     RankName = e.RankName,
                                                     UpdatedOn = e.UpdatedOn,
                                                     RegistrationId = e.RegistrationId,
                                                     TypeId = e.TypeId,
                                                     ApplyForId = e.ApplyForId


                                                 }).ToList();
                                return await Task.FromResult(allrecord);
                            }
                            else if (dTOBasicDetailTempObsRequest.Name == dTOApplFwdCondition.MP6A.Name) //"MP 6A"
                            {
                                var QueryFinal = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                                                " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                                                " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps Temps" +
                                                " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                                                " WHERE Temps.ApplyForId=1 AND ranks1.Orderby <= @MP6A_RankOrderby AND SUBSTRING(UPPER(Temps.ServiceNo),1,2) != @MP6F_ArmyNoPrefix  AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";

                                var parameters = new DynamicParameters();
                                parameters.Add("@MP6A_RankOrderby", dTOApplFwdCondition.MP6A.RankOrderby, DbType.Int16, ParameterDirection.Input);
                                parameters.Add("@MP6F_ArmyNoPrefix", dTOApplFwdCondition.MP6F.ArmyNoPrefix, DbType.String, ParameterDirection.Input);
                                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(QueryFinal, parameters);
                                int sno = 1;
                                var allrecord = (from e in BasicDetailTempList
                                                 select new DTOBasicDetailTempRequest()
                                                 {
                                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                                     Sno = sno++,
                                                     FName = e.FName,
                                                     LName = e.LName,
                                                     ServiceNo = e.ServiceNo,
                                                     DOB = e.DOB,
                                                     DateOfCommissioning = e.DateOfCommissioning,
                                                     District = e.District,
                                                     PO = e.PO,
                                                     PS = e.PS,
                                                     PinCode = e.PinCode,
                                                     State = e.State,
                                                     Tehsil = e.Tehsil,
                                                     Village = e.Village,
                                                     Observations = e.Observations,
                                                     Remarks2 = e.Remarks2,
                                                     RankName = e.RankName,
                                                     UpdatedOn = e.UpdatedOn,
                                                     RegistrationId = e.RegistrationId,
                                                     TypeId = e.TypeId,
                                                     ApplyForId = e.ApplyForId


                                                 }).ToList();
                                return await Task.FromResult(allrecord);
                            }
                            else if (dTOBasicDetailTempObsRequest.Name == dTOApplFwdCondition.MP6F.Name) //"MP 6F"
                            {
                                var QueryFinal = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                                                " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                                                " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps Temps" +
                                                " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                                                " left join OROMapping oro on oro.TDMId = @TDMId " +
                                                " WHERE Temps.ApplyForId=1 AND SUBSTRING(UPPER(Temps.ServiceNo),1,2) = @MP6F_ArmyNoPrefix OR Temps.ArmedId in (select value from string_split(oro.ArmedIdList,','))  AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";

                                var parameters = new DynamicParameters();
                                parameters.Add("@TDMId", TDMId);
                                parameters.Add("@MP6F_ArmyNoPrefix", dTOApplFwdCondition.MP6F.ArmyNoPrefix, DbType.String, ParameterDirection.Input);

                                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(QueryFinal, parameters);
                                int sno = 1;
                                var allrecord = (from e in BasicDetailTempList
                                                 select new DTOBasicDetailTempRequest()
                                                 {
                                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                                     Sno = sno++,
                                                     FName = e.FName,
                                                     LName = e.LName,
                                                     ServiceNo = e.ServiceNo,
                                                     DOB = e.DOB,
                                                     DateOfCommissioning = e.DateOfCommissioning,
                                                     District = e.District,
                                                     PO = e.PO,
                                                     PS = e.PS,
                                                     PinCode = e.PinCode,
                                                     State = e.State,
                                                     Tehsil = e.Tehsil,
                                                     Village = e.Village,
                                                     Observations = e.Observations,
                                                     Remarks2 = e.Remarks2,
                                                     RankName = e.RankName,
                                                     UpdatedOn = e.UpdatedOn,
                                                     RegistrationId = e.RegistrationId,
                                                     TypeId = e.TypeId,
                                                     ApplyForId = e.ApplyForId


                                                 }).ToList();
                                return await Task.FromResult(allrecord);
                            }
                            else
                            {
                                var QueryFinal = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                                                " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                                                " ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps Temps" +
                                                " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                                                " left join OROMapping oro on oro.TDMId = @TDMId " +
                                                " WHERE Temps.ApplyForId=1 AND ranks1.Orderby > @MP6A_RankOrderby AND SUBSTRING(UPPER(Temps.ServiceNo),1,2) != @MP6F_ArmyNoPrefix AND Temps.ArmedId in (select value from string_split(oro.ArmedIdList,','))  AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";
                                
                                var parameters = new DynamicParameters();
                                parameters.Add("@TDMId", TDMId);
                                parameters.Add("@MP6F_ArmyNoPrefix", dTOApplFwdCondition.MP6F.ArmyNoPrefix, DbType.String, ParameterDirection.Input);
                                parameters.Add("@MP6A_RankOrderby", dTOApplFwdCondition.MP6A.RankOrderby, DbType.Int16, ParameterDirection.Input);

                                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(QueryFinal, parameters);
                                int sno = 1;
                                var allrecord = (from e in BasicDetailTempList
                                                 select new DTOBasicDetailTempRequest()
                                                 {
                                                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                                     Sno = sno++,
                                                     FName = e.FName,
                                                     LName = e.LName,
                                                     ServiceNo = e.ServiceNo,
                                                     DOB = e.DOB,
                                                     DateOfCommissioning = e.DateOfCommissioning,
                                                     District = e.District,
                                                     PO = e.PO,
                                                     PS = e.PS,
                                                     PinCode = e.PinCode,
                                                     State = e.State,
                                                     Tehsil = e.Tehsil,
                                                     Village = e.Village,
                                                     Observations = e.Observations,
                                                     Remarks2 = e.Remarks2,
                                                     RankName = e.RankName,
                                                     UpdatedOn = e.UpdatedOn,
                                                     RegistrationId = e.RegistrationId,
                                                     TypeId = e.TypeId,
                                                     ApplyForId = e.ApplyForId


                                                 }).ToList();
                                return await Task.FromResult(allrecord);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailTempDB->GetALLBasicDetailTemp");
                return new List<DTOBasicDetailTempRequest>();
            }

        }

        /// <summary>
        /// Retrieves the detailed basic information for a specific BasicDetailTempId based on the UserId and BasicDetailId.
        /// </summary>
        /// <param name="UserId">The ID of the user requesting the details.</param>
        /// <param name="BasicDetailId">The ID of the specific basic detail entry.</param>
        /// <returns>A DTOBasicDetailTempRequest object containing the requested basic detail information.</returns>
        public async Task<DTOBasicDetailTempRequest?> GetALLBasicDetailTempByBasicDetailId(int UserId, int BasicDetailId)
        {
            //var BasicDetailTempList = _context.BasicDetailTemps.Where(x => x.Updatedby == UserId).ToList();
            var query = "SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.NameAsPerRecord,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds " +
                        " ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2" +
                        " ,mappy.Name ApplyType,reg.Name RegistrationName,micard.Name CardType" +
                        " ,users.DomainId,unit.UnitName,unit.Suffix,unit.Sus_no,pro.Name OffName,ranks.RankAbbreviation,pro.ArmyNo,Temps.UpdatedOn " +
                        " FROM BasicDetailTemps  Temps" +
                        " inner join MApplyFor mappy on mappy.ApplyForId=Temps.ApplyForId" +
                        " inner join MRegistration reg on Temps.ApplyForId=reg.RegistrationId" +
                        " inner join MICardType micard on Temps.TypeId=micard.TypeId" +
                        " inner join AspNetUsers users on users.Id = Temps.Updatedby" +
                        " inner join TrnDomainMapping trn on trn.AspNetUsersId = users.Id" +
                        " inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId" +
                        " inner join MUnit unit on unit.UnitId = mapuni.UnitId" +
                        " left join UserProfile pro on pro.UserId = trn.UserId" +
                        " inner join MRank ranks on ranks.RankId = pro.RankId"+
                        " inner join MRank ranks1 on ranks1.RankId = Temps.RankId" +
                        " WHERE Temps.BasicDetailTempId=@BasicDetailId ORDER BY Temps.UpdatedOn DESC"; //Temps.Updatedby=@UserId and 
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(query, new { UserId, BasicDetailId });
                    int sno = 1;
                    var allrecord = (from e in BasicDetailTempList
                                     select new DTOBasicDetailTempRequest()
                                     {
                                         EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                                         Sno = sno++,
                                         FName = e.FName,
                                         LName = e.LName,
                                         ServiceNo = e.ServiceNo,
                                         DOB = e.DOB,
                                         DateOfCommissioning = e.DateOfCommissioning,
                                         District = e.District,
                                         PO = e.PO,
                                         PS = e.PS,
                                         PinCode = e.PinCode,
                                         State = e.State,
                                         Tehsil = e.Tehsil,
                                         Village = e.Village,
                                         Observations = e.Observations,
                                         Remarks2 = e.Remarks2,
                                         ApplyType = e.ApplyType,
                                         RegistrationName = e.RegistrationName,
                                         CardType = e.CardType,
                                         DomainId = e.DomainId,
                                         UnitName = e.UnitName,
                                         Suffix = e.Suffix,
                                         Sus_no = e.Sus_no,
                                         OffName = e.OffName,
                                         RankAbbreviation = e.RankAbbreviation,
                                         ArmyNo = e.ArmyNo,
                                         RankName = e.RankName,
                                         UpdatedOn = e.UpdatedOn

                                     }).FirstOrDefault();
                    return await Task.FromResult(allrecord);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailTempDB->GetALLBasicDetailTempByBasicDetailId");
                return null;
            }
        }

        /// <summary>
        /// Retrieves a BasicDetailTemp entry based on the provided Army Number.
        /// </summary>
        /// <param name="ArmyNo">The Army number for which the basic detail is fetched.</param>
        /// <returns>A BasicDetailTemp object if found, otherwise null.</returns>
        public async Task<BasicDetailTemp?> GetByArmyNo(string ArmyNo)
        {
            var query = "SELECT * FROM BasicDetailTemps where ServiceNo=@ArmyNo";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    var BasicDetailTempList = await connection.QueryAsync<BasicDetailTemp>(query, new { ArmyNo });
                    return BasicDetailTempList.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailTempDB->GetByArmyNo");
                return null;
            }
        }

        /// <summary>
        /// Marks a BasicDetailTemp entry as inactive based on the provided Army Number.
        /// </summary>
        /// <param name="ArmyNo">The Army number of the record to be updated.</param>
        /// <returns>A boolean indicating if the update was successful.</returns>
        public async Task<bool> UpdateByArmyNo(string ArmyNo)
        {
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {
                    await connection.ExecuteAsync("UPDATE BasicDetailTemps SET IsActive=0 WHERE ServiceNo=@ArmyNo", new { ArmyNo });
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailTempDB->UpdateByArmyNo");
                return false;
            }
        }
    }
}
