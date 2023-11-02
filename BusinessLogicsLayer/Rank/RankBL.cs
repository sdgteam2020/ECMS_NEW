using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogicsLayer.BdeCate
{
    public class RankBL : GenericRepositoryDL<MRank>, IRankBL
    {
       

        public RankBL(ApplicationDbContext context) : base(context)
        {
           
        }

       
    }
}
