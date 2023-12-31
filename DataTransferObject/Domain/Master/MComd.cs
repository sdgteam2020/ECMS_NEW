﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MComd: Common
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte ComdId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string ComdName { get; set; }
        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        public string ComdAbbreviation { get; set; }
        [Required(ErrorMessage = "required!")]
        
        public int Orderby { get; set; }
    }
}
