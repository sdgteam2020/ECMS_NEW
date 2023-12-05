using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MStepCounter:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }
        [ForeignKey("MStepCounterStep"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte StepId { get; set; }
        public MStepCounterStep? MStepCounterStep { get; set; }
    }
}
