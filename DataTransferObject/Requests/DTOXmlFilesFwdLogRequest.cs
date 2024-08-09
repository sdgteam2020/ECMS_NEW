using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOXmlFilesFwdLogRequest
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string XmlFiles { get; set; } = string.Empty;
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int IsActive { get; set; }


    }
    public class DTOXmlFilesForUpdate
    {
        public int Id { get; set; }
      
        public object jsonfile { get; set; }



    }
}
