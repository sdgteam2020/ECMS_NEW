using DataAccessLayer;
using DataTransferObject.Domain.Master;

namespace BusinessLogicsLayer.ArmedCat
{
    public class ArmedCatBL : GenericRepositoryDL<MArmedCat>, IArmedCatBL
    {
        public ArmedCatBL(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
