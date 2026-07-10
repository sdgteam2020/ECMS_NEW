using DataTransferObject.Domain.Identitytable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class CompletedICardRequestMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompletedMappingId { get; set; }

        [ForeignKey("CompletedICardRequest"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CompletedId { get; set; }
        public CompletedICardRequest? CompletedICardRequest { get; set; }

        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AspNetUsersId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
