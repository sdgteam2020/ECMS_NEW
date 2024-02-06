﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DataTransferObject.Domain.Master
{
    public class MRank:Common
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Rank Id is number.")]
        public short RankId { get; set; }

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50, ErrorMessage = "Maximum length of Rank Name is fifty character.")]
        public string RankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "required!")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        [Column(TypeName = "varchar(30)")]
        [MaxLength(30, ErrorMessage = "Maximum length of Rank Abbreviation is thirty character.")]
        public string RankAbbreviation { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RankId is number.")]
        public short Orderby { get; set; }
        
        [Required(ErrorMessage = "required!")]
        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression(@"^[\d]+$", ErrorMessage = "ApplyForId is number.")]
        public byte ApplyForId { get; set; }
        public MApplyFor? MApplyFor { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "Rank Cd is number.")]
        public int? rank_cd { get; set; }
    }
}
