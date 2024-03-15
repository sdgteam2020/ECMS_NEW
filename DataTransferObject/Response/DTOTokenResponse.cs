using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class DTOTokenResponse
    {
      //  public string? API { get; set; }
        public Boolean CRL_OCSPCheck { get; set; }
        public string? CRL_OCSPMsg { get; set; }
        public string? Remarks { get; set; }
        public string? Thumbprint { get; set; }
        public string Status { get; set; }
        public Boolean TokenValid { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
        public string? issuer { get; set; }
        public string? subject { get; set; }
        public string? ArmyNo { get; set; }
        public string? Name { get; set; }
        public string? Rank { get; set; }
       

    
      
    }
}
