using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class DomainMapping:Common
    {
        [Key]
        public int Id { get; set; }
        public int DomainId { get; set; }
        public int ArmyNO { get; set; }
    }
}
