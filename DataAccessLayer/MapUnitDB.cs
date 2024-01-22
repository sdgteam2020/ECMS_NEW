using DataAccessLayer.BaseInterfaces;
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

namespace DataAccessLayer
{
    public class MapUnitDB : GenericRepositoryDL<MapUnit>, IMapUnitDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly ILogger<MapUnitDB> _logger;
        public MapUnitDB(ApplicationDbContext context, ILogger<MapUnitDB> logger) : base(context)
        {
            _logger = logger;
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MapUnit Data)
        {
            var ret = _context.MapUnit.Any(p => p.UnitId == Data.UnitId && p.UnitMapId!=Data.UnitMapId);
            return ret;
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
                       where MUni.UnitName == "" || MUni.UnitName.ToLower().Contains(Unit1)
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
                       where MUni.UnitName.Contains(UnitName) && MUni.IsVerify==true
                       select new DTOMapUnitResponse
                       {
                           UnitMapId = uni.UnitMapId,
                           UnitName = MUni.UnitName,
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
                    if (dTO.UnitId > 0 && dTO.UnitMapId == 0)
                    {
                        MUnit? mUnit = await _context.MUnit.FindAsync(dTO.UnitId);
                        if(mUnit!=null)
                        {

                            mUnit.Sus_no = dTO.Sus_no;
                            mUnit.Suffix = dTO.Suffix;
                            mUnit.UnitName = dTO.UnitName;
                            mUnit.IsVerify = dTO.IsVerify;
                            mUnit.IsActive = true;
                            mUnit.Updatedby = dTO.Updatedby;
                            mUnit.UpdatedOn = dTO.UpdatedOn;
                            
                            _context.MUnit.Update(mUnit);
                            await _context.SaveChangesAsync();
                            
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

                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (dTO.UnitId > 0 && dTO.UnitMapId > 0)
                    {
                        MUnit? mUnit = await _context.MUnit.FindAsync(dTO.UnitId);
                        if (mUnit != null)
                        {
                            mUnit.Sus_no = dTO.Sus_no;
                            mUnit.Suffix = dTO.Suffix;
                            mUnit.UnitName = dTO.UnitName;
                            mUnit.IsVerify = dTO.IsVerify;
                            mUnit.IsActive = true;
                            mUnit.Updatedby = dTO.Updatedby;
                            mUnit.UpdatedOn = dTO.UpdatedOn;

                            _context.MUnit.Update(mUnit);
                            await _context.SaveChangesAsync();

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
                        var mUnit = new MUnit
                        {
                            Sus_no = dTO.Sus_no,
                            Suffix = dTO.Suffix,
                            UnitName = dTO.UnitName,
                            IsVerify = dTO.IsVerify,
                            IsActive = true,
                            Updatedby = dTO.Updatedby,
                            UpdatedOn = dTO.UpdatedOn,
                            UnregdUserId = null,
                        };
                        await _context.MUnit.AddAsync(mUnit);
                        await _context.SaveChangesAsync();
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

                        transaction.Commit();
                        return true;
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
    }
 }
