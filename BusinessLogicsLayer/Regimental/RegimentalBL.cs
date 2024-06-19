using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class RegimentalBL : GenericRepositoryDL<MRegimental>, IRegimentalBL
    {
        private readonly IRegimentalDB _RegimentalDB;

        public RegimentalBL(ApplicationDbContext context, IRegimentalDB iRegimentalDB) : base(context)
        {
            _RegimentalDB = iRegimentalDB;   
        }

        public Task<List<DTORegimentalResponse>> GetAllData()
        {
            return _RegimentalDB.GetAllData();
        }
        public Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId)
        {
            return _RegimentalDB.GetByArmedId(ArmedId);
        }

        public Task<bool> GetByName(MRegimental Dto)
        {
            Dto.Name = Dto.Name.Trim().TrimEnd().TrimStart();    
           return _RegimentalDB.GetByName(Dto);   
        }
    }
}
