using BusinessLogicsLayer;
using BusinessLogicsLayer.Master;
using DapperRepo.Core.Constants;
using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using DataTransferObject.Response.User;
using EntityFramework.Exceptions.Common;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Data.Common;
using System.Security.Claims;
using System.Text;
using Web.WebHelpers;

namespace Web.Controllers
{
    public class MasterController : Controller
    {   
        private readonly IUnitOfWork unitOfWork;
        private readonly IChangeHierarchyMasterBL changeHierarchyMaster;
        private readonly ILogger<MasterController> _logger;
        private readonly IEncryptsqlDB _iEncryptsqlDB;
        public MasterController(IUnitOfWork unitOfWork, IChangeHierarchyMasterBL changeHierarchyMaster, ILogger<MasterController> logger, IEncryptsqlDB iEncryptsqlDB)
        {
            this.unitOfWork = unitOfWork;
            this.changeHierarchyMaster = changeHierarchyMaster;
            _logger = logger; 
            _iEncryptsqlDB = iEncryptsqlDB;
        }

        #region Command Page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Command()
        {
            string role = this.User.FindFirstValue(ClaimTypes.Role);
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveCommand(MComd dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.ComdName = dTO.ComdName.Trim();
                dTO.ComdAbbreviation= dTO.ComdAbbreviation.Trim();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Comds.GetByName(dTO))
                    {
                        if (dTO.ComdId > 0)
                        {
                            await unitOfWork.Comds.Update(dTO);
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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveCommand");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCommand(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Comds.GetAllByorder());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllCommand");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCommand(MComd dTO)
        {
            try
            {
                await unitOfWork.Comds.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteCommand");
                return Json(KeyConstants.InternalServerError);
            }


        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OrderByChange(MComd dTO)
        {
            try
            {
                await unitOfWork.Comds.OrderByChange(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->OrderByChange");
                return Json(KeyConstants.InternalServerError);
            }


        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteCommandMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBinaryTree(int Id)
        {
            try
            {
                var ret = Json(await unitOfWork.Comds.GetBinaryTree(Id));
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllCommand");
                return Json(KeyConstants.InternalServerError);
            }

        }
        #endregion Command

        #region Corps 
        [Authorize(Roles = "Admin")]
        public IActionResult Corps()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveCorps(MCorps dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.CorpsName = dTO.CorpsName.Trim();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Corps.GetByName(dTO))
                    {
                        if (dTO.CorpsId > 0)
                        {
                            await unitOfWork.Corps.Update(dTO);

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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveCorps");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCorps(int Id)
        {
            try
            {
                return Json(await unitOfWork.Corps.GetALLCorps());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllCorps");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCorps(MCorps dTO)
        {
            try
            {
                await unitOfWork.Corps.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteCorps");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteCorpsMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Corps

        #region Div  
        [Authorize(Roles = "Admin")]
        public IActionResult Div()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveDiv(MDiv dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.DivName = dTO.DivName.Trim();
                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Div.GetByName(dTO))
                    {
                        if (dTO.DivId > 0)
                        {
                            await unitOfWork.Div.Update(dTO);

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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveDiv");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDiv(int Id)
        {
            try
            {
                return Json(await unitOfWork.Div.GetALLDiv());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllDiv");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDiv(MDiv dTO)
        {
            try
            {
                await unitOfWork.Div.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteDiv");
                return Json(KeyConstants.InternalServerError);
            }
        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteDivMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Bde

        #region Bde  
        [Authorize(Roles = "Admin")]
        public IActionResult Bde()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveBde(MBde dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.BdeName = dTO.BdeName.Trim();
                if (ModelState.IsValid)
                {
                    #region old code
                    //if (dTO.BdeId > 0)
                    //{
                    //    bool? result = await unitOfWork.Bde.FindByBdeWithId(dTO.BdeName,dTO.BdeId);
                    //    if(result!=null)
                    //    {
                    //        if (result == true)
                    //        {
                    //            return Json(KeyConstants.Exists);
                    //        }
                    //        else
                    //        {
                    //            unitOfWork.Bde.Update(dTO);

                    //            /////update Commd By CorpsId
                    //            MapUnit dat = new MapUnit();
                    //            dat.CorpsId = dTO.CorpsId;
                    //            dat.ComdId = dTO.ComdId;
                    //            dat.DivId = dTO.DivId;
                    //            dat.BdeId = dTO.BdeId;
                    //            changeHierarchyMaster.UpdateComdCorpsByDivs(dat);
                    //            ////////End Code //////////////
                    //            ///
                    //            return Json(KeyConstants.Update);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        return Json(KeyConstants.InternalServerError);
                    //    }

                    //}
                    //else
                    //{
                    //    if(!await unitOfWork.Bde.GetByName(dTO))
                    //    {
                    //        await unitOfWork.Bde.Add(dTO);
                    //        return Json(KeyConstants.Save);
                    //    }
                    //    else
                    //    {
                    //        return Json(KeyConstants.Exists);
                    //    }
                    //}
                    #endregion

                    bool? result = await unitOfWork.Bde.GetByName(dTO);
                    if (result != null)
                    {
                        if (result == true)
                        {
                            return Json(KeyConstants.Exists);
                        }
                        else
                        {
                            if (dTO.BdeId > 0)
                            {
                                await unitOfWork.Bde.Update(dTO);

                                /////update Commd By CorpsId
                                MapUnit dat = new MapUnit();
                                dat.CorpsId = dTO.CorpsId;
                                dat.ComdId = dTO.ComdId;
                                dat.DivId = dTO.DivId;
                                dat.BdeId = dTO.BdeId;
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
                    }
                    else
                    {
                        return Json(KeyConstants.InternalServerError);
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveBde");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBde(int Id)
        {
            try
            {
                return Json(await unitOfWork.Bde.GetALLBdeCat());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllBde");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBde(MBde dTO)
        {
            try
            {
                await unitOfWork.Bde.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteBde");
                return Json(KeyConstants.InternalServerError);
            }
        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteBdeMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }
        #endregion End Bde

        #region MapUnit  

        public IActionResult MapUnit()
        {

            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> GetTopBySUSNo(string SUSNo)
        {
            try
            {
                return Json(await unitOfWork.Unit.GetTopBySUSNo(SUSNo));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetTopBySUSNo");
                return Json(KeyConstants.InternalServerError);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveUnitWithMapping(DTOSaveUnitWithMappingByAdminRequest dTO)
        {
            try
            {
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                dTO.Suffix= dTO.Suffix.Trim();
                dTO.Sus_no = dTO.Sus_no.Trim();
                if (ModelState.IsValid)
                {
                    string Sus_no = dTO.Sus_no + dTO.Suffix;
                    if (dTO.UnitId > 0 && dTO.UnitMapId == 0)
                    {
                        bool? CheckDuplicate = await unitOfWork.MappUnit.FindUnitId(dTO.UnitId);
                        if (CheckDuplicate == true)
                        {
                            return Json(KeyConstants.Exists);

                        }
                        else if (CheckDuplicate == false)
                        {
                            bool result = (bool)await unitOfWork.MappUnit.SaveUnitWithMapping(dTO);
                            if (result == true)
                            {
                                return Json(KeyConstants.Save);
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else
                        {
                            return Json(KeyConstants.InternalServerError);
                        }

                    }
                    else if (dTO.UnitId > 0 && dTO.UnitMapId > 0)
                    {
                        bool? CheckDuplicate = await unitOfWork.MappUnit.FindUnitIdMapped(dTO.UnitId,dTO.UnitMapId);
                        if (CheckDuplicate == true)
                        {
                            return Json(KeyConstants.Exists);

                        }
                        else if(CheckDuplicate == false)
                        {
                            bool result = (bool)await unitOfWork.MappUnit.SaveUnitWithMapping(dTO);
                            if (result == true)
                            {
                                return Json(KeyConstants.Update);
                            }
                            else
                            {
                                return Json(KeyConstants.InternalServerError);
                            }
                        }
                        else 
                        {
                            return Json(KeyConstants.InternalServerError);
                        }
                    }
                    else
                    {
                        return Json(KeyConstants.InternalServerError);
                    }
                }
                else
                {

                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveUnitWithMapping");
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> SaveMapUnit(MapUnit dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveMapUnit");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        public async Task<IActionResult> GetAllMapUnit(DTOMHierarchyRequest Data,string Unit)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLUnit(Data, Unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllMapUnit");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [AllowAnonymous]
        public async Task<IActionResult> GetALLByUnitName(string UnitName)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitName(UnitName));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetALLByUnitName");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [AllowAnonymous]
        public async Task<IActionResult> GetALLByUnitMapId(int UnitMapId)
        {
            try
            {
                return Json(await unitOfWork.MappUnit.GetALLByUnitMapId(UnitMapId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetALLByUnitMapId");
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
                _logger.LogError(1001, ex, "Master->GetALLByUnitById");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMapUnit(int UnitMapId)
        {
            try
            {
                MapUnit mapUnit = await unitOfWork.MappUnit.Delete(UnitMapId);
                if(mapUnit!=null)
                {
                    return Json(KeyConstants.Success);
                }
                else
                {
                    return Json(KeyConstants.InternalServerError);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteMapUnit");
                return Json(KeyConstants.InternalServerError);
            }
        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteMapUnitMultiple");
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
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.UnitName = dTO.UnitName.Trim();
                dTO.Abbreviation = dTO.Abbreviation != null ? dTO.Abbreviation.Trim() : dTO.Abbreviation;
                dTO.Suffix = dTO.Suffix.Trim();
                //dTO.UnitDesc =  await _iEncryptsqlDB.GetEncryptString(ConnKeyConstants.EncryptByPassPhraseKey, dTO.UnitName);
                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Unit.GetByName(dTO))
                    {
                        if (dTO.UnitId > 0)
                        {
                            await unitOfWork.Unit.Update(dTO);
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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveUnit");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        public async Task<IActionResult> GetAllUnit(string Unit)
        {
            try
            {
                return Json(await unitOfWork.Unit.GetAllUnit(Unit));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllUnit");
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
                _logger.LogError(1001, ex, "Master->DeleteUnit");
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
                _logger.LogError(1001, ex, "Master->DeleteUnitMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }
        public async Task<IActionResult> GetBySusNO(MUnit Data)
        {
            try
            {
                var ret = await unitOfWork.Unit.GetBySusNo(Data.Sus_no + Data.Suffix);
                return Json(ret);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetBySusNO");
                return Json(KeyConstants.InternalServerError);
            }

        }
        public async Task<IActionResult> GetUnitByUnitId(int UnitId)
        {
            try
            {
                var ret = await unitOfWork.Unit.Get(UnitId);
                var result = new DTOUnitResponse
                {
                    UnitId = ret.UnitId,
                    Sus_no = (ret.Sus_no + ret.Suffix).ToUpper(),
                    UnitName = ret.UnitName,
                    Abbreviation = ret.Abbreviation,
                    IsVerify = ret.IsVerify,
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetUnitByUnitId");
                return Json(KeyConstants.InternalServerError);
            }

        }
        #endregion End Unit

        #region Formation  
        [Authorize(Roles = "Admin")]
        public IActionResult Formation()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
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
                            await unitOfWork.Formation.Update(dTO);
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Appointment()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveAppointment(MAppointment dTO)
        {
            try
            {

                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.AppointmentName = dTO.AppointmentName.Trim();
                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Appt.GetByName(dTO))
                    {
                        if (dTO.ApptId > 0)
                        {
                            await unitOfWork.Appt.Update(dTO);
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
            catch (Exception ex) {
                _logger.LogError(1001, ex, "Master->SaveAppointment");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetByApptId(int ApptId)
        {
            try
            {
                return Json(await unitOfWork.Appt.GetByApptId(ApptId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [AllowAnonymous]
        public async Task<IActionResult> GetALLByAppointmentName(string AppointmentName)
        {
            try
            {
                return Json(await unitOfWork.Appt.GetALLByAppointmentName(AppointmentName));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        #endregion End Appointment

        #region Rank Page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Rank()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveRank(MRank dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.RankName = dTO.RankName.Trim();
                dTO.RankAbbreviation = dTO.RankAbbreviation.Trim();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Rank.GetByName(dTO))
                    {
                        if (dTO.RankId > 0)
                        {
                            await unitOfWork.Rank.Update(dTO);
                            return Json(KeyConstants.Update);
                        }
                        else
                        {
                            dTO.Orderby = await unitOfWork.Rank.GetByMaxOrder();
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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveRank");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRank(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Rank.GetAllByorder());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllRank");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRank(MRank dTO)
        {
            try
            {
                await unitOfWork.Rank.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRank");
                return Json(KeyConstants.InternalServerError);
            }


        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RankOrderByChange(MRank dTO)
        {
            try
            {
                await unitOfWork.Rank.OrderByChange(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->RankOrderByChange");
                return Json(KeyConstants.InternalServerError);
            }


        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteRankMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion Command

        #region ArmedType Page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ArmedType()
        {

            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveArmed(MArmedType dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = 1;
                dTO.UpdatedOn = DateTime.Now;
                dTO.ArmedName = dTO.ArmedName.Trim();
                dTO.Abbreviation= dTO.Abbreviation.Trim();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Armed.GetByName(dTO))
                    {
                        if (dTO.ArmedId > 0)
                        {
                            await unitOfWork.Armed.Update(dTO);
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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveArmed");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllArmed()
        {
            try
            {
                return Json(await unitOfWork.Armed.GetALLArmed());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllArmed");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArmed(MArmedType dTO)
        {
            try
            {
                await unitOfWork.Armed.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteArmed");
                return Json(KeyConstants.InternalServerError);
            }





        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteArmedMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion ArmedType

        #region Regimental Page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Regimental()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveRegimental(MRegimental dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.Name = dTO.Name.Trim();
                dTO.Abbreviation=dTO.Abbreviation.Trim();
                dTO.Location= dTO.Location.Trim();

                if (ModelState.IsValid)
                {
                    if (!await unitOfWork.Regimental.GetByName(dTO))
                    {
                        if (dTO.RegId > 0)
                        {
                            await unitOfWork.Regimental.Update(dTO);
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
            catch (Exception ex) 
            {
                _logger.LogError(1001, ex, "Master->SaveRegimental");
                return Json(KeyConstants.InternalServerError); 
            }

        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAllRegimental(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.Regimental.GetAllData());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllRegimental");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRegimental(MRegimental dTO)
        {
            try
            {
                await unitOfWork.Regimental.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRegimental");
                return Json(KeyConstants.InternalServerError);
            }
        }
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(1001, ex, "Master->DeleteRegimentalMultiple");
                return Json(KeyConstants.InternalServerError);
            }
        }

        #endregion ArmedType

        #region Record Office
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecordOffice()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveRecordOffice(MRecordOffice dTO)
        {
            try
            {
                dTO.IsActive = true;
                dTO.Updatedby = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                dTO.UpdatedOn = DateTime.Now;
                dTO.Name = dTO.Name.Trim();
                dTO.Abbreviation = dTO.Abbreviation.Trim();

                if (ModelState.IsValid)
                {
                    int result = await unitOfWork.RecordOffice.GetByName(dTO);
                    if (result == 1)
                    {
                        if (dTO.RecordOfficeId > 0)
                        {
                            await unitOfWork.RecordOffice.Update(dTO);
                            return Json(6);
                        }
                        else
                        {
                            await unitOfWork.RecordOffice.Add(dTO);
                            return Json(5);
                        }
                    }
                    else
                    {
                        if(result == 2)
                        {
                            return Json(2);
                        }
                        else if(result == 3)
                        {
                            return Json(3);
                        }
                        else if(result == 4)
                        {
                            return Json(4);
                        }
                        else
                        {
                            return Json(0);
                        }
                        
                    }
                }
                else
                {
                    return Json(ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList());
                }
            }
            //catch (DbUpdateException ex) when (ex.InnerException?.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            //{
            //    _logger.LogError(1001, ex, "Master->SaveRecordOffice");
            //    return Json(KeyConstants.Exists);
            //}
            //catch (UniqueConstraintException ex)
            //{
            //    _logger.LogError(1001, ex, "Master->SaveRecordOffice");
            //    return Json(KeyConstants.Exists);
            //}
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->SaveRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAllRecordOffice(int[] Id)
        {
            try
            {
                return Json(await unitOfWork.RecordOffice.GetAllData());
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetAllRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRecordOffice(MRecordOffice dTO)
        {
            try
            {
                await unitOfWork.RecordOffice.Delete(dTO);
                return Json(KeyConstants.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->DeleteRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }
        }
       
        [Authorize]
        public async Task<IActionResult> GetMappedForRecord(int TypeId, string SearchName)
        {
            try
            {
                return Json(await unitOfWork.MasterBL.GetMappedForRecord(TypeId, SearchName));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDomainIdByTDMId(int TDMId)
        {
            try
            {
                return Json(await unitOfWork.MasterBL.GetDomainIdByTDMId(TDMId));
            }
            catch (Exception ex)
            {
                return Json(KeyConstants.InternalServerError);
            }
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUpdateRecordOffice()
        {
            try
            {
                DtoSession? dtoSession = new DtoSession();
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
                {
                    dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

                }
                int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
                int TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
                return Json(await unitOfWork.RecordOffice.GetUpdateRecordOffice(TDMId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetUpdateRecordOffice");
                return Json(KeyConstants.InternalServerError);
            }

        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateRecordOffice()
        {
            DtoSession? dtoSession = new DtoSession();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                dtoSession = SessionHeplers.GetObject<DtoSession>(HttpContext.Session, "Token");

            }
            int UnitId = dtoSession != null ? dtoSession.UnitId : 0;
            int TDMId = dtoSession != null ? dtoSession.TrnDomainMappingId : 0;
            ViewBag.UnitId = UnitId;
            ViewBag.TDMId = TDMId;
            return View();
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetDDMappedForRecord(int UnitMapId)
        {
            try
            {
                return Json(await unitOfWork.RecordOffice.GetDDMappedForRecord(UnitMapId));
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "Master->GetDDMappedForRecord");
                return Json(KeyConstants.InternalServerError);
            }

        }
        #endregion

        #region Master Table 
        [AllowAnonymous]
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
        [AllowAnonymous]
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

        #region Dashboard
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DashboardMaster()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardMasterCount()
        {
            return Json(await unitOfWork.MasterBL.GetDashboardMasterCount());
        }
        #endregion Dashboard
    }
}
