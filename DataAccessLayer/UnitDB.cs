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

namespace DataAccessLayer
{
    public class UnitDB : GenericRepositoryDL<MUnit>, IUnitDB
    {
        protected readonly ApplicationDbContext _context;
        private readonly ILogger<UnitDB> _logger;
        public UnitDB(ApplicationDbContext context, ILogger<UnitDB> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MUnit Data)
        {
            var ret = _context.MUnit.Any(p => p.UnitName.ToUpper() == Data.UnitName.ToUpper() && p.UnitId !=Data.UnitId);
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
        public async Task<bool?> GetBySusNoWithUnitId(string Sus_no,int UnitId)
        {
            try
            {
                return await _context.MUnit.AnyAsync(x => (x.Sus_no.ToUpper() + x.Suffix.ToUpper()) == Sus_no && x.UnitId !=UnitId);
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
                UnitName = string.IsNullOrEmpty(UnitName) ? "" : UnitName.ToLower();
                var ret = await _context.MUnit.Where(P => UnitName == "" || P.UnitName.ToLower().Contains(UnitName)).Take(200).ToListAsync();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "UnitDB->GetAllUnit");
                return null;
            }


        }
    }
 }
