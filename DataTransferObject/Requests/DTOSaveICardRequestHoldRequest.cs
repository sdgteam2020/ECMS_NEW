using DataTransferObject.Domain;
using DataTransferObject.Domain.Model;
using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOSaveICardRequestHoldRequest
    {
        [Range(0, int.MaxValue, ErrorMessage = "ICardHoldId must be a non-negative integer.")]
        public int ICardHoldId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RequestId is number.")]
        [Range(1, int.MaxValue, ErrorMessage = "RequestId must be a positive integer.")]
        public int RequestId { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool IsHold { get; set; }

        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of Hold Reason is fifty character.")]
        public string HoldReason { get; set; } = string.Empty;


        [RegularExpression(@"^[\w \.]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        [MaxLength(50, ErrorMessage = "Maximum length of UnHold Reason is fifty character.")]
        public string? UnHoldReason { get; set; }

    }
}
