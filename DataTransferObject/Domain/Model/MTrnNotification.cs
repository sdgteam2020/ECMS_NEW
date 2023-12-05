using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MTrnNotification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int NotificationId { get; set; }
        public int NotificationTypeId { get; set; }
        public bool Read { get; set; }
        public int SenderAspNetUsersId { get; set; }
        public int ReciverAspNetUsersId { get; set; }
        public string Url { get; set; }


    }
}
