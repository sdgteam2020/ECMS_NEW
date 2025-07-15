using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnDispatchCardMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DispatchCardMappingId { get; set; }

        [ForeignKey("TrnDispatchCard"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DispatchCardId { get; set; }
        public TrnDispatchCard? TrnDispatchCard { get; set; }
        
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string ChipNo { get; set; }=string.Empty;

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }
    }
}
