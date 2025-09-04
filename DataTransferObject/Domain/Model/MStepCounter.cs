using DataTransferObject.Domain.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ApplyForId { get; set; }
        public MApplyFor? MApplyFor { get; set; }
        
        [NotMapped]
        public string UnitName { get; set; } = string.Empty;
        
        [NotMapped]
        public string Flag { get; set; } = string.Empty;
    }
}
