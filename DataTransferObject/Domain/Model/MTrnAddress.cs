using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MTrnAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [ForeignKey("BasicDetail"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasicDetailId { get; set; }
        public BasicDetail? BasicDetail { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string? State { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string? District { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string? PS { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string? PO { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string? Tehsil { get; set; }
        [Column(TypeName = "VARCHAR(50)")]
        public string? Village { get; set; }
        public int? PinCode { get; set; }
    }
}
