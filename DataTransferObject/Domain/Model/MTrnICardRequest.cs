using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class MTrnICardRequest:Common
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        //[ForeignKey("BasicDetail"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BasicDetailId { get; set; }
        //public BasicDetail? BasicDetail { get; set; }
        
        [ForeignKey("MTrnICardStatus"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte StatusId { get; set; }
        public MTrnICardStatus? MTrnICardStatus { get; set; }

        [ForeignKey("MICardType"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte TypeId { get; set; }
        public MICardType? MICardType { get; set; }

        [ForeignKey("Registration"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte RegistrationId { get; set; }
        public MRegistration? Registration { get; set; }

        [ForeignKey("TrnDomainMapping"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrnDomainMappingId { get; set; }
        public TrnDomainMapping? TrnDomainMapping { get; set; }

        [ForeignKey("MRecordOffice"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte RecordOfficeId { get; set; }
        public MRecordOffice? MRecordOffice { get; set; }

        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string? CardSerialNo { get; set; } 

        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string? ChipNo { get; set; }

        [ForeignKey("ApplicationUserCardPrinted"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CardPrintedByAspNetUserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? ApplicationUserCardPrinted { get; set; }

        [ForeignKey("UserProfileCardPrinted"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CardPrintedByUserId { get; set; }
        public MUserProfile? UserProfileCardPrinted { get; set; }
        public DateTime? CardPrintedOn { get; set; }

        [ForeignKey("ApplicationUserCardExported"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CardExportedByAspNetUserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? ApplicationUserCardExported { get; set; }

        [ForeignKey("UserProfileCardExported"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CardExportedByUserId { get; set; }
        public MUserProfile? UserProfileCardExported { get; set; }
        public DateTime? CardExportedOn { get; set; }
    }
}
