using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;

namespace DataAccessLayer.BaseInterfaces
{
    public interface INotificationDB : IGenericRepositoryDL<MTrnNotification>
    {

        public Task<bool> UpdateRead(MTrnNotification Data);
        public Task<bool> UpdatePrevious(DTOTrnNotificationRequest Data);
        public Task<bool> AddNotification(DTOTrnNotificationRequest Data);
    }
}
