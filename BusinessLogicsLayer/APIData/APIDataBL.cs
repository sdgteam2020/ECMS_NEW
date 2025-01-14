using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;


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

        public Task<MApiData?> GetByIC(DTOAPIDataRequest Data)
        {
           return _aPIDataDB.GetByIC(Data);
        }

        public Task<MApiDataOffrs?> GetByoffrsIC(DTOAPIDataRequest Data)
        {
            return _aPIDataDB.GetByoffrsIC(Data);
        }
    }
}
