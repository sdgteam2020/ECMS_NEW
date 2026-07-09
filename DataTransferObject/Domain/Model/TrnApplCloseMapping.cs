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
    public class TrnApplCloseMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplCloseMappingId { get; set; }

        [ForeignKey("TrnApplClose"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CloseId { get; set; }
        public TrnApplClose? TrnApplClose { get; set; }

        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AspNetUsersId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
