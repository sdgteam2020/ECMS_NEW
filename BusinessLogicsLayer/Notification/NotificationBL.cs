using BusinessLogicsLayer.Bde;
using DapperRepo.Core.Constants;
using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain.Model;
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
