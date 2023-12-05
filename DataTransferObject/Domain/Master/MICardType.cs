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
        public byte TypeId { get; set; }
        public string Name { get; set; }
    }
}
