using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Identitytable;

namespace DataTransferObject.Domain.Model
{
    public class TrnDistributeCard : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DistributeCardId { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DistributedOn { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Remark { get; set; }

        [ForeignKey("UserProfileUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UpdatedbyUserId { get; set; }
        public MUserProfile? UserProfileUserUpdate { get; set; }
    }
}
