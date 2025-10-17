using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class MTrnNotification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("MStepCounterStep"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte StepId { get; set; }
        public MStepCounterStep? MStepCounterStep { get; set; }
        public bool Read { get; set; }
        [ForeignKey("Display"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DisplayId { get; set; }
        public MTrnNotificationDisplay? Display { get; set; }

        [ForeignKey("SentAspNetUsers"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SentAspNetUsersId { get; set; }

        public ApplicationUser? SentAspNetUsers { get; set; }

        [ForeignKey("ReciverAspNetUsers"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ReciverAspNetUsersId { get; set; }

        public ApplicationUser? ReciverAspNetUsers { get; set; }

        [ForeignKey("MTrnICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RequestId { get; set; }
        public MTrnICardRequest? MTrnICardRequest { get; set; }
        public string? Url { get; set; }


    }
}
