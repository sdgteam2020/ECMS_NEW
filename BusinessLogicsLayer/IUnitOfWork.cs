using BusinessLogicsLayer.Appt;
using BusinessLogicsLayer.ArmedCat;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCat;
using BusinessLogicsLayer.Corps;
using BusinessLogicsLayer.Div;
using BusinessLogicsLayer.Formation;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer
{ 
    public interface IUnitOfWork
    {
        IUserBL Users { get; }
        IComd Comds { get; }
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
        IArmedCatBL ArmedCat { get; }


        public Task<List<DTOMasterResponse>> GetAllMMaster(DTOMasterRequest Data);
        public Task<List<DTOMasterResponse>> GetAllMMasterByParent(DTOMHierarchyRequest Data);
    }
}
