using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MFaultyStage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte FaultyStageId { get; set; }
        
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string Name { get; set; } = string.Empty;
    }
}
