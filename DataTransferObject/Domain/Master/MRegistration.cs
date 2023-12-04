﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MRegistration : Common
    {
        [Key]
        public short RegistrationId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }

        [Required(ErrorMessage = "required!")]
        public int Type { get; set; }
    }
}
