using AutoMapper;
using BusinessLogicsLayer.Helpers;
using BusinessLogicsLayer.Service;
using DataTransferObject.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Requests;

namespace Web.Controllers
{
    public class DocUploadController : Controller
    {
        private readonly ApplicationDbContext context, contextTransaction;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IGenericRepositoryDL<DocUpload> docUploadRepository;
        private readonly IService service;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IDataProtector protector;
        private readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public DateTime dateTimenow;
        public DocUploadController(IGenericRepositoryDL<DocUpload> docUploadRepository, IService service, IMapper mapper, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment, IDataProtectionProvider dataProtectionProvider,
                              DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            this.docUploadRepository = docUploadRepository;
            this.service = service;
            this._mapper = mapper;
            this.context = context;
            this.contextTransaction = context;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult UploadDocument()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            DocUpload? docUpload = context.DocUploads.FirstOrDefault(x => x.Updatedby == Convert.ToInt32(userId));

            if (docUpload != null)
            {
                DTODocUploadUpdRequest docUploadUpdVM = _mapper.Map<DocUpload, DTODocUploadUpdRequest>(docUpload);
                docUploadUpdVM.ExistingDocPath = docUploadUpdVM.DocPath;
                return RedirectToAction("EditUploadDocument", "DocUpload");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(DTODocUploadCrtRequest model)
        {
            ViewBag.OptionsRank = service.GetRank(1);
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
                DocUpload docUpload = _mapper.Map<DTODocUploadCrtRequest, DocUpload>(model);

                if (docUpload != null)
                {
                    string sourceFolderPhotoDB = "/WriteReadData/" + "Document";
                    string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Document");

                    if (!Directory.Exists(sourceFolderPhotoPhy))
                        Directory.CreateDirectory(sourceFolderPhotoPhy);

                    if (model.Doc_ != null)
                    {
                        string FileName = service.ProcessUploadedFile(model.Doc_, sourceFolderPhotoPhy,"");

                        string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                        bool result = service.IsValidDocHeader(path);

                        if (!result)
                        {
                            ModelState.AddModelError("", "File format not correct");
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            goto xyz;
                        }

                        docUpload.DocPath = sourceFolderPhotoDB + "/" + FileName;
                    }

                    await docUploadRepository.Add(docUpload);
                    TempData["success"] = "Successfully created.";
                    return RedirectToAction("Message");
                }
                else
                {
                    TempData["error"] = "Operation failed.";
                }
            }
        xyz:
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult EditUploadDocument()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            DocUpload? docUpload = context.DocUploads.FirstOrDefault(x => x.Updatedby == Convert.ToInt32(userId));

            if (docUpload != null)
            {
                DTODocUploadUpdRequest docUploadUpdVM = _mapper.Map<DocUpload, DTODocUploadUpdRequest>(docUpload);
                docUploadUpdVM.ExistingDocPath = docUploadUpdVM.DocPath;
                return View(docUploadUpdVM);
            }
            else
            {
                Response.StatusCode = 404;
                return RedirectToAction("Error", "Error");
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUploadDocument(DTODocUploadUpdRequest model)
        {
            ViewBag.OptionsRank = service.GetRank(1);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(userId);
            string user_name = string.Empty;
            string ipAddress;

            ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            var url = location.AbsoluteUri;

            if (ModelState.IsValid)
            {
                DocUpload? docUpload = context.DocUploads.FirstOrDefault(x => x.Updatedby == Convert.ToInt32(userId));
                if (docUpload != null)
                {
                    docUpload.DocName = model.DocName;
                    if (model.Doc_ != null)
                    {
                        string sourceFolderPhotoDB = "/WriteReadData/" + "Document";
                        string sourceFolderPhotoPhy = Path.Combine(hostingEnvironment.WebRootPath, "WriteReadData", "Document");

                        if (!Directory.Exists(sourceFolderPhotoPhy))
                            Directory.CreateDirectory(sourceFolderPhotoPhy);

                        string FileName = service.ProcessUploadedFile(model.Doc_, sourceFolderPhotoPhy,"");

                        string path = Path.Combine(sourceFolderPhotoPhy, FileName);

                        bool result = service.IsValidDocHeader(path);

                        if (!result)
                        {
                            ModelState.AddModelError("", "File format not correct");
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            goto xyz;
                        }
                        if (model.ExistingDocPath != null)
                        {
                            string f = Path.Join(hostingEnvironment.WebRootPath, docUpload.DocPath.Replace('/', '\\').ToString());
                            if (System.IO.File.Exists(f))
                            {
                                System.IO.File.Delete(f);
                            }
                        }
                        docUpload.DocPath = sourceFolderPhotoDB + "/" + FileName;
                    }
                    docUpload.Updatedby = Convert.ToInt32(userId);
                    docUpload.UpdatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                    await docUploadRepository.Update(docUpload);
                    TempData["success"] = "Updated Successfully.";
                    return RedirectToAction("Message");
                }
                else
                {
                    Response.StatusCode = 404;
                    return RedirectToAction("Error", "Error");
                }
            }
        xyz:
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public Task<ViewResult> Message()
        {
            return Task.FromResult(View());
        }
        [Authorize]
        public async Task<IActionResult> Download(string filepath)
        {

            string fullfilePath = Path.Join(hostingEnvironment.WebRootPath, filepath.Replace('/', '\\').ToString());
            var memory = new MemoryStream();
            using (var stream = new FileStream(fullfilePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, service.GetContentType(fullfilePath), Path.GetFileName(fullfilePath));
        }
    }
}
