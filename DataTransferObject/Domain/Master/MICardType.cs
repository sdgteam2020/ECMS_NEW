using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MICardType
    {
        [Key]
        public int TypeId { get; set; }
        public string ICardType { get; set; }
    }
}
