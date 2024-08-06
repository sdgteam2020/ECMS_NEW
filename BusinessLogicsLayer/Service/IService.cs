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
        public Task<List<MRegimental>> GetRegimentalListByArmedId(byte ArmedId);
        public IEnumerable<SelectListItem> GetRank(byte Type);
        public IEnumerable<SelectListItem> GetArmedType();
        public bool IsValidHeader(string path);
        public bool IsValidDocHeader(string path);
        public string ProcessUploadedFile(IFormFile UploadDoc, string FileAddress,string FileName);
        public bool IsImage(IFormFile postedFile);
        public string GetContentType(string path);
        public Dictionary<string, string> GetMimeTypes();
    }
}
