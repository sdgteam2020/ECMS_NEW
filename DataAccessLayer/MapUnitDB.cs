﻿using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer
{
    public class MapUnitDB : GenericRepositoryDL<MapUnit>, IMapUnitDB
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MapUnitDB> _logger;
        public MapUnitDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MapUnitDB> logger) : base(context)
        {
            _logger = logger;
            _contextDP = contextDP;
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MapUnit Data)
        {
            var ret = _context.MapUnit.Any(p => p.UnitId == Data.UnitId && p.UnitMapId!=Data.UnitMapId);
            return ret;
        }
        public async Task<bool?> FindUnitId(int UnitId)
        {
            try
            {
                var ret = await _context.MapUnit.AnyAsync(p => p.UnitId == UnitId);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->FindUnitId");
                return null;
            }

        }
        public async Task<bool?> FindUnitIdMapped(int UnitId,int UnitMapId)
        {
            try
            {
                var ret = await _context.MapUnit.AnyAsync(p => p.UnitId == UnitId && p.UnitMapId != UnitMapId);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->FindUnitIdMapped");
                return null;
            }
        }

        public Task<List<DTOMapUnitResponse>> GetALLUnit(DTOMHierarchyRequest unit, string Unit1)
        {
            Unit1 = string.IsNullOrEmpty(Unit1) ? "" : Unit1.ToLower();
            if (unit.ComdId == null)unit.ComdId = 0;
            if (unit.CorpsId == null)unit.CorpsId = 0;
            if (unit.DivId == null)unit.DivId = 0;
            if (unit.BdeId == null)unit.BdeId = 0;

            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = (from uni in _context.MapUnit
            //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
            //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
            //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
            //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
            
            join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd on uni.ComdId equals Com.ComdId
                    //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                       join pso in _context.MPso on uni.PsoId equals pso.PsoId
                       join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
                       join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
                       where MUni.Sus_no == "" ||  MUni.Sus_no.ToLower().Contains(Unit1)//MUni.UnitName == "" ||

                       select new DTOMapUnitResponse
                       { 
                           UnitMapId= uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           IsVerify = MUni.IsVerify,
                           UnitId = uni.UnitId,
                           BdeId = bde.BdeId,  
                           BdeName = bde.BdeName,   
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix=MUni.Suffix,
                           Sus_no=MUni.Sus_no,
                           UnitType = uni.UnitType,
                           PsoId=pso.PsoId,
                           PSOName=pso.PSOName,
                           FmnBranchID= FmnBranch.FmnBranchID,
                           BranchName= FmnBranch.BranchName,
                           SubDteId=SubDte.SubDteId,
                           SubDteName=SubDte.SubDteName,
                       }
                     ).Distinct().OrderByDescending(x=>x.UnitMapId).Take(200).ToList(); ;




            return Task.FromResult(Div);
        }

        public Task<List<DTOMapUnitResponse>> GetALLByUnitName(string UnitName)
        {
            var Div = (from uni in _context.MapUnit
                           //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
                           //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
                           //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
                           //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       where MUni.Sus_no.ToLower().Contains(UnitName.ToLower()) && MUni.IsVerify==true
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           Suffix=MUni.Suffix,
                           Sus_no=MUni.Sus_no,
                       }
                     ).Distinct().Take(5).ToList(); ;
            return Task.FromResult(Div);
        }

        public Task<DTOMapUnitResponse> GetALLByUnitMapId(int UnitMapId)
        {
            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div = (from uni in _context.MapUnit
                           //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
                           //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
                           //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
                           //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd on uni.ComdId equals Com.ComdId
                       //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                       join pso in _context.MPso on uni.PsoId equals pso.PsoId
                       join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
                       join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
                       where uni.UnitMapId==UnitMapId
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitAbbreviation= MUni.Abbreviation,
                           UnitId = uni.UnitMapId,
                           BdeId = bde.BdeId,
                           BdeName = bde.BdeName,
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix = MUni.Suffix,
                           Sus_no = MUni.Sus_no,
                           UnitType = uni.UnitType,
                           PsoId = pso.PsoId,
                           PSOName = pso.PSOName,
                           FmnBranchID = FmnBranch.FmnBranchID,
                           BranchName = FmnBranch.BranchName,
                           SubDteId = SubDte.SubDteId,
                           SubDteName = SubDte.SubDteName,
                       }
                     ).Distinct().SingleOrDefault() ;




            return Task.FromResult(Div);
        }
        public async Task<DTOMapUnitResponse> GetALLByUnitById(int UnitId)
        {


            //on new { Div.UnitId, a.Years_Months } equals new { c.UnitId, c.Years_Months }
            var Div =await (from uni in _context.MapUnit
                           //where (unit.ComdId == 0 ? uni.ComdId == uni.ComdId : uni.ComdId == unit.ComdId)
                           //&& (unit.CorpsId == 0 ? uni.CorpsId == uni.CorpsId : uni.CorpsId == unit.CorpsId)
                           //&& (unit.DivId == 0 ? uni.DivId == uni.DivId : uni.DivId == unit.DivId)
                           //&& (unit.BdeId == 0 ? uni.BdeId == uni.BdeId : uni.BdeId == unit.BdeId)
                       join MUni in _context.MUnit on uni.UnitId equals MUni.UnitId
                       join Com in _context.MComd
                       on uni.ComdId equals Com.ComdId
                       //   on new { uni.ComdId } equals new { Com.ComdId }
                       join cor in _context.MCorps on uni.CorpsId equals cor.CorpsId
                       join div in _context.MDiv on uni.DivId equals div.DivId
                       join bde in _context.MBde on uni.BdeId equals bde.BdeId
                            join pso in _context.MPso on uni.PsoId equals pso.PsoId
                            join FmnBranch in _context.MFmnBranches on uni.FmnBranchID equals FmnBranch.FmnBranchID
                            join SubDte in _context.MSubDte on uni.SubDteId equals SubDte.SubDteId
                            where MUni.UnitId == UnitId
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
                           UnitId = uni.UnitId,
                           BdeId = bde.BdeId,
                           BdeName = bde.BdeName,
                           DivId = div.DivId,
                           DivName = div.DivName,
                           CorpsId = cor.CorpsId,
                           CorpsName = cor.CorpsName,
                           ComdName = Com.ComdName,
                           ComdId = Com.ComdId,
                           Suffix = MUni.Suffix,
                           Sus_no = MUni.Sus_no,
                           UnitType = uni.UnitType,
                           PsoId = pso.PsoId,
                           PSOName = pso.PSOName,
                           FmnBranchID = FmnBranch.FmnBranchID,
                           BranchName = FmnBranch.BranchName,
                           SubDteId = SubDte.SubDteId,
                           SubDteName = SubDte.SubDteName,
                       }
                     ).Distinct().SingleOrDefaultAsync();





            return (Div);
        }

        public async Task<bool?> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (dTO.UnitMapId == 0)
                    {
                        MUnit? mUnit = await _context.MUnit.FindAsync(dTO.UnitId);
                        if(mUnit!=null)
                        {
                            var mapUnit = new MapUnit
                            {
                                UnitId = mUnit.UnitId,
                                UnitType = dTO.UnitType,
                                ComdId = dTO.ComdId,
                                CorpsId = dTO.CorpsId,
                                DivId = dTO.DivId,
                                BdeId = dTO.BdeId,
                                FmnBranchID = dTO.FmnBranchID,
                                PsoId = dTO.PsoId,
                                SubDteId = dTO.SubDteId,
                                IsActive = true,
                                Updatedby = dTO.Updatedby,
                                UpdatedOn = dTO.UpdatedOn,
                            };
                            await _context.MapUnit.AddAsync(mapUnit);
                            await _context.SaveChangesAsync();

                            mUnit.IsVerify = dTO.IsVerify;
                            await _context.SaveChangesAsync();
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MUnit? mUnit = await _context.MUnit.FindAsync(dTO.UnitId);
                        if (mUnit != null)
                        {
                            MapUnit? mapUnit = await _context.MapUnit.FindAsync(dTO.UnitMapId);
                            if(mapUnit!=null)
                            {

                                mapUnit.UnitId = mUnit.UnitId;
                                mapUnit.UnitType = dTO.UnitType;
                                mapUnit.ComdId = dTO.ComdId;
                                mapUnit.CorpsId = dTO.CorpsId;
                                mapUnit.DivId = dTO.DivId;
                                mapUnit.BdeId = dTO.BdeId;
                                mapUnit.FmnBranchID = dTO.FmnBranchID;
                                mapUnit.PsoId = dTO.PsoId;
                                mapUnit.SubDteId = dTO.SubDteId;
                                mapUnit.IsActive = true;
                                mapUnit.Updatedby = dTO.Updatedby;  
                                mapUnit.UpdatedOn = dTO.UpdatedOn;

                                _context.MapUnit.Update(mapUnit);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                return false;
                            }
                            
                            mUnit.IsVerify = dTO.IsVerify;
                            await _context.SaveChangesAsync();
                            
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(1001, ex, "MapUnitDB->SaveUnitWithMapping");
                    return null;
                }
            }
        }
        public async Task<DTOUnitMapIdCheckInFKTableResponse?> UnitMapIdCheckInFKTable(int UnitMapId)
        {
            try
            {
                string query = "Select count(distinct bd.BasicDetailId) as TotalBD, count(distinct mro.RecordOfficeId) as TotalRO, count(distinct tdm.Id) as TotalTDM, count(distinct tfwd.TrnFwdId) as TotalTF,count(distinct tpo.Id)as TotalTPOFrom,count(distinct tpo_.Id)as TotalTPOTo from MapUnit munit" +
                                " left join BasicDetails bd on bd.UnitId = munit.UnitMapId " +
                                " left join MRecordOffice mro on mro.UnitId =  munit.UnitMapId " +
                                " left join TrnDomainMapping tdm on tdm.UnitId = munit.UnitMapId " +
                                " left join TrnFwds tfwd on tfwd.UnitId = munit.UnitMapId " +
                                " left join TrnPostingOut tpo on tpo.FromUnitID= munit.UnitMapId " +
                                " left join TrnPostingOut tpo_ on tpo_.ToUnitID= munit.UnitMapId " +
                                " where munit.UnitMapId=@UnitMapId";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOUnitMapIdCheckInFKTableResponse>(query, new { UnitMapId });
                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->UnitMapIdCheckInFKTable");
                return null;
            }
        }

        public async Task<List<DTOUnitResponse>> GetUnitByHierarchy(DTOMHierarchyRequest Data)
        {
            try
            {
                string query = "SELECT Map.UnitMapId UnitId,unt.UnitName,unt.Suffix,unt.Sus_no FROM MapUnit Map " +
                                " Inner join MUnit unt on Map.UnitId = unt.UnitId" +
                                " where Map.ComdId = ISNULL(@ComdId,Map.ComdId)" +
                                " AND Map.CorpsId = ISNULL(@CorpsId,Map.CorpsId)" +
                                " AND Map.DivId = ISNULL(@DivId,Map.DivId)" +
                                " AND Map.BdeId = ISNULL(@BdeId,Map.BdeId)";
                                //" AND Map.FmnBranchID = ISNULL(@FmnBranchID,Map.FmnBranchID)" +
                                //" AND Map.PsoId = ISNULL(@PsoId,Map.PsoId)" +
                                //" AND Map.SubDteId = ISNULL(@SubDteId,Map.SubDteId)";

                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOUnitResponse>(query, new { 
                        Data.ComdId, 
                        Data.CorpsId ,
                        Data.DivId,
                        Data.BdeId,
                        Data.FmnBranchID,
                        Data.PsoId,
                        Data.SubDteId
                    });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->GetUnitByHierarchy");
                return null;
            }
        }

        public async Task<List<DTOUnitResponse>> GetUnitByHierarchyForIcardRequest(DTOMHierarchyRequest Data)
        {
            try
            {
                string query = " SELECT distinct munit.UnitMapId UnitId FROM TrnICardRequest trnicrd" +
                               " inner join BasicDetails B on trnicrd.BasicDetailId = B.BasicDetailId" +
                               " inner join TrnDomainMapping map on map.Id= trnicrd.TrnDomainMappingId" +
                               " inner join MapUnit munit on map.UnitId=munit.UnitMapId" +
                               " where munit.ComdId=ISNULL(@ComdId,munit.ComdId) " +
                               " and munit.CorpsId=ISNULL(@CorpsId,munit.CorpsId)" +
                               " and munit.DivId=ISNULL(@DivId,munit.DivId)" +
                               " and munit.BdeId=ISNULL(@BdeId,munit.BdeId)" +
                               " and munit.UnitMapId=ISNULL(@UnitId,munit.UnitMapId)";


                using (var connection = _contextDP.CreateConnection())
                {
                    var ret = await connection.QueryAsync<DTOUnitResponse>(query, new { Data.ComdId, Data.CorpsId, Data.DivId, Data.BdeId, Data.UnitMapId });
                    return ret.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitDB->GetUnitByHierarchy");
                return null;
            }
        }
    }
 }
