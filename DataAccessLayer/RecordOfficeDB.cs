using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RecordOfficeDB : GenericRepositoryDL<MRecordOffice>, IRecordOfficeDB
    {
        protected new readonly ApplicationDbContext _context;
        public RecordOfficeDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly IConfiguration configuration;

        public async Task<int> GetByName(MRecordOffice Dto)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            if (mRecordOffices.Any(x => (x.Name.ToUpper() == Dto.Name.ToUpper() || x.Abbreviation.ToUpper() == Dto.Abbreviation.ToUpper()) && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 2;
            }
            else if (mRecordOffices.Any(x => x.ArmedId == Dto.ArmedId && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 3;
            }
            else if (mRecordOffices.Any(x => x.TDMId == Dto.TDMId && x.RecordOfficeId != Dto.RecordOfficeId))
            {
                return 4;
            }
            else
            {
                return 1;
            }
        }
        public async Task<bool> GetByTDMId(int TDMId)
        {
            List<MRecordOffice> mRecordOffices = await _context.MRecordOffice.AsNoTracking().ToListAsync();
            var result = mRecordOffices.Any(x => x.TDMId == TDMId);
            return result;
        }

        public Task<List<DTORecordOfficeResponse>> GetAllData()
        {
            var Corps = (from c in _context.MRecordOffice
                         join d in _context.MArmedType
                         on c.ArmedId equals d.ArmedId

                         select new DTORecordOfficeResponse
                         {
                             RecordOfficeId = c.RecordOfficeId,
                             Name = c.Name,
                             Abbreviation = c.Abbreviation,
                             ArmedId = c.ArmedId,
                             ArmedName = d.ArmedName,
                             TDMId = c.TDMId,
                         }).ToList();



            return Task.FromResult(Corps);
        }
    }
}
