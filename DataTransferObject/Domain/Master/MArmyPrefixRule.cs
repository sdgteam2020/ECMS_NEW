using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MArmyPrefixRule:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public byte Id { get; set; }

        [Required(ErrorMessage = "required!")]
        [Column(TypeName = "varchar(3)")]
        [MaxLength(3, ErrorMessage = "Maximum length of Prefix is three characters.")]
        public string Prefix { get; set; }   // DR, JC, NO

        [Required(ErrorMessage = "required!")]
        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public byte ApplyForId { get; set; }

        public byte MinDigits { get; set; }

        public byte MaxDigits { get; set; }

        public byte Order { get; set; }

        public bool StorePrefix { get; set; } // false for OR (NO)
    }
}
