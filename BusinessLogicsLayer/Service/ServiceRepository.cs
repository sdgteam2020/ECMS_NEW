﻿using DataAccessLayer;
using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Service
{
    public class ServiceRepository:IService
    {   
        private readonly ApplicationDbContext context;
        private readonly ILogger<ServiceRepository> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly DateTime EndDate;
        private readonly static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        public const int ImageMinimumBytes = 512;
        public decimal filesize { get; set; }
        public ServiceRepository(ApplicationDbContext context, ILogger<ServiceRepository> logger, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            _logger = logger;
            this.userManager = userManager;
            EndDate = new DateTime(2299, 01, 01);
        }
        public IEnumerable<SelectListItem> GetRegistrationType()
        {
            var RegType = new List<SelectListItem>
            {
                new SelectListItem{ Text="Please Select", Value = null },
                new SelectListItem{ Text="Officer", Value = "1" },
                new SelectListItem{ Text="JCOs/OR", Value = "2" },
            };
            return new SelectList(RegType, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetComd()
        {
            var comd = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="SC", Value = "SC" },
                new SelectListItem{ Text="EC", Value = "EC" },
                new SelectListItem{ Text="WC", Value = "WC" },
                new SelectListItem{ Text="CC", Value = "CC" },
                new SelectListItem{ Text="NC", Value = "NC" },
                new SelectListItem{ Text="SWC", Value = "SWC" },
                new SelectListItem{ Text="TRG", Value = "TRG" },
            };
            return new SelectList(comd, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetCorps()
        {
            var corps = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="1 Corps", Value = "1 Corps" },
                new SelectListItem{ Text="2 Corps", Value = "2 Corps" },
                new SelectListItem{ Text="3 Corps", Value = "3 Corps" },
                new SelectListItem{ Text="4 Corps", Value = "4 Corps" },
                new SelectListItem{ Text="9 Corps", Value = "9 Corps" },
                new SelectListItem{ Text="10 Corps", Value = "10 Corps" },
                new SelectListItem{ Text="11 Corps", Value = "11 Corps" },
                new SelectListItem{ Text="12 Corps", Value = "12 Corps" },
                new SelectListItem{ Text="14 Corps", Value = "14 Corps" },
                new SelectListItem{ Text="15 Corps", Value = "15 Corps" },
                new SelectListItem{ Text="16 Corps", Value = "16 Corps" },
                new SelectListItem{ Text="17 Corps", Value = "17 Corps" },
            };
            return new SelectList(corps, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetDiv()
        {
            var divs = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="1 Div", Value = "1 Div" },
                new SelectListItem{ Text="2 Div", Value = "2 Div" },
                new SelectListItem{ Text="3 Div", Value = "3 Div" },
                new SelectListItem{ Text="4 Div", Value = "4 Div" },
                new SelectListItem{ Text="5 Div", Value = "5 Div" },
                new SelectListItem{ Text="6 Div", Value = "6 Div" },
                new SelectListItem{ Text="7 Div", Value = "7 Div" },
                new SelectListItem{ Text="8 Div", Value = "8 Div" },
                new SelectListItem{ Text="9 Div", Value = "9 Div" },
                new SelectListItem{ Text="10 Div", Value = "10 Div" },
                new SelectListItem{ Text="11 Div", Value = "11 Div" },
                new SelectListItem{ Text="12 Div", Value = "12 Div" },
                new SelectListItem{ Text="14 Div", Value = "14 Div" },
                new SelectListItem{ Text="15 Div", Value = "15 Div" },
            };
            return new SelectList(divs, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetTemp()
        {
            var rakes = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="Temp 1", Value = "Temp 1" },
                new SelectListItem{ Text="Temp 2", Value = "Temp 2" },
                new SelectListItem{ Text="Temp 3", Value = "Temp 3" },
                new SelectListItem{ Text="Temp 4", Value = "Temp 4" },
                new SelectListItem{ Text="Temp 5", Value = "Temp 5" },
                new SelectListItem{ Text="Temp 6", Value = "Temp 6" },
                new SelectListItem{ Text="Temp 7", Value = "Temp 7" },
                new SelectListItem{ Text="Temp 8", Value = "Temp 8" },
                new SelectListItem{ Text="Temp 9", Value = "Temp 9" },
                new SelectListItem{ Text="Temp 10", Value = "Temp 10" },
                new SelectListItem{ Text="Temp 11", Value = "Temp 11" },
                new SelectListItem{ Text="Temp 12", Value = "Temp 12" },
            };
            return new SelectList(rakes, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetTypeOfUnit()
        {
            var rakes = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="Directorate", Value = "Directorate" },
                new SelectListItem{ Text="OMG Branch", Value = "OMG Branch" },
                new SelectListItem{ Text="Formation / Unit", Value = "Formation / Unit" },
            };
            return new SelectList(rakes, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetArmyNumberPart1()
        {
            var rakes = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="IC", Value = "IC" },
                new SelectListItem{ Text="SS", Value = "SS" },
                new SelectListItem{ Text="WS", Value = "WS" },
            };
            return new SelectList(rakes, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetArmyNumberPart3()
        {
            var rakes = new List<SelectListItem>
            {
                new SelectListItem{ Text="-- Select --", Value = null },
                new SelectListItem{ Text="A", Value = "A" },
                new SelectListItem{ Text="B", Value = "B" },
                new SelectListItem{ Text="C", Value = "C" },
                new SelectListItem{ Text="D", Value = "D" },
                new SelectListItem{ Text="E", Value = "E" },
                new SelectListItem{ Text="F", Value = "F" },
                new SelectListItem{ Text="G", Value = "G" },
                new SelectListItem{ Text="H", Value = "H" },
                new SelectListItem{ Text="I", Value = "I" },
                new SelectListItem{ Text="J", Value = "J" },
                new SelectListItem{ Text="K", Value = "K" },
                new SelectListItem{ Text="L", Value = "L" },
                new SelectListItem{ Text="M", Value = "M" },
                new SelectListItem{ Text="N", Value = "N" },
                new SelectListItem{ Text="O", Value = "O" },
                new SelectListItem{ Text="P", Value = "P" },
                new SelectListItem{ Text="Q", Value = "Q" },
                new SelectListItem{ Text="R", Value = "R" },
                new SelectListItem{ Text="S", Value = "S" },
                new SelectListItem{ Text="T", Value = "T" },
                new SelectListItem{ Text="U", Value = "U" },
                new SelectListItem{ Text="V", Value = "V" },
                new SelectListItem{ Text="W", Value = "W" },
                new SelectListItem{ Text="X", Value = "X" },
                new SelectListItem{ Text="Y", Value = "Y" },
                new SelectListItem{ Text="Z", Value = "Z" },
            };
            return new SelectList(rakes, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetRegistration()
        {
            var RegistrationOptions = context.MRegistration.OrderBy(o => o.Order)
                .Select(a =>
                  new SelectListItem
                  {
                      Value = a.RegistrationId.ToString(),
                      Text = a.Name
                  }).ToList();

            var ddfirst = new SelectListItem()
            {
                Value = null,
                Text = "Please Select"
            };
            RegistrationOptions.Insert(0, ddfirst);
            return new SelectList(RegistrationOptions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetArmedCat()
        {
            var ArmedCatOptions = context.MArmedCats.OrderBy(o => o.Order)
                .Select(a =>
                  new SelectListItem
                  {
                      Value = a.ArmedCatId.ToString(),
                      Text = a.Name
                  }).ToList();

            var ddfirst = new SelectListItem()
            {
                Value = null,
                Text = "Please Select"
            };
            ArmedCatOptions.Insert(0, ddfirst);
            return new SelectList(ArmedCatOptions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetRegimentalDDLIdSelected(int ArmedId)
        {
            try
            {
                var RegimentalOptions = context.MRegimental.Where(s => s.ArmedId == ArmedId)
                    .Select(a =>
                      new SelectListItem
                      {
                          Value = a.RegId.ToString(),
                          Text = a.Name
                      }).ToList();

                var ddfirst = new SelectListItem()
                {
                    Value = null,
                    Text = "Please Select"
                };
                RegimentalOptions.Insert(0, ddfirst);
                return new SelectList(RegimentalOptions, "Value", "Text");
            }
            catch (Exception ex)
            {
                _logger.LogError(1001, ex, "ServiceRepository->GetRegimentalDDLIdSelected");
                return null;
            }

        }
        public async Task<List<MRegimental>> GetRegimentalListByArmedId(byte ArmedId)
        {
            return await context.Set<MRegimental>().Where(o => o.ArmedId == ArmedId).ToListAsync();
        }
        public IEnumerable<SelectListItem> GetAppointment(short FormationId)
        {
            var AppointmentOptions = context.MAppointment.OrderBy(o => o.AppointmentName)
                 .Select(a =>
                   new SelectListItem
                   {
                       //Value = a.FormationId.ToString(),
                       //Text = a.AppointmentName,
                   }).ToList();
            var ddfirst = new SelectListItem()
            {
                Value = null,
                Text = "Select Appointment"
            };
            AppointmentOptions.Insert(0, ddfirst);
            return new SelectList(AppointmentOptions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetRank(byte Type)
        {
            var RankOptions = context.MRank.Where(x=>x.ApplyForId== Type).OrderBy(o => o.Orderby)
                 .Select(a =>
                   new SelectListItem
                   {
                       Value = a.RankId.ToString(),
                       Text = a.RankName,
                   }).ToList();
            var ddfirst = new SelectListItem()
            {
                Value = null,
                Text = "Select Rank"
            };
            RankOptions.Insert(0, ddfirst);
            return new SelectList(RankOptions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetArmedType()
        {
            var ArmedOptions = context.MArmedType.OrderBy(o => o.ArmedName)
                 .Select(a =>
                   new SelectListItem
                   {
                       Value = a.ArmedId.ToString(),
                       Text = a.ArmedName,
                   }).ToList();
            var ddfirst = new SelectListItem()
            {
                Value = null,
                Text = "Please Select"
            };
            ArmedOptions.Insert(0, ddfirst);
            return new SelectList(ArmedOptions, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetRole()
        {
            var roles = new List<SelectListItem>
            {
                new SelectListItem{ Text="Please Select", Value = null },
                new SelectListItem{ Text="User", Value = "user" },
            };
            return new SelectList(roles, "Value", "Text");
        }
        public IEnumerable<SelectListItem> GetBloodGroup()
        {
            var ArmedOptions = context.MBloodGroup
                 .Select(a =>
                   new SelectListItem
                   {
                       Value = a.BloodGroupId.ToString(),   
                       Text = a.BloodGroup
                   }).ToList();
            var ddfirst = new SelectListItem()
            {
                Value = null,
                Text = "Please Select"
            };
            ArmedOptions.Insert(0, ddfirst);
            return new SelectList(ArmedOptions, "Value", "Text");

 
            //var roles = new List<SelectListItem>
            //{
            //    new SelectListItem{ Text="Please Select", Value = null },
            //    new SelectListItem{ Text="A+", Value = "1" },
            //    new SelectListItem{ Text="A-", Value = "2" },
            //    new SelectListItem{ Text="B+", Value = "3" },
             
            //};
            //return new SelectList(roles, "Value", "Text");
        }
        public string ProcessUploadedFile(IFormFile UploadDoc, string FileAddress,string FileName)
        {
            string? uniqueFileName = null;
            string ext = System.IO.Path.GetExtension(UploadDoc.FileName);
            uniqueFileName = FileName + ext;// Guid.NewGuid().ToString() + ext;
            string filePath = Path.Combine(FileAddress, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                UploadDoc.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
        public bool IsImage(IFormFile postedFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png")
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.OpenReadStream().CanRead)
                {
                    return false;
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------ 
                if (postedFile.Length < ImageMinimumBytes)
                {
                    return false;
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try
            {
                using (var bitmap = new System.Drawing.Bitmap(postedFile.OpenReadStream()))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }
        public bool IsValidHeader(string path)
        {
            string msg = "";
            bool retMsg = false;

            string[] file_hexa_signature = { "25-50-44-46-2D-31-2E", "50-4B-03-04-14-00-06", "D0-CF-11-E0-A1-B1-1A", "47-49-46-38-39-61-20", "FF-D8-FF-E0-00-10-4A", "89-50-4E-47-0D-0A-1A" };
            if (path != null && path != "")
            {
                BinaryReader reader = new BinaryReader(new FileStream(Convert.ToString(path), FileMode.Open, FileAccess.Read, FileShare.None));
                reader.BaseStream.Position = 0x0;     // The offset you are reading the data from
                byte[] data = reader.ReadBytes(0x10); // Read 16 bytes into an array         
                string data_as_hex = BitConverter.ToString(data);
                reader.Close();

                // substring to select first 20 characters from hexadecimal array
                string fUpload = data_as_hex.Substring(0, 11);
                string? output = null;
                bool isGeniun = false;

                switch (fUpload)
                {
                    case "47-49-46-38":
                        output = "gif";
                        isGeniun = true;
                        break;
                    case "FF-D8-FF-E0":
                        output = "jpeg";
                        isGeniun = true;
                        break;
                    case "FF-D8-FF-E1":
                        output = "jpg";
                        isGeniun = true;
                        break;
                    case "89-50-4E-47":
                        output = "png";
                        isGeniun = true;
                        break;
                    case null:
                        output = "notmatched";
                        isGeniun = false;
                        break;
                }

                msg = output;

                if (!isGeniun)
                    retMsg = isGeniun;
                else
                    retMsg = isGeniun;
            }
            return retMsg;
        }
        public bool IsValidDocHeader(string path)
        {
            string msg = "";
            bool retMsg = false;

            if (path != null && path != "")
            {
                BinaryReader reader = new BinaryReader(new FileStream(Convert.ToString(path), FileMode.Open, FileAccess.Read, FileShare.None));
                reader.BaseStream.Position = 0x0;     // The offset you are reading the data from
                byte[] data = reader.ReadBytes(0x10); // Read 16 bytes into an array         
                string data_as_hex = BitConverter.ToString(data);
                reader.Close();

                // substring to select first 20 characters from hexadecimal array
                string fUpload = data_as_hex.Substring(0, 11);
                string? output = null;
                bool isGeniun = false;

                switch (fUpload)
                {
                    case "25-50-44-46":
                        output = "pdf";
                        isGeniun = true;
                        break;
                    case "50-4B-03-04":
                        output = "docx-xlsx-pptx";
                        isGeniun = true;
                        break;
                    case null:
                        output = "notmatched";
                        isGeniun = false;
                        break;
                }

                msg = output;

                if (!isGeniun)
                    retMsg = isGeniun;
                else
                    retMsg = isGeniun;
            }
            return retMsg;
        }
        public string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        public Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

       
    }
}
