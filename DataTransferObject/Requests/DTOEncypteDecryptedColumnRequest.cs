using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOEncypteDecryptedColumnRequest
    {
        public string ArmyNo { get; set; } = string.Empty;
        public int RequestIdForFaulty { get; set; }
    }
}
