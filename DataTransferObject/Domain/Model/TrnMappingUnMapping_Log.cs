using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnMappingUnMapping_Log:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TrnMappUnMapLogId { get; set; }
        public int TDMId { get; set; }
        public int UserId { get; set; }
        public int DeregisterUserId { get; set; }
    }
}
