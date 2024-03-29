﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MArmedCat : Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte ArmedCatId { get; set; }
        
        [Column(TypeName = "varchar(25)")]
        public string Name { get; set; } = string.Empty;
        public byte Order { get; set; }
    }
}
