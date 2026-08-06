using BusinessLogicsLayer.AfsacCellMapp;
using BusinessLogicsLayer.Appt;
using BusinessLogicsLayer.ArmedCat;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.Corps;
using BusinessLogicsLayer.DispatchMode;
using BusinessLogicsLayer.Div;
using BusinessLogicsLayer.Formation;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.OROMapp;
using BusinessLogicsLayer.RecordOffice;
using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using BusinessLogicsLayer.Rank;
using DataTransferObject.Requests;
using DataTransferObject.Response;

namespace BusinessLogicsLayer
{ 
    public interface IUnitOfWork
    {
        IUserBL Users { get; }
        IComdBL Comds { get; }
        ICorpsBL Corps { get; }
        IBdeBL Bde { get; }
        IDivBL Div { get; }
        IMapUnitBL MappUnit { get; }
        IFormationBL Formation { get; }
        IApptBL Appt { get; }
        IArmedBL Armed { get; }
        IRankBL Rank { get; }   
        IUnitBL Unit { get; }
        IRegimentalBL Regimental { get; }
        IRecordOfficeBL RecordOffice { get; }
        IArmedCatBL ArmedCat { get; }
        IMasterBL MasterBL { get; }
        IOROMappingBL OROMapping { get; }
        IAfsacCellMappingBL AfsacCellMapping { get; }
        IDispatchModeBL DispatchMode { get; }


        public Task<List<DTOMasterResponse>> GetAllMMaster(DTOMasterRequest Data);

        public Task<List<DTOMasterResponse>> GetAllMMaster_Outer(DTOMasterRequest Data);
        public Task<List<DTOMasterResponse>> GetAllMMasterByParent(DTOParentChildIdRequest Data);
    }
}
