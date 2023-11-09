using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class APIDataDB : GenericRepositoryDL<MApiData>, IAPIDataDB
    {
        protected readonly ApplicationDbContext _context;
        public APIDataDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<MApiData> GetByIC(string ICNo)
        {
            var ret=  _context.MApiData.Where(P => P.ServiceNo == ICNo).SingleOrDefault();
            return ret;
        }
    }
}
