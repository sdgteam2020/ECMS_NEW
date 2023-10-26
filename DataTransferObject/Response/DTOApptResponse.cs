using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOApptResponse
    {
        public int ApptId { get; set; }
        public string ApptName { get; set; }
        public int FormationId { get; set; }
        public string FormationName { get; set; }

    }
}
