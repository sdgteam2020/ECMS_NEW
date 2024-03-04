using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Logger;
using Dapper;

namespace DataAccessLayer
{
    public class UnitDB : GenericRepositoryDL<MUnit>, IUnitDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly ILogger<UnitDB> _logger;
        public UnitDB(ApplicationDbContext context, ILogger<UnitDB> logger, DapperContext contextDP) : base(context)
        {
            _context = context;
            _logger = logger;
            _contextDP = contextDP;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MUnit Data)
        {
            var ret = _context.MUnit.Any(p => p.Sus_no.ToUpper() == Data.Sus_no.ToUpper() && p.UnitId !=Data.UnitId);
            return ret;
        }
        public async Task<bool> FindSusNo(string Sus_no)
        {
            var ret = _context.MUnit.Any(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no.ToUpper());
            return ret;
        }

        public async Task<MUnit?> GetBySusNo(string Sus_no)
        {
            try
            {
                return await _context.MUnit.Where(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetBySusNo");
                return null;
            }
        }
        public async Task<bool?> GetBySusNoWithUnitId(string Sus_no, int UnitId)
        {
            try
            {
                return await _context.MUnit.AnyAsync(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no && x.UnitId != UnitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetBySusNoWithUnitId");
                return null;
            }
        }

        public async Task<List<MUnit>> GetAllUnit(string UnitName)
        {
            try
            {
                //UnitName = string.IsNullOrEmpty(UnitName) ? "" : UnitName.ToLower();
                //string query = "";
                //if(UnitName!="")
                // query = " declare @UnitName varchar(Max)='"+ UnitName.ToUpper() + "' "+
                //                " SELECT  [UnitId] ,[Sus_no],[Suffix],CONVERT(varchar(Max),[UnitName]) UnitName,[Abbreviation],[IsVerify],[IsActive]"+
                //                " ,[Updatedby],[UpdatedOn],[UnregdUserId] "+
                //                " FROM   dbo.MUnit where CONVERT(varchar(Max),[UnitName])=@UnitName";
                //else
                //{
                //    query = " SELECT  [UnitId] ,[Sus_no],[Suffix],CONVERT(varchar(Max),[UnitName]) UnitName,[Abbreviation],[IsVerify],[IsActive]" +
                //               " ,[Updatedby],[UpdatedOn],[UnregdUserId] " +
                //               " FROM   dbo.MUnit";
                //}

               
                //using (var connection = _contextDP.CreateConnection())
                //{
                //    var basicDetail = await connection.QueryAsync<MUnit>(query, new { });
                //    if (basicDetail != null)
                //    {
                //        return basicDetail.ToList();
                //    }
                //    else
                //    {
                //        return null;
                //    }
                //}

                //var ret = await _context.MUnit.Where(P => UnitName == "" || P.UnitName.ToLower().Contains(UnitName)).Take(200).ToListAsync();



                var ret = await _context.MUnit.Take(200).ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetAllUnit");
                return null;
            }
        }
        public async Task<List<DTOUnitResponse>?> GetTopBySUSNo(string SUSNo)
        {
            try
            {
                var Unit = await (from unit in _context.MUnit.Where(x => (x.Sus_no + x.Suffix).Contains(SUSNo))
                                  select new DTOUnitResponse
                                  {
                                      UnitId = unit.UnitId,
                                      Sus_no = unit.Sus_no + unit.Suffix,
                                      UnitName = unit.UnitName,
                                      Abbreviation = unit.Abbreviation,
                                      IsVerify = unit.IsVerify,
                                  }).Take(5).ToListAsync();
                return Unit;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetTopBySUSNo");
                return null;
            }

        }
    }
}