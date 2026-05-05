using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MRegimental : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegId is number.")]
        public byte RegId { get; set; }
        
        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/()\-]+(?: [A-Za-z0-9&\/()\-]+)*$", ErrorMessage = "Regimental name must contain at least one alphabet. Only A-Z, a-z, 0-9, & - / ( ) and single space allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50, ErrorMessage = "Maximum length of Abbreviation is fifty character.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/()\-]+(?: [A-Za-z0-9&\/()\-]+)*$", ErrorMessage = "Abbreviation name must contain at least one alphabet. Only A-Z, a-z, 0-9, & - / ( ) and single space allowed.")]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10, ErrorMessage = "Maximum length of Abbreviation is ten character.")]
        public string Abbreviation { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/().,\-]+(?: [A-Za-z0-9&\/().,\-]+)*$", ErrorMessage = "Location must contain at least one alphabet. Only A-Z, a-z, 0-9, &, -, /, (, ), dot, comma and single space allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50, ErrorMessage = "Maximum length of Location is fifty character.")]
        public string Location { get; set; } = string.Empty;

   
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        [ForeignKey("MArmedType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ArmedId { get; set; }
        public MArmedType? Armed { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UnitId { get; set; }
        public MapUnit? MapUnit { get; set; }

    }
}
