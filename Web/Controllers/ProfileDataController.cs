using AutoMapper;
using BusinessLogicsLayer.Helpers;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System.Security.Claims;
using System;
using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using BusinessLogicsLayer.Service;
using DataTransferObject.Requests;

namespace Web.Controllers
{
    public class ProfileDataController : Controller
    {
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IGenericRepositoryDL<ProfileData> profileDataRepository;
        private readonly IService service;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private readonly ILogger<ProfileDataController> _logger;
        public DateTime dateTimenow;
        public ProfileDataController(IGenericRepositoryDL<ProfileData> profileDataRepository, IService service, IMapper mapper, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<ProfileDataController> logger)
        {
            this.profileDataRepository = profileDataRepository;
            this.service = service;
            this._mapper = mapper;
            this.context = context;
            this.contextTransaction = context;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
            _logger = logger;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usera = await userManager.FindByIdAsync(userId);
            int sno = 1;
            var ProfileDataList = context.ProfileDatas.Where(x => x.Updatedby == Convert.ToInt32(userId)).ToList();

            var allrecord = from e in ProfileDataList
                            orderby e.UpdatedOn descending
                            select new DTOProfileDataRequest()
                            {
                                ProfileDataId = e.ProfileDataId,
                                EncryptedId = protector.Protect(e.ProfileDataId.ToString()),
                                Sno = sno++,
                                ArmyNumber = e.ArmyNumber != null ? e.ArmyNumber : string.Empty,
                                Rank = e.Rank != null ? e.Rank : string.Empty,
                                Name = e.Name != null ? e.Name : string.Empty,
                                Appointment = e.Appointment != null ? e.Appointment : string.Empty,
                                DomainId = e.DomainId != null ? e.DomainId : string.Empty,
                                UnitSusNo = e.UnitSusNo != null ? e.UnitSusNo : string.Empty,
                                UnitName = e.UnitName != null ? e.UnitName : string.Empty,
                                InitiatingOfficerArmyNumber = e.InitiatingOfficerArmyNumber != null ? e.InitiatingOfficerArmyNumber : string.Empty
                            };
            ViewBag.Title = "List of Profile";
            return View(allrecord);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public Task<ViewResult> Create()
        {
            ViewBag.OptionsRank = service.GetRank();
            ViewBag.OptionsArmyNumberPart1 = service.GetArmyNumberPart1();
            ViewBag.OptionsArmyNumberPart3 = service.GetArmyNumberPart3();
            ViewBag.OptionsTypeOfUnit = service.GetTypeOfUnit();
            ViewBag.OptionsTemp = service.GetTemp();
            ViewBag.OptionsComd = service.GetComd();
            ViewBag.OptionsCorps = service.GetCorps();
            ViewBag.OptionsDiv = service.GetDiv();

            return Task.FromResult(View());
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DTOProfileDataCrtRequest model)
        {
            ViewBag.OptionsRank = service.GetRank();
            ViewBag.OptionsArmyNumberPart1 = service.GetArmyNumberPart1();
            ViewBag.OptionsArmyNumberPart3 = service.GetArmyNumberPart3();
            ViewBag.OptionsTypeOfUnit = service.GetTypeOfUnit();
            ViewBag.OptionsTemp = service.GetTemp();
            ViewBag.OptionsComd = service.GetComd();
            ViewBag.OptionsCorps = service.GetCorps();
            ViewBag.OptionsDiv = service.GetDiv();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            string user_name = string.Empty;
            string ipAddress;

            ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var url = location.AbsoluteUri;

            model.Updatedby = Convert.ToInt32(userId);
            if (ModelState.IsValid)
            {
                bool result = false;
                ProfileData newProfileData = _mapper.Map<DTOProfileDataCrtRequest, ProfileData>(model);
                if (model.TypeOfUnit == "Formation / Unit")
                {
                    if (model.Comd == null)
                    {
                        ModelState.AddModelError("Comd", "Comd is required.");
                        result = true;
                    }
                    if (model.Corps == null)
                    {
                        ModelState.AddModelError("Corps", "Corps is required.");
                        result = true;
                    }
                    if (model.Div == null)
                    {
                        ModelState.AddModelError("Div", "Div is required.");
                        result = true;
                    }
                    if (model.Bde == null)
                    {
                        ModelState.AddModelError("Bde", "Bde is required.");
                        result = true;
                    }
                    if (model.GISArmyNumberPart1 == null)
                    {
                        ModelState.AddModelError("GISArmyNumberPart1", "GIS Officer Army Number is required.");
                        result = true;
                    }
                    if (model.GISArmyNumberPart2 == 0)
                    {
                        ModelState.AddModelError("GISArmyNumberPart2", "GIS Officer Army Number is required.");
                        result = true;
                    }
                    if (model.GISArmyNumberPart3 == null)
                    {
                        ModelState.AddModelError("GISArmyNumberPart3", "GIS Officer Army Number is required.");
                        result = true;
                    }
                    if (model.GISRank == null)
                    {
                        ModelState.AddModelError("GISRank", "Rank is required.");
                        result = true;
                    }
                    if (model.GISName == null)
                    {
                        ModelState.AddModelError("GISName", "Name is required.");
                        result = true;
                    }
                    if (model.GISAppointment == null)
                    {
                        ModelState.AddModelError("GISAppointment", "Appointment is required.");
                        result = true;
                    }
                    if (model.GISUnitFormation == null)
                    {
                        ModelState.AddModelError("GISUnitFormation", "Formation / Unit is required.");
                        result = true;
                    }
                    if (result)
                    {
                        goto xyz;
                    }
                }
                else
                {
                    model.Comd = null;
                    model.Corps = null;
                    model.Div = null;
                    model.Bde = null;
                }
                newProfileData.UnitSusNo = model.UnitSusNoPart1 + model.UnitSusNoPart2;
                newProfileData.ArmyNumber = model.ArmyNumberPart1 + model.ArmyNumberPart2 + model.ArmyNumberPart3;
                newProfileData.InitiatingOfficerArmyNumber = model.IOArmyNumberPart1 + model.IOArmyNumberPart2 + model.IOArmyNumberPart3;
                newProfileData.IsSubmit = true;
                await profileDataRepository.Add(newProfileData);
                TempData["success"] = "Successfully created.";
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        xyz:
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> Edit(string Id)
        {
            ViewBag.OptionsRank = service.GetRank();
            ViewBag.OptionsArmyNumberPart1 = service.GetArmyNumberPart1();
            ViewBag.OptionsArmyNumberPart3 = service.GetArmyNumberPart3();
            ViewBag.OptionsTypeOfUnit = service.GetTypeOfUnit();
            ViewBag.OptionsTemp = service.GetTemp();
            ViewBag.OptionsComd = service.GetComd();
            ViewBag.OptionsCorps = service.GetCorps();
            ViewBag.OptionsDiv = service.GetDiv();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(Id);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }

            ProfileData? profileData = await profileDataRepository.Get(decryptedIntId);

            if (profileData != null)
            {
                DTOProfileDataUpdRequest profileDataUpdVM = _mapper.Map<ProfileData, DTOProfileDataUpdRequest>(profileData);
                profileDataUpdVM.EncryptedId = Id;
                return View(profileDataUpdVM);
            }
            else
            {
                Response.StatusCode = 404;
                return View("ProfileDataNotFound", decryptedId.ToString());
            }

        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DTOProfileDataUpdRequest model)
        {
            ViewBag.OptionsRank = service.GetRank();
            ViewBag.OptionsArmyNumberPart1 = service.GetArmyNumberPart1();
            ViewBag.OptionsArmyNumberPart3 = service.GetArmyNumberPart3();
            ViewBag.OptionsTypeOfUnit = service.GetTypeOfUnit();
            ViewBag.OptionsTemp = service.GetTemp();
            ViewBag.OptionsComd = service.GetComd();
            ViewBag.OptionsCorps = service.GetCorps();
            ViewBag.OptionsDiv = service.GetDiv();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            string user_name = string.Empty;
            string ipAddress;

            ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var url = location.AbsoluteUri;

            //ProfileData profileData = _mapper.Map<ProfileDataUpdVM, ProfileData>(model);
            ProfileData profileData = await profileDataRepository.Get(model.ProfileDataId);

            if (profileData != null)
            {
                if (ModelState.IsValid)
                {
                    bool result = false;
                    profileData.ArmyNumberPart1 = model.ArmyNumberPart1;
                    profileData.ArmyNumberPart2 = model.ArmyNumberPart2;
                    profileData.ArmyNumberPart3 = model.ArmyNumberPart3;
                    profileData.ArmyNumber = model.ArmyNumberPart1 + model.ArmyNumberPart2 + model.ArmyNumberPart3;
                    profileData.UnitSusNoPart1 = model.UnitSusNoPart1;
                    profileData.UnitSusNoPart2 = model.UnitSusNoPart2;
                    profileData.UnitSusNo = model.UnitSusNoPart1 + model.UnitSusNoPart2;
                    profileData.Rank = model.Rank;
                    profileData.Name = model.Name;
                    profileData.Appointment = model.Appointment;
                    profileData.DomainId = model.DomainId;
                    profileData.UnitName = model.UnitName;
                    profileData.TypeOfUnit = model.TypeOfUnit;
                    if (model.TypeOfUnit == "Formation / Unit")
                    {
                        if (model.Comd == null)
                        {
                            ModelState.AddModelError("Comd", "Comd is required.");
                            result = true;
                        }
                        if (model.Corps == null)
                        {
                            ModelState.AddModelError("Corps", "Corps is required.");
                            result = true;
                        }
                        if (model.Div == null)
                        {
                            ModelState.AddModelError("Div", "Div is required.");
                            result = true;
                        }
                        if (model.Bde == null)
                        {
                            ModelState.AddModelError("Bde", "Bde is required.");
                            result = true;
                        }
                        if (model.GISArmyNumberPart1 == null)
                        {
                            ModelState.AddModelError("GISArmyNumberPart1", "GIS Officer Army Number is required.");
                            result = true;
                        }
                        if (model.GISArmyNumberPart2 == 0)
                        {
                            ModelState.AddModelError("GISArmyNumberPart2", "GIS Officer Army Number is required.");
                            result = true;
                        }
                        if (model.GISArmyNumberPart3 == null)
                        {
                            ModelState.AddModelError("GISArmyNumberPart3", "GIS Officer Army Number is required.");
                            result = true;
                        }
                        if (model.GISRank == null)
                        {
                            ModelState.AddModelError("GISRank", "Rank is required.");
                            result = true;
                        }
                        if (model.GISName == null)
                        {
                            ModelState.AddModelError("GISName", "Name is required.");
                            result = true;
                        }
                        if (model.GISAppointment == null)
                        {
                            ModelState.AddModelError("GISAppointment", "Appointment is required.");
                            result = true;
                        }
                        if (model.GISUnitFormation == null)
                        {
                            ModelState.AddModelError("GISUnitFormation", "Formation / Unit is required.");
                            result = true;
                        }
                        if (result)
                        {
                            goto xyz;
                        }
                        profileData.Comd = model.Comd;
                        profileData.Corps = model.Corps;
                        profileData.Div = model.Div;
                        profileData.Bde = model.Bde;
                        profileData.GISArmyNumberPart1 = model.IOArmyNumberPart1;
                        profileData.GISArmyNumberPart2 = model.IOArmyNumberPart2;
                        profileData.GISArmyNumberPart3 = model.IOArmyNumberPart3;
                        profileData.GISOfficerArmyNumber = model.GISArmyNumberPart1 + model.GISArmyNumberPart2 + model.GISArmyNumberPart3;
                        profileData.GISRank = model.GISRank;
                        profileData.GISName = model.GISName;
                        profileData.GISAppointment = model.GISAppointment;
                        profileData.GISUnitFormation = model.GISUnitFormation;
                    }
                    else
                    {
                        profileData.Comd = null;
                        profileData.Corps = null;
                        profileData.Div = null;
                        profileData.Bde = null;
                        profileData.GISArmyNumberPart1 = null;
                        profileData.GISArmyNumberPart2 = null;
                        profileData.GISArmyNumberPart3 = null;
                        profileData.GISOfficerArmyNumber = null;
                        profileData.GISRank = null;
                        profileData.GISName = null;
                        profileData.GISAppointment = null;
                        profileData.GISUnitFormation = null;
                    }
                    profileData.IOArmyNumberPart1 = model.IOArmyNumberPart1;
                    profileData.IOArmyNumberPart2 = model.IOArmyNumberPart2;
                    profileData.IOArmyNumberPart3 = model.IOArmyNumberPart3;
                    profileData.InitiatingOfficerArmyNumber = model.IOArmyNumberPart1 + model.IOArmyNumberPart2 + model.IOArmyNumberPart3;
                    profileData.IORank = model.IORank;
                    profileData.IOName = model.IOName;
                    profileData.IOAppointment = model.IOAppointment;
                    profileData.IOUnitFormation = model.IOUnitFormation;
                    profileData.Updatedby = Convert.ToInt32(userId);
                    profileData.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                    await profileDataRepository.Update(profileData);
                    TempData["success"] = "Updated Successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                Response.StatusCode = 404;
                return View("ProfileDataNotFound", model.ProfileDataId.ToString());
            }
        xyz:
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string Id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            string user_name = string.Empty;
            string ipAddress;

            ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var url = location.AbsoluteUri;

            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(Id);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }

            ProfileData profileData = await profileDataRepository.Get(decryptedIntId);
            if (profileData == null)
            {
                Response.StatusCode = 404;
                return View("ProfileDataNotFound", decryptedId.ToString());
            }
            else
            {
                await profileDataRepository.Delete(profileData.ProfileDataId);
                TempData["success"] = "Deleted Successfully.";
                return RedirectToAction("index");
            }
        }
        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> DummyData()
        {
            Random rnd1 = new Random();
            String[] first = new String[] {"Yogendra", "Ajay", "Dilshad", "Vijay", "Balbeer", "Hari", "Chandra", "Ram",
                                            "Rahul", "Roy", "Nikhil", "Manoj", "Ankit", "Pradeep", "Vishkrma", "Ushman" };
            String[] last = new String[] {"Kumar", "Gupta", "Garg", "Rajvanshi", "Gujjar", "Singh", "Pal", "Kaushik",
                                              "Patel", "Modi", "Sharma", "Vasisth", "Khan", "Babra", "Hussain", "Tyagi" };

            String[] ArmyNumberPart1 = new string[] { "IC", "SS" };
            String[] Rank = new string[] { "Lt", "Capt", "Maj", "Lt Col", "Col", "Brig", "Maj Gen" };
            String[] ArmyNumberPart3 = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" };
            String[] Appointment = new string[] { "GSO-1", "COORD", "HR" };
            String[] UnitName = new string[] { "The Grenadiers", "Sikh Light Infantry", "Maratha Light Infantry", "Rajput Regiment", "Dogra Regiment", "Jat Regiment", "Assam Regiment" };
            String[] TypeOfUnit = new string[] { "Directorate", "OMG Branch", "Formation / Unit" };
            String[] Temp = new string[] { "Temp 1", "Temp 2", "Temp 3", "Temp 4", "Temp 5", "Temp 6", "Temp 7", "Temp 8", "Temp 9", "Temp 10", "Temp 11", "Temp 12" };
            String[] Comd = new string[] { "SC", "EC", "WC", "CC", "NC", "SWC", "TRG" };
            String[] Corps = new string[] { "1 Corps", "2 Corps", "3 Corps", "4 Corps", "9 Corps", "10 Corps", "11 Corps", "12 Corps", "14 Corps", "15 Corps", "16 Corps", "17 Corps" };
            String[] Div = new string[] { "1 Div", "2 Div", "3 Div", "4 Div", "5 Div", "6 Div", "7 Div", "8 Div", "9 Div", "10 Div", "11 Div", "12 Div", "14 Div", "15 Div" };
            Random rnd = new Random();
            int x = -1, y = -1, xx = -1, yy = -1, xxx = -1, yyy = -1, z = -1, r = -1, s = -1, i = -1, ii = -1, iii = -1, p = -1, t = -1, iop3 = -1, susp2 = -1, GISRank = -1, IORank = -1;
            int Comd_ = -1, Corps_ = -1, Div_ = -1, Bde = -1, IOUnitForm = -1, GISUnitForm = -1;

            x = rnd.Next(0, first.Length);
            y = rnd.Next(0, last.Length);
            xx = rnd.Next(0, first.Length);
            yy = rnd.Next(0, last.Length);
            xxx = rnd.Next(0, first.Length);
            yyy = rnd.Next(0, last.Length);
            r = rnd.Next(0, Rank.Length);
            IORank = rnd.Next(0, Rank.Length);
            GISRank = rnd.Next(0, Rank.Length);
            s = rnd.Next(0, ArmyNumberPart3.Length);
            iop3 = rnd.Next(0, ArmyNumberPart3.Length);
            susp2 = rnd.Next(0, ArmyNumberPart3.Length);
            i = rnd.Next(0, Appointment.Length);
            ii = rnd.Next(0, Appointment.Length);
            iii = rnd.Next(0, Appointment.Length);
            z = rnd.Next(0, ArmyNumberPart1.Length);
            p = rnd.Next(0, UnitName.Length);
            t = rnd.Next(0, TypeOfUnit.Length);
            Comd_ = rnd.Next(0, Comd.Length);
            Corps_ = rnd.Next(0, Corps.Length);
            Div_ = rnd.Next(0, Div.Length);
            Bde = rnd.Next(0, Temp.Length);
            IOUnitForm = rnd.Next(0, Temp.Length);
            GISUnitForm = rnd.Next(0, Temp.Length);

            DTOProfileDataCrtRequest profileDataCrtVM = new DTOProfileDataCrtRequest();
            profileDataCrtVM.ArmyNumberPart1 = ArmyNumberPart1[z];
            profileDataCrtVM.ArmyNumberPart2 = Random.Shared.Next(50000, 99999);
            profileDataCrtVM.ArmyNumberPart3 = ArmyNumberPart3[s];
            profileDataCrtVM.Rank = Rank[r];
            profileDataCrtVM.Name = first[x] + " " + last[y];
            profileDataCrtVM.Appointment = Appointment[i];
            profileDataCrtVM.DomainId = profileDataCrtVM.Appointment + "_asdc_ihqa";
            profileDataCrtVM.UnitSusNoPart1 = Random.Shared.Next(1000000, 9999999);
            profileDataCrtVM.UnitSusNoPart2 = ArmyNumberPart3[susp2]; ;
            profileDataCrtVM.TypeOfUnit = TypeOfUnit[t];
            profileDataCrtVM.UnitName = UnitName[p];
            profileDataCrtVM.IOArmyNumberPart1 = ArmyNumberPart1[z];
            profileDataCrtVM.IOArmyNumberPart2 = Random.Shared.Next(50000, 99999);
            profileDataCrtVM.IOArmyNumberPart3 = ArmyNumberPart3[iop3];
            profileDataCrtVM.IORank = Rank[IORank];
            profileDataCrtVM.IOName = first[xx] + " " + last[yy];
            profileDataCrtVM.IOAppointment = Appointment[ii];
            profileDataCrtVM.IOUnitFormation = Temp[IOUnitForm];

            if (profileDataCrtVM.TypeOfUnit == "Formation / Unit")
            {
                profileDataCrtVM.Comd = Comd[Comd_];
                profileDataCrtVM.Corps = Corps[Corps_];
                profileDataCrtVM.Div = Div[Div_];
                profileDataCrtVM.Bde = Temp[Bde];
            }
            if (profileDataCrtVM.TypeOfUnit == "Formation / Unit")
            {
                profileDataCrtVM.GISArmyNumberPart1 = ArmyNumberPart1[z];
                profileDataCrtVM.GISArmyNumberPart2 = Random.Shared.Next(50000, 99999);
                profileDataCrtVM.GISArmyNumberPart3 = ArmyNumberPart3[iop3];
                profileDataCrtVM.GISRank = Rank[GISRank];
                profileDataCrtVM.GISName = first[xxx] + " " + last[yyy];
                profileDataCrtVM.GISAppointment = Appointment[iii];
                profileDataCrtVM.GISUnitFormation = Temp[GISUnitForm];
            }
            return Ok(profileDataCrtVM);
        }
    }
}
