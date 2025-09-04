using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Constants
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ArmedType
    {
        [Display(Name = "Arms")]
        Arms = 1,
        [Display(Name = "Service / Corps")]
        Service = 2,
    }
}
