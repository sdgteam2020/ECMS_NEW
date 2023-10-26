using BusinessLogicsLayer.User;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Master
{ 
    public class ArmedBL : GenericRepositoryDL<MArmedType>, IArmedBL
    {
        private readonly IArmedDB _iArmedDB;

        public ArmedBL(ApplicationDbContext context, IArmedDB iArmedDB) : base(context)
        {
            _iArmedDB = iArmedDB;   
        }

        public Task<bool> GetByName(MArmedType Dto)
        {
            Dto.ArmedName = Dto.ArmedName.Trim().TrimEnd().TrimStart();    
           return _iArmedDB.GetByName(Dto);   
        }
    }
}
