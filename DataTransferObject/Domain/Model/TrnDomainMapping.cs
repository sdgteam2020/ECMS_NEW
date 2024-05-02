using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnDomainMapping
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AspNetUsersId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UnitId { get; set; }
        public MapUnit? MapUnit { get; set; }

        [ForeignKey("MAppointment"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public short ApptId { get; set; }
        public MAppointment? MAppointment { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IsRO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IsIO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IsCO { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public Boolean IsORO { get; set; }

        [StringLength(6)]
        [Column(TypeName = "varchar(6)")]
        [Required(ErrorMessage = "ASCON Dialing  is required.")]
        [MinLength(6, ErrorMessage = "Minimum length of ASCON Dialing is six digit.")]
        [MaxLength(6, ErrorMessage = "Maximum length of ASCON Dialing is six digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Dialing code is invalid.")]
        public string DialingCode { get; set; } = string.Empty;

        [StringLength(5)]
        [Column(TypeName = "varchar(5)")]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MinLength(4, ErrorMessage = "Minimum length of Extension is four digit.")]
        [MaxLength(5, ErrorMessage = "Maximum length of Extension is five digit.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Extension is invalid.")]
        public string Extension { get; set; } = string.Empty;
        public int? MappedBy { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? MappedDate { get; set; }
        
        [NotMapped]
        public ApplicationRole? Role { get; set; }

        [NotMapped]
        public MRank? Rank { get; set; }
    }
}
