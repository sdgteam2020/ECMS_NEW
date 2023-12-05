using BusinessLogicsLayer;
using BusinessLogicsLayer.Master;
using DapperRepo.Core.Constants;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response.User;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Controllers
{
    public class MasterController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IChangeHierarchyMasterBL changeHierarchyMaster;
        public MasterController(IUnitOfWork unitOfWork, IChangeHierarchyMasterBL changeHierarchyMaster)
        {
            this.unitOfWork = unitOfWork;
            this.changeHierarchyMaster = changeHierarchyMaster;
        }

        #region Command Page
        public async Task<IActionResult> Command()
        {

            return View();
        }
        public async Task<IActionResult> SaveCommand(MComd dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Comds.GetByName(dTO))
                    {
                        if (dTO.ComdId > 0)
                        {
                            unitOfWork.Comds.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            dTO.Orderby=await unitOfWork.Comds.GetByMaxOrder();
                            await unitOfWork.Comds.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllCommand(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Comds.GetAllByorder());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteCommand(MComd dTO)
        {
            try
            {
                await unitOfWork.Comds.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }


        }
        public async Task<IActionResult> OrderByChange(MComd dTO)
        {
            try
            {
                await unitOfWork.Comds.OrderByChange(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }


        }
        public async Task<IActionResult> DeleteCommandMultiple(int[] ints)
        {
            try
            {
                MComd dto = new MComd();
                foreach (byte i in ints)
                {
                    dto.ComdId = i;
                    await unitOfWork.Comds.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {

                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion Command



        #region Corps 

        public IActionResult Corps()
        {

            return View();
        }
        public async Task<IActionResult> SaveCorps(MCorps dTO)
        {
            try
            {

                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Corps.GetByName(dTO))
                    {
                        if (dTO.CorpsId > 0)
                        {
                            unitOfWork.Corps.Update(dTO);

                            /////update Commd By CorpsId
                            MapUnit dat = new MapUnit();
                            dat.CorpsId = dTO.CorpsId;
                            dat.ComdId = dTO.ComdId;
                            changeHierarchyMaster.UpdateChageComdByCorps(dat);
                            ////////End Code //////////////

                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Corps.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllCorps(int Id)
        {
            try
            {
                return Json(await unitOfWork.Corps.GetALLCorps());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteCorps(MCorps dTO)
        {
            try
            {
                await unitOfWork.Corps.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {

                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteCorpsMultiple(int[] ints)
        {
            try
            {
                MCorps dto = new MCorps();
                foreach (byte i in ints)
                {
                    dto.CorpsId = i;
                    await unitOfWork.Corps.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Corps


        #region Div  

        public IActionResult Div()
        {

            return View();
        }
        public async Task<IActionResult> SaveDiv(MDiv dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Div.GetByName(dTO))
                    {
                        if (dTO.DivId > 0)
                        {
                            unitOfWork.Div.Update(dTO);

                            /////update Commd By CorpsId
                            MapUnit dat = new MapUnit();
                            dat.CorpsId = dTO.CorpsId;
                            dat.ComdId = dTO.ComdId;
                            dat.DivId=dTO.DivId;
                            changeHierarchyMaster.UpdateComdCorpsByDivs(dat);
                            ////////End Code //////////////
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Div.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllDiv(int Id)
        {
            try
            {
                return Json(await unitOfWork.Div.GetALLDiv());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteDiv(MDiv dTO)
        {
            try
            {
                await unitOfWork.Div.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> DeleteDivMultiple(int[] ints)
        {
            try
            {
                MDiv dto = new MDiv();
                foreach (byte i in ints)
                {
                    dto.DivId = i;
                    await unitOfWork.Div.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Bde

        #region Bde  

        public IActionResult Bde()
        {

            return View();
        }
        public async Task<IActionResult> SaveBde(MBde dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Bde.GetByName(dTO))
                    {
                        if (dTO.BdeId > 0)
                        {
                            unitOfWork.Bde.Update(dTO);

                            /////update Commd By CorpsId
                            MapUnit dat = new MapUnit();
                            dat.CorpsId = dTO.CorpsId;
                            dat.ComdId = dTO.ComdId;
                            dat.DivId = dTO.DivId;
                            dat.BdeId=dTO.BdeId;
                            changeHierarchyMaster.UpdateComdCorpsByDivs(dat);
                            ////////End Code //////////////
                            ///
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Bde.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllBde(int Id)
        {
            try
            {
                return Json(await unitOfWork.Bde.GetALLBdeCat());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteBde(MBde dTO)
        {
            try
            {
                await unitOfWork.Bde.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> DeleteBdeMultiple(int[] ints)
        {
            try
            {
                MBde dto = new MBde();
                foreach (byte i in ints)
                {
                    dto.BdeId = i;
                    await unitOfWork.Bde.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Bde

        #region MapUnit  

        public IActionResult MapUnit()
        {

            return View();
        }
        public async Task<IActionResult> SaveMapUnit(MapUnit dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.MappUnit.GetByName(dTO))
                    {
                        if (dTO.UnitMapId > 0)
                        {
                            unitOfWork.MappUnit.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.MappUnit.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllMapUnit(DTOMHierarchyRequest Data,string Unit)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLUnit(Data, Unit));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetALLByUnitName(string UnitName)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitName(UnitName));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        } 
      
        public async Task<IActionResult> GetALLByUnitMapId(int UnitMapId)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitMapId(UnitMapId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        } 
        public async Task<IActionResult> GetALLByUnitById(int UnitId)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitById(UnitId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteMapUnit(MapUnit dTO)
        {
            try
            {
                await unitOfWork.MappUnit.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> DeleteMapUnitMultiple(int[] ints)
        {
            try
            {
                MapUnit dto = new MapUnit();
                foreach (int i in ints)
                {
                    dto.UnitMapId = i;
                    await unitOfWork.MappUnit.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Unit

        #region Unit  

        public IActionResult Unit()
        {

            return View();
        }
        public async Task<IActionResult> SaveUnit(MUnit dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Unit.GetByName(dTO))
                    {
                        if (dTO.UnitId > 0)
                        {
                            unitOfWork.Unit.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Unit.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllUnit(string Unit)
        {
            try
            {
                return Json(await unitOfWork.Unit.GetAllUnit(Unit));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteUnit(MUnit dTO)
        {
            try
            {
                await unitOfWork.Unit.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> DeleteUnitMultiple(int[] ints)
        {
            try
            {
                MUnit dto = new MUnit();
                foreach (int i in ints)
                {
                    dto.UnitId = i;
                    await unitOfWork.Unit.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> GetBySusNO(MUnit Data)
        {
            try
            {
                var ret = await unitOfWork.Unit.GetBySusNo(Data);
                return Json(ret);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        #endregion End Unit

        #region Formation  

        public IActionResult Formation()
        {

            return View();
        }
        public async Task<IActionResult> SaveFormation(MFormation dTO)
        {
            try
            {
               
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Formation.GetByName(dTO))
                    {
                        if (dTO.FormationId > 0)
                        {
                            unitOfWork.Formation.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Formation.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllFormation()
        {
            try
            {
                return Json(await unitOfWork.Formation.GetAll());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteFormation(MFormation dTO)
        {
            try
            {
                await unitOfWork.Formation.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> DeleteFormationMultiple(int[] ints)
        {
            try
            {
                MFormation dto = new MFormation();
                foreach (byte i in ints)
                {
                    dto.FormationId = i;
                    await unitOfWork.Formation.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Formation


        #region Appt  

        public IActionResult Appointment()
        {

            return View();
        }
        public async Task<IActionResult> SaveAppointment(MAppointment dTO)
        {
            try
            {

                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Appt.GetByName(dTO))
                    {
                        if (dTO.ApptId > 0)
                        {
                            unitOfWork.Appt.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Appt.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllAppointment()
        {
            try
            {
                return Json(await unitOfWork.Appt.GetALLAppt());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteAppointment(MAppointment dTO)
        {
            try
            {
                await unitOfWork.Appt.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> DeleteAppointmentMultiple(short[] ints)
        {
            try
            {
                MAppointment dto = new MAppointment();
                foreach (byte i in ints)
                {
                    dto.ApptId = i;
                    await unitOfWork.Appt.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Appointment

        #region Rank Page
        public async Task<IActionResult> Rank()
        {

            return View();
        }
        public async Task<IActionResult> SaveRank(MRank dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Rank.GetByName(dTO))
                    {
                        if (dTO.RankId > 0)
                        {
                            unitOfWork.Rank.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            dTO.Orderby = Convert.ToByte(await unitOfWork.Comds.GetByMaxOrder());
                            await unitOfWork.Rank.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllRank(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Rank.GetAllByorder());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteRank(MRank dTO)
        {
            try
            {
                await unitOfWork.Rank.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }


        }
        public async Task<IActionResult> RankOrderByChange(MRank dTO)
        {
            try
            {
                await unitOfWork.Rank.OrderByChange(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }


        }
        public async Task<IActionResult> DeleteRankMultiple(int[] ints)
        {
            try
            {
                MRank dto = new MRank();
                foreach (byte i in ints)
                {
                    dto.RankId = i;
                    await unitOfWork.Rank.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {

                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion Command

        #region ArmedType Page
        public async Task<IActionResult> ArmedType()
        {

            return View();
        }
        public async Task<IActionResult> SaveArmed(MArmedType dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Armed.GetByName(dTO))
                    {
                        if (dTO.ArmedId > 0)
                        {
                            unitOfWork.Armed.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Armed.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllArmed()
        {
            try
            {
                return Json(await unitOfWork.Armed.GetALLArmed());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteArmed(MArmedType dTO)
        {
            try
            {
                await unitOfWork.Armed.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }





        }
        public async Task<IActionResult> DeleteArmedMultiple(int[] ints)
        {
            try
            {
                MArmedType dto = new MArmedType();
                foreach (byte i in ints)
                {
                    dto.ArmedId = i;
                    await unitOfWork.Armed.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {

                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion ArmedType


        #region Regimental Page
        public async Task<IActionResult> Regimental()
        {

            return View();
        }
        public async Task<IActionResult> SaveRegimental(MRegimental dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Regimental.GetByName(dTO))
                    {
                        if (dTO.RegId > 0)
                        {
                            unitOfWork.Regimental.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {

                            await unitOfWork.Regimental.Add(dTO);
                            return Json(KeyConstants.Save);


                        }
                    }
                    else
                    {
                        return Json(KeyConstants.Exists);
                    }

                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) { return Json(KeyConstants.InternalServerError); }

        }
        public async Task<IActionResult> GetAllRegimental(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Regimental.GetAllData());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteRegimental(MRegimental dTO)
        {
            try
            {
                await unitOfWork.Regimental.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }





        }
        public async Task<IActionResult> DeleteRegimentalMultiple(int[] ints)
        {
            try
            {
                MRegimental dto = new MRegimental();
                foreach (byte i in ints)
                {
                    dto.RegId = i;
                    await unitOfWork.Regimental.Delete(dto);
                }

                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {

                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion ArmedType

        #region Master Table 
        public async Task<IActionResult> GetAllMMaster(DTOMasterRequest Data)
        {
            try
            {
                var ret = await unitOfWork.GetAllMMaster(Data);
                return Json(ret);
            }
            catch
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> GetAllMMasterByParent(DTOMHierarchyRequest Data)
        {
            try
            {
                var ret = await unitOfWork.GetAllMMasterByParent(Data);
                return Json(ret);
            }
            catch
            {
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Master
    }
}
