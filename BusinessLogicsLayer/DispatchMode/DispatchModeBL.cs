using DataAccessLayer;
using DataTransferObject.Domain.Master;

namespace BusinessLogicsLayer.DispatchMode
{
    public class DispatchModeBL:GenericRepositoryDL<MDispatchMode>,IDispatchModeBL
    {
        public DispatchModeBL(ApplicationDbContext context):base(context)
        {

        }
    }
}
