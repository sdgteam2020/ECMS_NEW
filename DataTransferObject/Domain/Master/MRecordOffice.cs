using DataTransferObject.Domain.Model;
using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MRecordOffice : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RecordOffice Id is number.")]
        public byte RecordOfficeId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/()\-]+(?: [A-Za-z0-9&\/()\-]+)*$", ErrorMessage = "Record Office name must contain at least one alphabet. Only A-Z, a-z, 0-9, & - / ( ) and single space allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50, ErrorMessage = "Maximum length of Abbreviation is fifty character.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/()\-]+(?: [A-Za-z0-9&\/()\-]+)*$", ErrorMessage = "Record Office Abbreviation name must contain at least one alphabet. Only A-Z, a-z, 0-9, & - / ( ) and single space allowed.")]
        [Column(TypeName = "varchar(10)")]
        [MaxLength(15, ErrorMessage = "Maximum length of Abbreviation is ten character.")]
        public string Abbreviation { get; set; } = string.Empty;

        [ForeignKey("MArmedType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ArmedId is number.")]
        public byte ArmedId { get; set; }
        public MArmedType? MArmedType { get; set; }

        [ForeignKey("TrnDomainMapping"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "TDMId is number.")]
        public int? TDMId { get; set; }
        public TrnDomainMapping? TrnDomainMapping { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UnitId { get; set; }
        public MapUnit? MapUnit { get; set; }

        [StringLength(150)]
        [Column(TypeName = "varchar(150)")]
        [RegularExpression(@"^(?![0-9 ]+$)(?=.*[A-Za-z])[A-Za-z0-9&\/().,\-]+(?: [A-Za-z0-9&\/().,\-]+)*$", ErrorMessage = "Message name must contain at least one alphabet. Only A-Z, a-z, 0-9, &, -, /, (, ), dot, comma and single space allowed.")]
        [MaxLength(150, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        public string? Message { get; set; }
    }
}
