using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Localize;

namespace DataTransferObject.Requests
{
    public class DTOProfileRequest
    {
        public int UserId { get; set; }

        [Display(Name = "ArmyNo", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmyNo { get; set; } = string.Empty;
        public byte RankId { get; set; }
        public MRank? MRank { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "RequiredError")]
        [MaxLength(50, ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "MaxLengthError")]
        [RegularExpression(@"^[\w ]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string Name { get; set; } = string.Empty;
        public byte ApptId { get; set; }
        public MAppointment? MAppointment { get; set; }
        public Boolean IntOffr { get; set; }
    }
}
