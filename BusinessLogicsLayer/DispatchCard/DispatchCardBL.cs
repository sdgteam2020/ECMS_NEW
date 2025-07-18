using DataAccessLayer;
using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.DispatchCard
{
    public class DispatchCardBL:GenericRepositoryDL<TrnDispatchCard>,IDispatchCardBL
    {
        public DispatchCardBL(ApplicationDbContext context):base(context) 
        {

        }
    }
}
