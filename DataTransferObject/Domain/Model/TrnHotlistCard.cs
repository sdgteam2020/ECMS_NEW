using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnHotlistCard : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotlistCardId { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string RemarksIds { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? Remark { get; set; }

        [ForeignKey("UserProfileUserUpdate"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UpdatedbyUserId { get; set; }
        public MUserProfile? UserProfileUserUpdate { get; set; }
    }
}
