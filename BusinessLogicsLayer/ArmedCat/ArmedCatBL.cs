using BusinessLogicsLayer.Master;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.ArmedCat
{
    public class ArmedCatBL : GenericRepositoryDL<DataTransferObject.Domain.Master.MArmedCat>, IArmedCatBL
    {
       
        public ArmedCatBL(ApplicationDbContext context) : base(context)
        {
            
        }
       
    }
}
