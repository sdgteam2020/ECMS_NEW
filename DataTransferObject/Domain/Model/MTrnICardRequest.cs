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
    public class MTrnICardRequest:Common
    {
        [Key]
        public int RequestId { get; set; }

        [ForeignKey("BasicDetail"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasicDetailId { get; set; }
        public BasicDetail? BasicDetail { get; set; }
        public Boolean Status { get; set; }
        [ForeignKey("MICardType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte TypeId { get; set; }
        public MICardType? MICardType { get; set; }
    }
}
