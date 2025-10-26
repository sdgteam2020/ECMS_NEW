using DataTransferObject.Localize;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOTrnNotificationRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "NotificationId is number.")]
        public int NotificationId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public byte StepId { get; set; }

        [RegularExpression(@"^[a-zA-Z]*$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public bool Read { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "DisplayId is number.")]
        public int DisplayId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "SentAspNetUsersId is number.")]
        public int SentAspNetUsersId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "SentAspNetUsersId is number.")]
        public int ReciverAspNetUsersId { get; set; }
        public required int[] RequestIds { get; set; }
        public string? Url { get; set; }
    }
}
