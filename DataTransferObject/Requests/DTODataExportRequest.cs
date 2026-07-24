using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTODataExportRequest
    {
        public int[] Ids { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "IsJco is number.")]
        public int IsJco { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public int StepId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "DataExportType is number.")]
        public byte DataExportType { get; set; }

        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string publicKey { get; set; } = string.Empty;

        [RegularExpression(@"^[\w ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string privateKey { get; set; } = string.Empty;
        public int CardExportedByAspNetUserId { get; set; }
        public int CardExportedByUserId { get; set; }
    }
}
