using Dapper;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
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
        public async Task<List<DTOBasicDetailTempRequest>> GetALLBasicDetailTemp(int UserId, bool claim, short ArmedIdForORO, int typeId)
        {
            try
            {
                if (typeId == 1)
                {
                    var query = @"SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds 
                                ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2
                                ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps  Temps
                                inner join MRank ranks1 on ranks1.RankId = Temps.RankId
                                WHERE Temps.Updatedby=@UserId AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";
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
                    if (claim)
                    {
                        var query = @"SELECT 
                                        CASE 
                                            WHEN RECO.TDMId IS NOT NULL THEN RECO.TDMId
                                            ELSE ORO.TDMId
                                        END AS TDMId,
                                        TDM.AspNetUsersId,
                                        RECO.RecordOfficeId,
                                        RECO.Name,
                                        RECO.ArmedId
                                    FROM MRecordOffice RECO
                                    LEFT JOIN OROMapping ORO 
                                        ON RECO.RecordOfficeId = ORO.RecordOfficeId
                                    LEFT JOIN TrnDomainMapping TDM
                                        ON TDM.Id = CASE 
                                                        WHEN RECO.TDMId IS NOT NULL THEN RECO.TDMId
                                                        ELSE ORO.TDMId
                                                    END
                                    WHERE TDM.AspNetUsersId = @UserId";
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
                                var QueryFinal = @"SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds 
                                                ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2
                                                ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps Temps
                                                inner join MRank ranks1 on ranks1.RankId = Temps.RankId
                                                WHERE Temps.RecordOfficeId =@RecordOfficeId AND Temps.ApplyForId = 2 AND Temps.IsActive = 1 ORDER BY Temps.UpdatedOn DESC";

                                var BasicDetailTempList = await connection.QueryAsync<DTOBasicDetailTempRequest>(QueryFinal, new { dTOBasicDetailTempObsRequest.RecordOfficeId });
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
                                var QueryFinal = @"SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds 
                                                ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2
                                                ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps Temps
                                                INNER JOIN MRank ranks1 on ranks1.RankId = Temps.RankId
                                                WHERE Temps.RecordOfficeId=@RecordOfficeId AND Temps.ApplyForId=1 AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";

                                var parameters = new DynamicParameters();
                                parameters.Add("@RecordOfficeId", dTOBasicDetailTempObsRequest.RecordOfficeId);

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
                    else
                    {
                        var query = @"SELECT Temps.BasicDetailTempId,ranks1.RankAbbreviation RankName,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds 
                                ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2
                                ,Temps.UpdatedOn,Temps.RegistrationId,Temps.TypeId,Temps.ApplyForId FROM BasicDetailTemps  Temps
                                inner join MRank ranks1 on ranks1.RankId = Temps.RankId
                                WHERE Temps.Updatedby=@UserId AND Temps.IsActive=1 ORDER BY Temps.UpdatedOn DESC";
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
        public async Task<DTOGenericResponse<DTOBasicDetailTempRequest?>> GetALLBasicDetailTempByBasicDetailId(int AspNetUsersId, int BasicDetailId, bool claim)
        {
            var response = new DTOGenericResponse<DTOBasicDetailTempRequest?>();
            DTOBasicDetailTempRequest? BasicDetailTemp = null;

            var query = @"SELECT Temps.BasicDetailTempId,Temps.RecordOfficeId,ranks1.RankAbbreviation RankName,Temps.NameAsPerRecord,Temps.FName,Temps.LName,Temps.ServiceNo,Temps.DOB,Temps.DateOfCommissioning,Temps.District,Temps.PO,Temps.PS,Temps.PinCode,Temps.State,Temps.Tehsil,Temps.Village,Temps.Observations,Temps.RemarksIds 
                            ,(select STRING_AGG(Remarks,'#') from MRemarks where RemarksId in (select value from string_split(Temps.RemarksIds,','))) Remarks2
                            ,mappy.Name ApplyType,reg.Name RegistrationName,micard.Name CardType
                            ,users.DomainId,unit.UnitName,unit.Suffix,unit.Sus_no,pro.Name OffName,ranks.RankAbbreviation,pro.ArmyNo,Temps.Updatedby,Temps.UpdatedOn 
                            FROM BasicDetailTemps  Temps
                            inner join MApplyFor mappy on mappy.ApplyForId=Temps.ApplyForId
                            inner join MRegistration reg on Temps.ApplyForId=reg.RegistrationId
                            inner join MICardType micard on Temps.TypeId=micard.TypeId
                            inner join AspNetUsers users on users.Id = Temps.Updatedby
                            inner join TrnDomainMapping trn on trn.AspNetUsersId = users.Id
                            inner join MapUnit mapuni on mapuni.UnitMapId = trn.UnitId
                            inner join MUnit unit on unit.UnitId = mapuni.UnitId
                            left join UserProfile pro on pro.UserId = trn.UserId
                            inner join MRank ranks on ranks.RankId = pro.RankId
                            inner join MRank ranks1 on ranks1.RankId = Temps.RankId
                            WHERE Temps.BasicDetailTempId=@BasicDetailId";
            try
            {
                using (var connection = _contextDP.CreateConnection())
                {

                    if (claim)
                    {
                        var query2 = @"SELECT 
                                        CASE 
                                            WHEN RECO.TDMId IS NOT NULL THEN RECO.TDMId
                                            ELSE ORO.TDMId
                                        END AS TDMId,
                                        TDM.AspNetUsersId,
                                        RECO.RecordOfficeId,
                                        RECO.Name,
                                        RECO.ArmedId
                                    FROM MRecordOffice RECO
                                    LEFT JOIN OROMapping ORO 
                                        ON RECO.RecordOfficeId = ORO.RecordOfficeId
                                    LEFT JOIN TrnDomainMapping TDM
                                        ON TDM.Id = CASE 
                                                        WHEN RECO.TDMId IS NOT NULL THEN RECO.TDMId
                                                        ELSE ORO.TDMId
                                                    END
                                    WHERE TDM.AspNetUsersId = @AspNetUsersId";

                        DTOBasicDetailTempObsRequest? dTOBasicDetailTempObsRequest = await connection.QueryFirstOrDefaultAsync<DTOBasicDetailTempObsRequest>(query2, new { AspNetUsersId });
                        if (dTOBasicDetailTempObsRequest == null)
                        {
                            response.Result = false;
                            response.Message = "You are not authorized to view this record.";
                        }
                        else
                        {
                            BasicDetailTemp = await connection.QueryFirstOrDefaultAsync<DTOBasicDetailTempRequest>(query, new { BasicDetailId });
                            if (BasicDetailTemp == null)
                            {
                                response.Result = false;
                                response.Message = "Invalid Id.";
                            }
                            else
                            {
                                if (BasicDetailTemp.RecordOfficeId == dTOBasicDetailTempObsRequest.RecordOfficeId)
                                {
                                    response.Result = true;
                                    response.Message = "valid";
                                }
                                else
                                {
                                    response.Result = false;
                                    response.Message = "You are not authorized to view this record.";

                                }
                            }
                        }
                        response.Value = BasicDetailTemp;
                    }
                    else
                    {
                        BasicDetailTemp = await connection.QueryFirstOrDefaultAsync<DTOBasicDetailTempRequest>(query, new { BasicDetailId });
                        if (BasicDetailTemp == null)
                        {
                            response.Result = false;
                            response.Message = "Invalid Id.";
                        }
                        else
                        {
                            if (BasicDetailTemp.Updatedby == AspNetUsersId)
                            {
                                response.Result = true;
                                response.Message = "valid";
                            }
                            else
                            {
                                response.Result = false;
                                response.Message = "You are not authorized to view this record.";

                            }
                        }
                        response.Value = BasicDetailTemp;
                    }

                    return response;


                    //int sno = 1;
                    //var allrecord = (from e in BasicDetailTemp
                    //                 select new DTOBasicDetailTempRequest()
                    //                 {
                    //                     EncryptedId = protector.Protect(e.BasicDetailTempId.ToString()),
                    //                     Sno = sno++,
                    //                     FName = e.FName,
                    //                     LName = e.LName,
                    //                     ServiceNo = e.ServiceNo,
                    //                     DOB = e.DOB,
                    //                     DateOfCommissioning = e.DateOfCommissioning,
                    //                     District = e.District,
                    //                     PO = e.PO,
                    //                     PS = e.PS,
                    //                     PinCode = e.PinCode,
                    //                     State = e.State,
                    //                     Tehsil = e.Tehsil,
                    //                     Village = e.Village,
                    //                     Observations = e.Observations,
                    //                     Remarks2 = e.Remarks2,
                    //                     ApplyType = e.ApplyType,
                    //                     RegistrationName = e.RegistrationName,
                    //                     CardType = e.CardType,
                    //                     DomainId = e.DomainId,
                    //                     UnitName = e.UnitName,
                    //                     Suffix = e.Suffix,
                    //                     Sus_no = e.Sus_no,
                    //                     OffName = e.OffName,
                    //                     RankAbbreviation = e.RankAbbreviation,
                    //                     ArmyNo = e.ArmyNo,
                    //                     RankName = e.RankName,
                    //                     UpdatedOn = e.UpdatedOn

                    //                 }).FirstOrDefault();
                    //return await Task.FromResult(allrecord);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "BasicDetailTempDB->GetALLBasicDetailTempByBasicDetailId");
                response.Result = false;
                response.Message = "An error occurred while fetching the record.";
                return response;
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
