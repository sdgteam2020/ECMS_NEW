using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MTrnICardRequest:Common
    {
        [Key]
        public int RequestId { get; set; }
        public int BasicDetailId { get; set; }
        public Boolean Status { get; set; }
        public int TypeId { get; set; }
    }
}
