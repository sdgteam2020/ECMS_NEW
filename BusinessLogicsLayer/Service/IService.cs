using DataTransferObject.Domain.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicsLayer.Service
{
    public interface IService
    {
        public IEnumerable<SelectListItem> GetSubmitType();
        public IEnumerable<SelectListItem> GetRegistrationType();
        public IEnumerable<SelectListItem> GetRole();
        public IEnumerable<SelectListItem> GetBloodGroup();
        public IEnumerable<SelectListItem> GetArmyNumberPart1();
        public IEnumerable<SelectListItem> GetArmyNumberPart3();
        public IEnumerable<SelectListItem> GetRank();
        public IEnumerable<SelectListItem> GetArmedType();
        public IEnumerable<SelectListItem> GetTypeOfUnit();
        public IEnumerable<SelectListItem> GetTemp();
        public IEnumerable<SelectListItem> GetComd();
        public IEnumerable<SelectListItem> GetCorps();
        public IEnumerable<SelectListItem> GetDiv();
        public bool IsValidHeader(string path);
        public bool IsValidDocHeader(string path);
        public string ProcessUploadedFile(IFormFile UploadDoc, string FileAddress);
        public bool IsImage(IFormFile postedFile);
        public string GetContentType(string path);
        public Dictionary<string, string> GetMimeTypes();
    }
}
