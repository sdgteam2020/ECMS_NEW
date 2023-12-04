using DataTransferObject.Domain.Master;
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
        public MTrnICardRequest? MTrnICardRequest { get; set; }
        public short StepId { get; set; }
        public MStepCounterStep? MStepCounterStep { get; set; }
    }
}
