using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ArmedType
    {
        [Display(Name = "Arms")]
        Arms = 1,
        [Display(Name = "Service")]
        Service = 2,
        [Display(Name = "Corps")]
        Corps =3
    }
}
