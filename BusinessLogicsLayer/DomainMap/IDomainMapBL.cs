using DataAccessLayer;
using DataTransferObject.Domain.Model;

namespace BusinessLogicsLayer.Bde
{
    public interface IDomainMapBL : IGenericRepositoryDL<TrnDomainMapping>
    {
        public Task<TrnDomainMapping?> GetTrnDomainMappingByUserId(int UserId);
        public Task<bool> GetByDomainId(TrnDomainMapping Data);
        //public Task<TrnDomainMapping> GetByAspnetUserIdBy(TrnDomainMapping Data);
        public Task<TrnDomainMapping?> GetByRequestId(int RequestId);
        public Task<TrnDomainMapping?> GetByAspnetUserIdBy(int AspNetUsersId);
        public Task<TrnDomainMapping?> GetAllRelatedDataByDomainId(string DomainId, string Role);
        public Task<TrnDomainMapping?> GetProfileDataByAspNetUserId(int Id);

    }
}
