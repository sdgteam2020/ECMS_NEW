using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MDispatchMode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte DispatchModeId { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string Description { get; set; } = string.Empty;
    }
}
