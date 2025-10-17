using DataAccessLayer;
using DataTransferObject.Domain.Model;

namespace BusinessLogicsLayer.Bde
{
    public interface INotificationBL : IGenericRepositoryDL<MTrnNotification>
    {

        public Task<bool> UpdateRead(MTrnNotification Data);
        public Task<bool> UpdatePrevious(MTrnNotification Data);
    }
}
