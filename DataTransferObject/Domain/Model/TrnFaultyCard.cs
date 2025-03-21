using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;

namespace DataTransferObject.Domain.Model
{
    public class TrnFaultyCard : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrnFaultyCardId { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string RemarksIds { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? OtherRemark { get; set; }
        
        [ForeignKey("MFaultyStage"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte FaultyStageId { get; set; }
        public MFaultyStage? MFaultyStage { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }
    }
}
