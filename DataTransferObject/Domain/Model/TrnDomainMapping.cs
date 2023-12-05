using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class TrnDomainMapping
    { 
        [Key]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AspNetUsersId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }

        [ForeignKey("MapUnit"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UnitId { get; set; }
        public MapUnit? MapUnit { get; set; }
    }
}
