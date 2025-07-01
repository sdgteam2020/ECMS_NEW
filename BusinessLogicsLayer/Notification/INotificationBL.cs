using DataAccessLayer;
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

namespace BusinessLogicsLayer.Bde
{
    public interface INotificationBL : IGenericRepositoryDL<MTrnNotification>
    {

        public Task<bool> UpdateRead(MTrnNotification Data);
        public Task<bool> UpdatePrevious(MTrnNotification Data);
    }
}
