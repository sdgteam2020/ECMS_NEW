using BusinessLogicsLayer;
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
        public MasterController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #region Command Page
        public async Task<IActionResult> Command()
        {

            return View();
        }
        public async Task<IActionResult> SaveCommand(Comd dTO)
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
                return Json(await unitOfWork.Comds.GetAll());
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> DeleteCommand(Comd dTO)
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




            return Json(2);
        }
        public async Task<IActionResult> DeleteCommandMultiple(int[] ints)
        {
            try
            {
                Comd dto = new Comd();
                foreach (int i in ints)
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
                foreach (int i in ints)
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
                foreach (int i in ints)
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
                foreach (int i in ints)
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
        public async Task<IActionResult> GetAllMapUnit(DTOMHierarchyRequest Data)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLUnit(Data));
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
        public async Task<IActionResult> GetAllUnit()
        {
            try
            {
                return Json(await unitOfWork.Unit.GetAll());
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
                foreach (int i in ints)
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
        public async Task<IActionResult> DeleteAppointmentMultiple(int[] ints)
        {
            try
            {
                MAppointment dto = new MAppointment();
                foreach (int i in ints)
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

        #region Command Page
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
        public async Task<IActionResult> GetAllArmed(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Armed.GetAll());
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
                foreach (int i in ints)
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

        #endregion Command

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
