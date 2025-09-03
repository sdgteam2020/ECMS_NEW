using DataTransferObject.Domain.Identitytable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class TrnLogin_Log:Common
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AspNetUsersId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("MUserProfile"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? UserId { get; set; }
        public MUserProfile? MUserProfile { get; set; }

        public string IP { get; set; }

        [ForeignKey("ApplicationRole"), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleId { get; set; }

        public ApplicationRole? ApplicationRole { get; set; }
    }
}
