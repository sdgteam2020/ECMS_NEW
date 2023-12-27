using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MTrnIdentityInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InfoId { get; set; }
        [ForeignKey("BasicDetail"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasicDetailId { get; set; }
        public BasicDetail? BasicDetail { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string IdenMark1 { get; set; } = string.Empty;
        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string IdenMark2 { get; set; } = string.Empty;

        [MaxLength(12)]
        [MinLength(12)]
        public long AadhaarNo { get; set; }
        public float Height { get; set; }

        public int BloodGroup { get; set; }
    }
}
