using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Constants
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RegistrationType
    {
        [Display(Name = "Select One")]
        Select = 0,
        [Display(Name = "Officer")]
        Officer = 1,
        [Display(Name = "JCOs/OR")]
        JCO = 2,
    }
}
