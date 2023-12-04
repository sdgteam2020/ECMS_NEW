using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MRegistration : Common
    {
        [Key]
        public short RegistrationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public short Order { get; set; }
    }
}
