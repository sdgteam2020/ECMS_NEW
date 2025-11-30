using DataAccessLayer;
using DataTransferObject.Domain.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BusinessLogicsLayer.Service
{
    public class ServiceRepository:IService
    {   
        private readonly ApplicationDbContext context;
        public const int ImageMinimumBytes = 512;
        public decimal filesize { get; set; }
        public ServiceRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<MRegimental>> GetRegimentalListByArmedId(byte ArmedId)
        {
            return await context.Set<MRegimental>().Where(o => o.ArmedId == ArmedId).ToListAsync();
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

        /// <summary>
        /// Retrieves a list of armed types from the database, ordered by name, 
        /// and prepares them as <see cref="SelectListItem"/> objects for use in dropdown lists.
        /// </summary>
        /// <remarks>
        /// - Adds a default first item with text "Please Select" and null value.
        /// - Returns an <see cref="IEnumerable{SelectListItem}"/> that can be bound to a Razor view dropdown.
        /// </remarks>
        /// <returns>
        /// A collection of <see cref="SelectListItem"/> representing armed types, including a default "Please Select" option.
        /// </returns>
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
                        //postedFile.ContentType.ToLower() != "image/gif" &&
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
                //&& Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
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
        public bool IsValidZipHeader(string path)
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
                    case "00-01-00-00":
                        output = "zip";
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
        public bool IsValidBase64(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }
        public string CheckFirstTwoChars(string input)
        {
            if (input.Length >= 2 && Char.IsLetter(input[0]) && Char.IsLetter(input[1]))
            {
                return input.Substring(0, 2).ToUpper();
            }
            else
            {
                return string.Empty; // Return empty if not alphabetic or less than 2 characters
            }
        }

    }
}
