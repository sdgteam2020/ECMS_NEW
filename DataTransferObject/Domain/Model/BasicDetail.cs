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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BasicDetailId { get; set; }

        [StringLength(36)]
        [Column(TypeName = "nvarchar(36)")]
        public string Name { get; set; } = string.Empty;
        
        [ForeignKey("MArmedType")]
        public byte ArmedId { get; set; }
        public MArmedType? Armed { get; set; }

        [ForeignKey("MRank")]
        public byte RankId { get; set; }
        public MRank? Rank { get; set; }

        [Index("IX_BasicDetails_ServiceNo", IsClustered = false, IsUnique = true, Order = 1)]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string ServiceNo { get; set; } = string.Empty;

        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string IdentityMark { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public int Height { get; set; }

        [StringLength(4)]
        [Column(TypeName = "nvarchar(4)")]
        public string AadhaarNo { get; set; } = string.Empty;

        [StringLength(3)]
        [Column(TypeName = "nvarchar(3)")]
        public string BloodGroup { get; set; } = string.Empty;

        [StringLength(50)] 
        [Column(TypeName = "nvarchar(50)")]
        public string PlaceOfIssue { get; set; } = string.Empty;
        public DateTime DateOfIssue { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string IssuingAuth { get; set; } = string.Empty;

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string SignatureImagePath { get; set; } = string.Empty;  

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string PhotoImagePath { get; set; } = string.Empty;
        public DateTime DateOfCommissioning { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string PermanentAddress { get; set; } = string.Empty;
        public byte StatusLevel { get; set; }
        
        [ForeignKey("MRegimental")]
        public byte? RegimentalId { get; set; }
        public MRegimental? Regimental { get; set; }

        public byte ApplyForId { get; set; }
        //[ForeignKey("MRegistration")]
        //public byte RegistrationId { get; set; }
        //public MRegistration? Registration { get; set; }

        [ForeignKey("MUnit")]
        public int UnitId { get; set; }
        public MUnit? Unit { get; set; }
        public int Step { get; set; }
        public bool IsSubmit { get; set; }

        [NotMapped]
        public bool success { get; set; }

        [NotMapped]
        public string? message { get; set; } = string.Empty;

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
