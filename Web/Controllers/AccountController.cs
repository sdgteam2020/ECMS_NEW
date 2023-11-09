using BusinessLogicsLayer;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.Service;
using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System;
using System.Data.Entity;
using System.Data.Entity.Hierarchy;
using System.Security.Claims;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly IDataProtector protector;
        private readonly IService service;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public const string SessionKeySalt = "_Salt";

        public AccountController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ApplicationDbContext contextTransaction,
            IDataProtectionProvider dataProtectionProvider, IService service, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.contextTransaction =contextTransaction;
            this.service = service;
            this.protector = dataProtectionProvider.CreateProtector(
    dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usera = await userManager.FindByIdAsync(userId);
            int sno = 1;
            var UserList = context.Users.ToList();
            var UserRoleList = context.UserRoles.ToList();
            var UserRoleNameList = context.Roles.ToList();
            var allrecord = from e in UserList
                            join r in UserRoleList on e.Id equals r.UserId
                            join n in UserRoleNameList on r.RoleId equals n.Id
                            orderby e.Id
                            select new DTORegisterListRequest()
                            {
                                EncryptedId = protector.Protect(e.Id.ToString()),
                                Sno = sno++,
                                DomainId = e.DomainId,
                                RoleName = n.Name,
                            };
            ViewBag.Title = "List of Register User";
            return View(allrecord);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.T = "Register User";
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.RoleOptions = unitOfWork.Users.GetRole();
            ViewBag.OptionsRank = service.GetRank();
            DTORegisterRequest model = new DTORegisterRequest();
            string dd = AESEncrytDecry.GetSalt();  // "8080808080808080"; //protector.Protect("1");
            HttpContext.Session.SetString(SessionKeySalt, dd);
            model.hdns = dd;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DTORegisterRequest model)
        {
            //var data =await unitOfWork.Users.GetAll();
            ////var data1 = unitOfWork.Users.GetByUserName("Kapoor").Result;
            //int i = (int)ResponseMessage.Success;
            //return View();
            string rolename = "";
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.RoleOptions = unitOfWork.Users.GetRole();
            ViewBag.OptionsRank = service.GetRank();
            if (userId == null)
            {
                userId = "0";
                rolename = "Admin";
            }
            else
            {
                var usera = await userManager.FindByIdAsync(userId);
                var roles = await userManager.GetRolesAsync(usera);

                if (roles[0] == "Admin")
                {
                    rolename = model.UserRole;
                }
            }

            if (ModelState.IsValid)
            {
                var user_ = await userManager.FindByIdAsync(userId);
                string user_name = string.Empty;
                string ipAddress;

                string dd = HttpContext.Session.GetString(SessionKeySalt);
                csConst.cSalt = dd;
                //string Password = AESEncrytDecry.DecryptStringAES(model.Password);

                user_name = model.DomainId;
                ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                var url = location.AbsoluteUri;

                var user = new ApplicationUser
                {
                    Active = true,
                    DomainId = model.DomainId,
                    Updatedby=1,
                    UpdatedOn= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                    UserName = model.DomainId.ToLower(),
                    Email = model.DomainId.ToLower() +"@army.mil",
                };
                //var password = Password.Generate(8, 6);
                var result = await userManager.CreateAsync(user, "Admin123#");//model.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, rolename);
                    TempData["success"] = "User ID has been successfully created.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);

        }
        public async Task<ActionResult> Logout() {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);

            // for concurrent login
            //if (userId != null)
            //{
            //    user.IsLogged = false;
            //    await userManager.UpdateAsync(user);
            //}
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // for concurrent login
            if (userId != null)
            {
                //user.LastLogOutDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                //await userManager.UpdateAsync(user);
            }

            //Solved ---- Lack of session validation and server expiration 
            //await userManager.UpdateSecurityStampAsync(user);
            await signInManager.SignOutAsync();
            return View();

        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            DTOLoginRequest loginVM = new DTOLoginRequest();
            string dd = AESEncrytDecry.GetSalt();  // "8080808080808080"; //protector.Protect("1");
            HttpContext.Session.SetString(SessionKeySalt, dd);
            loginVM.hdns = dd;

            return View(loginVM);

        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(DTOLoginRequest model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                string dd = HttpContext.Session.GetString(SessionKeySalt);
                csConst.cSalt = dd;
                string Password = AESEncrytDecry.DecryptStringAES(model.Password);

                string ipAddress;
                ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
                var url = location.AbsoluteUri;

                string user_name = string.Empty;

                var selecteduser = await userManager.FindByNameAsync(model.UserName);
                if (selecteduser == null)
                {
                    user_name = model.UserName;

                    ModelState.AddModelError(string.Empty, "Invalid User Id or Password");
                    goto xyz;
                }
                //else if (selecteduser.IsLogged)
                //{
                //    user_name = selecteduser.FullName != null ? selecteduser.FullName : "no name";
                //    await service.SaveUserActivity("Already loged", url, user_name, ipAddress);
                //    ModelState.AddModelError("", "This user is already loged in.");
                //    goto xyz;
                //}

                //else if (selecteduser.IsLocked)
                //{
                //    user_name = model.UserId;
                //    await service.SaveUserActivity("Locked User", url, user_name, ipAddress);
                //    ModelState.AddModelError("", "Your account is locked by admin");
                //    goto xyz;
                //}
                else
                {
                    //var result = await signInManager.PasswordSignInAsync(model.UserId, Password, model.RememberMe, true);
                    var result = await signInManager.PasswordSignInAsync(model.UserName, Password, false, true);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            var user = await userManager.FindByNameAsync(model.UserName);

                            user_name = model.UserName;


                            var roles = await userManager.GetRolesAsync(user);
                            ViewBag.Message = "Sucessfully Logged In.";

                        //user.IsLogged = true;
                        //await userManager.UpdateAsync(user);

                        //Start Concurrent Issue solve code.

                        //if(user!=null)
                        //{
                        //    DateTime? LastLogInDate = user.LastLogInDate;
                        //    DateTime? LastLogOutDate = user.LastLogOutDate;

                        //    DateTime CurrentDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")); ;
                        //    if (LastLogInDate == null)
                        //    {
                        //        user.LastLogInDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        //        await userManager.UpdateAsync(user);
                        //        goto roll_check;
                        //    }
                        //    else if(LastLogInDate.Value.Date == CurrentDateTime.Date)
                        //    {
                        //        if(LastLogOutDate!=null)
                        //        {
                        //            TimeSpan span = LastLogOutDate.Value.Subtract(LastLogInDate.Value); //Perform to Find the Difference
                        //            int Minutesdiff = span.Minutes;
                        //            if (Minutesdiff >= 20)
                        //            {
                        //                ModelState.AddModelError("", "This user is already loged in.");
                        //                goto xyz;
                        //            }
                        //            else
                        //            {
                        //                user.LastLogInDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        //                user.LastLogOutDate = null;
                        //                await userManager.UpdateAsync(user);
                        //                goto roll_check;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            TimeSpan span = CurrentDateTime.Subtract(LastLogInDate.Value); //Perform to Find the Difference
                        //            int Minutesdiff = span.Minutes;
                        //            if (Minutesdiff <= 20)
                        //            {
                        //                ModelState.AddModelError("", "This user is already loged in.");
                        //                goto xyz;
                        //            }
                        //            else
                        //            {
                        //                user.LastLogInDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        //                await userManager.UpdateAsync(user);
                        //                goto roll_check;
                        //            }
                        //        }
                        //    }
                        //    else if(CurrentDateTime.Date> LastLogInDate.Value.Date)
                        //    {
                        //        user.LastLogInDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
                        //        await userManager.UpdateAsync(user);
                        //        goto roll_check;
                        //    }
                        //}


                        //End Concurrent Issue solve code.

                        roll_check:
                            if (roles[0] == "Admin")
                            {
                                return RedirectToActionPermanent("Dashboard", "Home");
                            }
                            else if (roles[0] == "User")
                            {
                                return RedirectToActionPermanent("Dashboard", "Home");
                            }
                        }
                    }
                    if (result.IsLockedOut)
                        ModelState.AddModelError("", "Your account is locked out. Kindly wait for 10 minutes and try again");
                    else
                        ModelState.AddModelError(string.Empty, "Invalid User Id or Password");
                }
            }
            ViewBag.Message = "Sucessfully Logged In.";
        xyz:
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> SetPassword(string Id)
        {
            string decryptedId = string.Empty;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(Id);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Error");
            }

            var usera = await userManager.FindByIdAsync(decryptedId);

            var user = new ApplicationUser
            {
                Id = usera.Id,
                UserName = usera.UserName,

            };
            //var token = await userManager.GeneratePasswordResetTokenAsync(usera);

            string dd = AESEncrytDecry.GetSalt();
            HttpContext.Session.SetString(SessionKeySalt, dd);
            //var model = new SetPasswordVM { Token = token, UserId = user.UserId,hdns = dd };
            var model = new DTOSetPasswordRequest { UserName = user.UserName, hdns = dd };
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(DTOSetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null)
                RedirectToAction("UserNotFound");

            string dd = HttpContext.Session.GetString(SessionKeySalt);
            csConst.cSalt = dd;
            string ConfirmPassword = AESEncrytDecry.DecryptStringAES(model.ConfirmPassword);
            string OldPassword = AESEncrytDecry.DecryptStringAES(model.OldPassword);

            //var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, ConfirmPassword);
            var resetPassResult = await userManager.ChangePasswordAsync(user, OldPassword, ConfirmPassword);
            if (resetPassResult.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["success"] = "Password Successfully Changed.";
                return RedirectToAction("Detail");
            }
            else
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }

        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string Id)
        {
            string decryptedId = string.Empty;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(Id);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Error");
            }

            var usera = await userManager.FindByIdAsync(decryptedId);

            var user = new ApplicationUser
            {
                Id = usera.Id,
                UserName = usera.UserName,

            };
            var token = await userManager.GeneratePasswordResetTokenAsync(usera);

            string dd = AESEncrytDecry.GetSalt();
            HttpContext.Session.SetString(SessionKeySalt, dd);
            var model = new DTOResetPasswordRequest { Token = token, UserName = user.UserName, hdns = dd };

            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(DTOResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByNameAsync(model.UserName);

            if (user == null)
                RedirectToAction("UserNotFound");

            string dd = HttpContext.Session.GetString(SessionKeySalt);
            csConst.cSalt = dd;
            string ConfirmPassword = AESEncrytDecry.DecryptStringAES(model.ConfirmPassword);

            var resetPassResult = await userManager.ResetPasswordAsync(user, model.Token, ConfirmPassword);
            if (resetPassResult.Succeeded)
            {
                await signInManager.RefreshSignInAsync(user);
                TempData["success"] = "New Password Successfully Created.";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(RegisterRequest model)
        {
           return View(model);
        }

        //public async Task<ActionResult> Add(RegisterRequest model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var user = CreateUser();
        //            user.UserName = model.Username.ToLower();
        //            //user.Email = string.Format("{0}@{1}", model.Username.ToLower(), Common.Configurations.Application.MailDomain);
        //            user.EmailConfirmed = true;
        //            user.RankId = model.RankId;
        //            // user.PersonnelNumber = model.Number;
        //            user.FullName = model.FullName;
        //            // user.DEFwdAuth = model.DEFwdAuth;
        //            // user.GEBFwdAuth = model.GEBFwdAuth;
        //            user.Active = model.Active;
        //            //user.EstablishmentFull = model.EstablishmentFull;
        //            //user.EstablishmentAbbreviation = model.EstablishmentAbbreviation;
        //            // user.CreatedByIntId = CurrentUser.IntId;
        //            user.CreatedOn = DateTime.Now;
        //            //user.Superior = CurrentUser;
        //            user.PhoneNumber = model.ASCON;
        //            user.Unit_ID = model.Unit_ID;
        //            user.UserTypeId = model.UserTypeId;
        //            user.CreatedBy = 1;
        //            user.ParentId = 1;
        //            user.IntId = 1;
        //            // user.Appointment = model.Appointment;

        //            // await _userStore.SetUserNameAsync(user, model.Username, CancellationToken.None);
        //            // await _emailStore.SetEmailAsync(user, model.Username, CancellationToken.None);
        //            string Usenm = model.Username;
        //            //AH Commented password and Hardcoded due to logic changed by Kapoor during IAM Integration 
        //            //AH Corrected by Sub Maj Sanal on 09 Jun 23
        //            string pwd = "Dte@123"; // char.ToUpper(Usenm[0]) + Usenm.Substring(1) + "@123";
        //                                    //  end

        //            var result = await _userManager.CreateAsync(user, pwd);

        //            if (result.Succeeded)
        //            {
        //                // _logger.LogInformation("User created a new account with password.");
        //                await _userManager.AddToRoleAsync(user, "admin");

        //                var userId = await _userManager.GetUserIdAsync(user);

        //                TempData["msg"] = "*********** User Sucessfully Registered *************";
        //                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //                //var callbackUrl = Url.Page(
        //                //    "/Account/ConfirmEmail",
        //                //    pageHandler: null,
        //                //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
        //                //    protocol: Request.Scheme);

        //                //await _emailSender.SendEmailAsync(Input.UserName, "Confirm your email",
        //                //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //                //if (_userManager.Options.SignIn.RequireConfirmedAccount)
        //                //{
        //                return LocalRedirect("~/Identity/Account/Register");

        //                //}
        //                //else
        //                //{
        //                //    await _signInManager.SignInAsync(user, isPersistent: false);
        //                //    return LocalRedirect(returnUrl);
        //                //}
        //            }
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return View(model);
        //}
        //private ApplicationUser CreateUser()
        //{
        //    try
        //    {
        //        return Activator.CreateInstance<ApplicationUser>();
        //    }
        //    catch
        //    {
        //        throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
        //            $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
        //            $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        //    }
        //}


    }
}
