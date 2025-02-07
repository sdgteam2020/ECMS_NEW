using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;


namespace BusinessLogicsLayer.APIData
{
    public class APIDataBL : GenericRepository<MApiData>, IAPIDataBL
    {
        private readonly IAPIDataDB _aPIDataDB;

        public APIDataBL(IAPIDataDB aPIDataDB) 
        {
            _aPIDataDB = aPIDataDB;
        }

        public Task<bool> apiLogin(string accessKey)
        {
           
            return _aPIDataDB.apiLogin(accessKey);
        }

        public Task<DTOApiPersDataResponse> GetByIC(DTOAPIDataRequest Data)
        {
           return _aPIDataDB.GetByIC(Data);
        }

        public Task<DTOApiPersDataResponse> GetByoffrsIC(DTOAPIDataRequest Data)
        {
            return _aPIDataDB.GetByoffrsIC(Data);
        }
    }
}
