using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
