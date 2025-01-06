using DataAccessLayer;

namespace BusinessLogicsLayer.ArmedCat
{
    public class ArmedCatBL : GenericRepositoryDL<DataTransferObject.Domain.Master.MArmedCat>, IArmedCatBL
    {
       
        public ArmedCatBL(ApplicationDbContext context) : base(context)
        {
            
        }
       
    }
}
