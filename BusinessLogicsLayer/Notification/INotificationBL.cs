using DataAccessLayer;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Bde
{
    public interface INotificationBL : IGenericRepositoryDL<MTrnNotification>
    {

        public Task<bool> UpdateRead(MTrnNotification Data);
        public Task<bool> UpdatePrevious(DTOTrnNotificationRequest Data);
        public Task<bool> AddNotification(DTOTrnNotificationRequest Data);
        public Task<DTODataTablesResponse<DTONotificationResponse>> GetAllNotificationData(DTODataTablesRequestForNotification dTO);
    }
}
