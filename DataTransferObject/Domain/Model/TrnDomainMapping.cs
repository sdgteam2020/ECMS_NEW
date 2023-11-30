using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnDomainMapping
    {
        public int Id { get; set; }
        public int DomianId { get; set; }
        public int? UserId { get; set; }
        public int UnitId { get; set; }
    }
}
