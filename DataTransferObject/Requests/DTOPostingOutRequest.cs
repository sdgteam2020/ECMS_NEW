using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Requests
{
    public class DTOPostingOutRequest
    {
        [Required(ErrorMessage = "ReasonId is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Id is number.")]
        public byte ReasonId { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [Required(ErrorMessage = "Authority is required.")]
        public string Authority { get; set; }=string.Empty;

        [Required(ErrorMessage = "SOSDate is required.")]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime SOSDate { get; set; }

        [Required(ErrorMessage = "ToUnitID is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToUnitID is number.")]
        public int ToUnitID { get; set; }

        [Required(ErrorMessage = "ToAspNetUsersId is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToAspNetUsersId is number.")]
        public int ToAspNetUsersId { get; set; }

        [Required(ErrorMessage = "ToUserID is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ToUserID is number.")]
        public int ToUserID { get; set; }

        [Required(ErrorMessage = "RequestId is required.")]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "RequestId is number.")]
        public int RequestId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "TrnFwdId is number.")]
        public int? TrnFwdId { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DispatchedOn { get; set; }
        
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string? RefNo { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DispatchUpdatedOn { get; set; }
    }
}
