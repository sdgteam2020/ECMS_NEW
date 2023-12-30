using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UnitDB : GenericRepositoryDL<MUnit>, IUnitDB
    {
        protected readonly ApplicationDbContext _context;
        public UnitDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MUnit Data)
        {
            var ret = _context.MUnit.Any(p => p.UnitName.ToUpper() == Data.UnitName.ToUpper() && p.UnitId !=Data.UnitId);
            return ret;
        }

        public async Task<MUnit> GetBySusNo(string Sus_no)
        {
            return _context.MUnit.Where(P => P.Sus_no + P.Suffix == Sus_no).SingleOrDefault();
        }

        public async Task<List<MUnit>> GetAllUnit(string UnitName)
        {
            UnitName = string.IsNullOrEmpty(UnitName) ?"": UnitName.ToLower();
            var ret=  await _context.MUnit.Where(P => UnitName == "" || P.UnitName.ToLower().Contains(UnitName)).Take(200).ToListAsync();
            return ret;

        }
    }
 }
