using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;

namespace BusinessLogicsLayer.Bde
{
    public interface INotificationBL : IGenericRepositoryDL<MTrnNotification>
    {

        public Task<bool> UpdateRead(MTrnNotification Data);
        public Task<bool> UpdatePrevious(DTOTrnNotificationRequest Data);
        public Task<bool> AddNotification(DTOTrnNotificationRequest Data);
    }
}
