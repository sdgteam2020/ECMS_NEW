using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MStepCounter:Common
    {
        [Key]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int Step { get; set; }
    }
}
