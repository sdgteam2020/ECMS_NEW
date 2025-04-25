using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MapUnitChangeDB : GenericRepositoryDL<TrnMapUnitChangeRequest>, IMapUnitChangeDB
    {
        protected new readonly ApplicationDbContext _context;
        protected readonly DapperContext _contextDP;
        private readonly ILogger<MapUnitChangeDB> _logger;
        public MapUnitChangeDB(ApplicationDbContext context, DapperContext contextDP, ILogger<MapUnitChangeDB> logger) : base(context)
        {
            _logger = logger;
            _contextDP = contextDP;
            _context = context;
        }
        public async Task<bool> FindUnitIdMapped(int UnitMapId)
        {
            try
            {
                return await _context.TrnMapUnitChangeRequest.AnyAsync(f => f.UnitMapId == UnitMapId && f.IsComplete == false);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "MapUnitChangeDB->FindUnitIdMapped");
                return false;
            }
        }
    }
}
