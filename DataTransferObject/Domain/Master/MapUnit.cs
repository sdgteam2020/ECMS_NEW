using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MapUnit : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UnitMapId { get; set; }
      

        [ForeignKey("MUnit") ,DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "required!")]
        public int UnitId { get; set; }
        public MUnit? MUnit { get; set; }
        
        public int UnitType { get; set; }   
        [ForeignKey("Comd")]
        public byte ComdId { get; set; }

        public MComd? Comd { get; set; }

        [ForeignKey("MCorps")]
        public byte CorpsId { get; set; }
        public MCorps? Corps { get; set; }

        [ForeignKey("MDiv")]

        [Required(ErrorMessage = "required!")]

        public byte DivId { get; set; }
        public MDiv? Div { get; set; }

        [ForeignKey("MBde")]

        [Required(ErrorMessage = "required!")]
        public byte BdeId { get; set; }
        public MBde? Bde { get; set;}


        [ForeignKey("MPSO")]

        [Required(ErrorMessage = "required!")]
        public byte PsoId { get; set; }
        public MPSO? MPSO { get; set; }

        [ForeignKey("MFmnBranches")]

        [Required(ErrorMessage = "required!")]
        public byte FmnBranchID { get; set; }
        public MFmnBranches? MFmnBranches { get; set; }

        [ForeignKey("MSubDte")]

        [Required(ErrorMessage = "required!")]
        public byte SubDteId { get; set; }
        public MSubDte? MSubDte { get; set; }
    }
}
