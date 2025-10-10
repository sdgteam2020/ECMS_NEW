using BusinessLogicsLayer.Bde;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;

namespace BusinessLogicsLayer.BdeCate
{
    public class NotificationBL : GenericRepositoryDL<MTrnNotification>, INotificationBL
    {


        private readonly INotificationDB _iNotificationDB;

        public NotificationBL(ApplicationDbContext context, INotificationDB iNotificationDB) : base(context)
        {
            _iNotificationDB = iNotificationDB;
        }

        public async Task<bool> UpdatePrevious(MTrnNotification Data)
        {
            return await _iNotificationDB.UpdatePrevious(Data);
        }

        public async Task<bool> UpdateRead(MTrnNotification Data)
        {
            return await _iNotificationDB.UpdateRead(Data);
        }
    }
}
