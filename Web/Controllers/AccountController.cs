using BusinessLogicsLayer;
using BusinessLogicsLayer.Account;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System;
using System.Data.Entity;
using System.Data.Entity.Hierarchy;
using System.Security.Claims;
using ApplicationRole = DataTransferObject.Domain.Identitytable.ApplicationRole;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAccountBL iAccountBL;
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly IDataProtector protector;
        private readonly IService service;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public const string SessionKeySalt = "_Salt";
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUnitOfWork unitOfWork, IAccountBL iAccountBL , RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ApplicationDbContext contextTransaction,
            IDataProtectionProvider dataProtectionProvider, IService service, DataProtectionPurposeStrings dataProtectionPurposeStrings, ILogger<AccountController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.unitOfWork = unitOfWork;
            this.iAccountBL= iAccountBL;
            this.context = context;
            this.contextTransaction = contextTransaction;
            this.service = service;
            this.protector = dataProtectionProvider.CreateProtector(
    dataProtectionPurposeStrings.AFSACIdRouteValue);
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditRole(string Id)
        {
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

            var role = await roleManager.FindByIdAsync(decryptedId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                EncryptedId= protector.Protect(role.Id.ToString()),
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId.ToString());

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.RoleId} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["success"] = "Role Updated Successfully.";
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["error"] = "Operation failed.";
                return View(model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(userId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaims = await userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                // If the user has the claim, set IsSelected property to true, so the checkbox
                // next to the claim is checked on the UI
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(model.UserId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            result = await userManager.AddClaimsAsync(user,
                model.Cliams.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            TempData["success"] = "Updated Successfully.";
            return RedirectToAction("EditUser", new { Id = model.UserId });
        }
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(userId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }

            ViewBag.userId = userId;

            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id.ToString(),
                    RoleName = role.Name
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(userId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user,model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            TempData["success"] = "Updated Successfully.";
            return RedirectToAction("EditUser", new { Id = userId });
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> DeleteUser(string Id)
        {
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

            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["success"] = "User Deleted Successfully.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("Index");
            }
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
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
            var role = await roleManager.FindByIdAsync(decryptedId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    //throw new Exception("Test Exception");

                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        TempData["success"] = "Role Deleted Successfully.";
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Error deleting role {ex}");

                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users " +
                        $"in this role. If you want to delete this role, please remove the users from " +
                        $"the role and then try to delete";
                    return View("Error");
                }
            }
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> EditUser(string Id)
        {
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
            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                UserId = user.Id,
                EncryptedId = protector.Protect(user.Id.ToString()),
                DomainId = user.DomainId,
                Active = user.Active,
                AdminFlag = user.AdminFlag,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin,Admin")]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(model.EncryptedId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            var user = await userManager.FindByIdAsync(decryptedId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.DomainId = model.DomainId;
                user.Active = model.Active;
                user.AdminFlag = model.AdminFlag;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["success"] = "User detail Updated Successfully.";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole identityRole = new ApplicationRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    TempData["success"] = "Created Role Successfully.";
                    return RedirectToAction("ListRoles", "Account");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["error"] = "Operation failed.";
            }

            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult ListRoles()
        {
            int sno = 1;
            var roles = roleManager.Roles;
            List<ApplicationRole> rolesList = new List<ApplicationRole>();
            foreach (var e in roles)
            {
                ApplicationRole applicationRole = new ApplicationRole()
                {
                   Id = e.Id,
                   Sno = sno++,
                   EncryptedId = protector.Protect(e.Id.ToString()),
                   Name = e.Name,
                };
                rolesList.Add(applicationRole);
            }
            //var allrecord = (from e in roles
            //                 select new ApplicationRole()
            //                 {
            //                     Id =e.Id,
            //                     Sno = sno++,
            //                     EncryptedId = protector.Protect(e.Id.ToString()),
            //                     Name=e.Name,
            //                 }).ToList();
            return View(rolesList);
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(roleId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(decryptedId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    EncryptedId= protector.Protect(user.Id.ToString()),
                    DomainId = user.DomainId
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            string decryptedId = string.Empty;
            int decryptedIntId = 0;
            try
            {
                // Decrypt the  id using Unprotect method
                decryptedId = protector.Unprotect(roleId);
                decryptedIntId = Convert.ToInt32(decryptedId);
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "This error occure because Id value change by user.");
                return RedirectToAction("Error", "Error");
            }
            var role = await roleManager.FindByIdAsync(decryptedId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {decryptedId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId.ToString());

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                    {
                        TempData["success"] = "Updated Successfully.";
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public async Task<IActionResult> Index()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usera = await userManager.FindByIdAsync(userId);
            int sno = 1;
            var UserList = context.Users.ToList();
            //var UserRoleList = context.UserRoles.ToList();
            //var UserRoleNameList = context.Roles.ToList();
            var allrecord = from e in UserList
                            //join r in UserRoleList on e.Id equals r.UserId
                            //join n in UserRoleNameList on r.RoleId equals n.Id
                            orderby e.Id
                            select new DTORegisterListRequest()
                            {
                                EncryptedId = protector.Protect(e.Id.ToString()),
                                Sno = sno++,
                                DomainId = e.DomainId,
                                //RoleName = n.Name,
                            };
            ViewBag.Title = "List of Register User";
            return View(allrecord);
        }
        [HttpGet]
        [Authorize(Roles = "Super Admin")]
        public IActionResult Create()
        {
            ViewBag.T = "Register User";
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.RoleOptions = unitOfWork.Users.GetRole();
            ViewBag.OptionsRank = service.GetRank(1);
            DTORegisterRequest model = new DTORegisterRequest();
            string dd = AESEncrytDecry.GetSalt();  // "8080808080808080"; //protector.Protect("1");
            HttpContext.Session.SetString(SessionKeySalt, dd);
            model.hdns = dd;
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Super Admin")]
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
            ViewBag.OptionsRank = service.GetRank(1);
            if (userId == null)
            {
                userId = "0";
                rolename = "Super Admin";
            }
            else
            {
                var usera = await userManager.FindByIdAsync(userId);
                var roles = await userManager.GetRolesAsync(usera);

                if (roles[0] == "Super Admin")
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
        public async Task<IActionResult> IMLogin(string DomainId,string Role)
        {
            DTOAccountResponse? dTOAccountResponse  = await iAccountBL.FindDomainId(DomainId);
            if(dTOAccountResponse!=null)
            {
                if(dTOAccountResponse.AdminFlag==true)
                {

                }
                else
                {
                    //Your request under process. Please contact Admin.
                }
            }
            else
            {
                //Create DomainId in AspNetUser Table & Role.
            }

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
                                return RedirectToActionPermanent("index", "ConfigUser");
                            }
                            else if (roles[0] == "Super Admin")
                            {
                                return RedirectToActionPermanent("Index", "Account");
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
        [Authorize(Roles = "Super Admin,Admin,User")]
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
        [Authorize(Roles = "Super Admin,Admin,User")]
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
        [Authorize(Roles = "Super Admin,Admin")]
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
        [Authorize(Roles = "Super Admin,Admin")]
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
