using DataAccessLayer;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Response;


namespace BusinessLogicsLayer.Master
{ 
    public class ArmedBL : GenericRepositoryDL<MArmedType>, IArmedBL
    {
        private readonly IArmedDB _iArmedDB;

        public ArmedBL(ApplicationDbContext context, IArmedDB iArmedDB) : base(context)
        {
            _iArmedDB = iArmedDB;   
        }

        public Task<bool> GetByName(MArmedType Dto)
        {
            Dto.ArmedName = Dto.ArmedName.Trim().TrimEnd().TrimStart();    
           return _iArmedDB.GetByName(Dto);   
        }
        public Task<List<DTOArmedResponse>> GetALLArmed()
        {
            return _iArmedDB.GetALLArmed();
        }
        public async Task<DTOArmedIdCheckInFKTableResponse?> ArmedIdCheckInFKTable(byte ArmedId)
        {
            return await _iArmedDB.ArmedIdCheckInFKTable(ArmedId);
        }
    }
}
