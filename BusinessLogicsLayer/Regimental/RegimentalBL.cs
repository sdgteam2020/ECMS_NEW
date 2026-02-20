using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer.Master
{ 
    public class RegimentalBL : GenericRepositoryDL<MRegimental>, IRegimentalBL
    {
        private readonly IRegimentalDB _RegimentalDB;

        public RegimentalBL(ApplicationDbContext context, IRegimentalDB iRegimentalDB) : base(context)
        {
            _RegimentalDB = iRegimentalDB;   
        }

        public Task<List<DTORegimentalResponse>> GetAllData()
        {
            return _RegimentalDB.GetAllData();
        }
        public Task<List<DTORegimentalResponse>> GetByArmedId(byte ArmedId)
        {
            return _RegimentalDB.GetByArmedId(ArmedId);
        }

        public Task<bool> GetByName(MRegimental Dto)
        {
            Dto.Name = Dto.Name.Trim().TrimEnd().TrimStart();    
           return _RegimentalDB.GetByName(Dto);   
        }
        public async Task<DTODataTablesResponse<DTORegimentalResponse>> GetAllRegimental_Pagination(DTODataTablesRequest dTO)
        {
            return await _RegimentalDB.GetAllRegimental_Pagination(dTO); 
        }
        public async Task<bool> ValidateUnitIdInRegimental(int UnitId)
        {   
            return await _RegimentalDB.ValidateUnitIdInRegimental(UnitId);
        }
    }
}
