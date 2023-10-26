using BusinessLogicsLayer.Master;
using DataAccessLayer.BaseInterfaces;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;

namespace BusinessLogicsLayer.Formation
{
    public class FormationBL : GenericRepositoryDL<MFormation>, IFormationBL
    {
        private readonly IFormationDB _formationDB;

        public FormationBL(ApplicationDbContext context, IFormationDB formationDB) : base(context)
        {
            _formationDB = formationDB;
        }

        public Task<bool> GetByName(MFormation Dto)
        {
            Dto.FormationName = Dto.FormationName.Trim().TrimEnd().TrimStart();
            return _formationDB.GetByName(Dto);
        }
    }
}
