using BusinessLogicsLayer.Appt;
using BusinessLogicsLayer.BasicDet;
using BusinessLogicsLayer.BasicDetTemp;
using BusinessLogicsLayer.Bde;
using BusinessLogicsLayer.BdeCat;
using BusinessLogicsLayer.Corps;
using BusinessLogicsLayer.Div;
using BusinessLogicsLayer.Formation;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.Master;
using BusinessLogicsLayer.Unit;
using BusinessLogicsLayer.User;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer
{
    public class UnitOfWork : IUnitOfWork
    {

      
        public UnitOfWork(IUserBL _user, IComd _comds, ICorpsBL _corpsBL, IBdeBL _bdeCat, IDivBL divBL, IUnitBL unit, IMapUnitBL MapUnitBL, IFormationBL FormationBL, IApptBL apptBL, IArmedBL armedBL, IBasicDetailBL _basicDetailBL,IBasicDetailTempBL _basicDetailTempBL, IRankBL rankBL)
        {
            Users = _user;
            Comds = _comds;
            Corps = _corpsBL;
            Bde = _bdeCat;
            Div = divBL;
            MappUnit = MapUnitBL;
            Formation = FormationBL;
            Appt = apptBL;
            Armed = armedBL;
            Unit = unit;
            BasicDetail = _basicDetailBL;
            BasicDetailTemp = _basicDetailTempBL;
            Rank = rankBL;
        }
        public IUserBL Users { get; }

        public IComd Comds { get; }

        public ICorpsBL Corps { get; }
        public IDivBL Div { get; }
        public IBdeBL Bde { get; }
        public IMapUnitBL MappUnit { get; }
        public IUnitBL Unit { get; }
        public IFormationBL Formation { get; }
        public IApptBL Appt { get; }
        public IArmedBL Armed { get; }
        public IRankBL Rank { get; }
        public IBasicDetailBL BasicDetail { get; }
        public IBasicDetailTempBL BasicDetailTemp { get; }
        public async Task<List<DTOMasterResponse>> GetAllMMaster(DTOMasterRequest Data)
        {
            List<DTOMasterResponse> lst = new List<DTOMasterResponse>();
            int count = 0;
            if (Data.id == Convert.ToInt16(Constants.MasterTbl.Command))
            {
                var Ret = await Comds.GetAll();
                foreach (var comd in Ret)
                {

                    DTOMasterResponse db = new DTOMasterResponse();
                    if (comd.ComdId != 1)
                    {
                        db.Id = comd.ComdId;
                        db.Name = comd.ComdName;
                        lst.Add(db);
                    }

                }
            }
            else if (Data.id == Convert.ToInt16(Constants.MasterTbl.Corps))
            {
                var Ret = await Corps.GetByComdId(Convert.ToInt32(Data.ParentId));

                foreach (var comd in Ret)
                {
                    if (count == 0)
                    {
                        DTOMasterResponse db1 = new DTOMasterResponse();
                        db1.Id = 1;
                        db1.Name = "No Corps";
                        lst.Add(db1);
                        count = 1;
                    }

                    DTOMasterResponse db = new DTOMasterResponse();
                    db.Id = comd.CorpsId;
                    db.Name = comd.CorpsName;
                    lst.Add(db);



                }
                if (count == 0)
                {
                    DTOMasterResponse db = new DTOMasterResponse();
                    db.Id = 1;
                    db.Name = "No Corps";
                    lst.Add(db);

                }
            }
            else if (Data.id == Convert.ToInt16(Constants.MasterTbl.Formation))
            {
                var Ret = await Formation.GetAll();
                foreach (var Forma in Ret)
                {

                    DTOMasterResponse db = new DTOMasterResponse();

                    db.Id = Forma.FormationId;
                    db.Name = Forma.FormationName;
                    lst.Add(db);


                }
            }
            else if (Data.id == Convert.ToInt16(Constants.MasterTbl.Appt))
            {
                var Ret = await Appt.GetAll();
                foreach (var Forma in Ret)
                {

                    DTOMasterResponse db = new DTOMasterResponse();
            else if (Data.id == Convert.ToInt16(Constants.MasterTbl.Appt))
            {
                var Ret = await Appt.GetAll();
                foreach (var Forma in Ret)
                {

                    DTOMasterResponse db = new DTOMasterResponse();

                    db.Id = Forma.ApptId;
                    db.Name = Forma.AppointmentName;
                    lst.Add(db);


                }
            } 
            else if (Data.id == Convert.ToInt16(Constants.MasterTbl.Rank))
            {
                var Ret = await Rank.GetAll();
                foreach (var Forma in Ret)
                {

                    DTOMasterResponse db = new DTOMasterResponse();

                    db.Id = Forma.RankId;
                    db.Name = Forma.RankName;
                    lst.Add(db);


                }
            }
            //Constants.MasterTbl.Command;
            return lst;
        }

        public async Task<List<DTOMasterResponse>> GetAllMMasterByParent(DTOMHierarchyRequest Data)
        {
            List<DTOMasterResponse> lst = new List<DTOMasterResponse>();
            int count = 0;
            if (Data.TableId == Convert.ToInt16(Constants.MasterTbl.Div))
            {
                var Ret = await Div.GetByHId(Data);
                foreach (var comd in Ret)
                {
                    if (count == 0)
                    {
                        DTOMasterResponse db1 = new DTOMasterResponse();
                        db1.Id = 1;
                        db1.Name = "No Div";
                        lst.Add(db1);
                        count = 1;
                    }

                    DTOMasterResponse db = new DTOMasterResponse();
                    db.Id = comd.DivId;
                    db.Name = comd.DivName;
                    lst.Add(db);



                }
                if (count == 0)
                {
                    DTOMasterResponse db1 = new DTOMasterResponse();
                    db1.Id = 1;
                    db1.Name = "No Div";
                    lst.Add(db1);

                }
            }
            else if (Data.TableId == Convert.ToInt16(Constants.MasterTbl.Bde))
            {
                var Ret = await Bde.GetByHId(Data);
                foreach (var comd in Ret)
                {
                    if (count == 0)
                    {
                        DTOMasterResponse db1 = new DTOMasterResponse();
                        db1.Id = 1;
                        db1.Name = "No Bde";
                        lst.Add(db1);
                        count = 1;
                    }

                    DTOMasterResponse db = new DTOMasterResponse();
                    db.Id = comd.BdeId;
                    db.Name = comd.BdeName;
                    lst.Add(db);



                }
                if (count == 0)
                {
                    DTOMasterResponse db1 = new DTOMasterResponse();
                    db1.Id = 1;
                    db1.Name = "No Bde";
                    lst.Add(db1);

                }
            }
            //Constants.MasterTbl.Command;
            return lst;
        }


    }
}
