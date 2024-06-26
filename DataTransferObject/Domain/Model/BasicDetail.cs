﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject.Domain.Master;
using DataTransferObject.Constants;

namespace DataTransferObject.Domain.Model
{ 
    public class BasicDetail: Common
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BasicDetailId { get; set; }

        [StringLength(18)]
        [Column(TypeName = "varchar(18)")]
        public string FName { get; set; } = string.Empty;

        [StringLength(18)]
        [Column(TypeName = "varchar(18)")]
        public string? LName { get; set; }

        [StringLength(36)]
        [Column(TypeName = "varchar(36)")]
        public string NameAsPerRecord { get; set; } = string.Empty;

        [ForeignKey("MArmedType")]
        public byte ArmedId { get; set; }
        public MArmedType? Armed { get; set; }

        [ForeignKey("MRank")]
        public short RankId { get; set; }
        public MRank? Rank { get; set; }

        [Index("IX_BasicDetails_ArmyNo", IsClustered = false, IsUnique = true, Order = 1)]
        [StringLength(10)]
        [Column(TypeName = "varchar(10)")]
        public string ServiceNo { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime DOB { get; set; }

        [StringLength(50)] 
        [Column(TypeName = "varchar(50)")]
        public string? PlaceOfIssue { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime? DateOfIssue { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "datetime")]
        public DateTime DateOfCommissioning { get; set; }

        [ForeignKey("MRegimental")]
        public byte? RegimentalId { get; set; }
        public MRegimental? MRegimental { get; set; }

        [ForeignKey("MApplyFor"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte ApplyForId { get; set; }
        public MApplyFor? MApplyFor { get; set; }

        [ForeignKey("MUnit")]
        public int UnitId { get; set; }
        public MapUnit? Unit { get; set; }
        public byte IssuingAuthorityId { get; set; }
        public MIssuingAuthority? IssuingAuthority { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string PaperIcardNo { get; set; } = string.Empty;

    }
}
