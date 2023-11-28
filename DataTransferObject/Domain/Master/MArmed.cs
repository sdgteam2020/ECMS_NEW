using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MArmed : Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArmedId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
