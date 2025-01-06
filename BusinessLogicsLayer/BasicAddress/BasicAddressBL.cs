using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataTransferObject.Domain.Model;


namespace BusinessLogicsLayer.BdeCate
{
    public class BasicAddressBL : GenericRepositoryDL<MTrnAddress>, IBasicAddressBL
    {


        public BasicAddressBL(ApplicationDbContext context) : base(context)
        {
            
        }

    
    }
}
