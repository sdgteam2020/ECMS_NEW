using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Web.Data
{
    public class ApplicationUser: IdentityUser
    {
        public int IntId { get; set; }
        //  public string DEFwdAuth { get; set; }
        //  public string GEBFwdAuth { get; set; }
        public string Active { get; set; }
        //  public string EstablishmentFull { get; set; }
        // public string EstablishmentAbbreviation { get; set; }
        // public string Appointment { get; set; }
        // public string PersonnelNumber { get; set; }
        public string RankId { get; set; }
        public string FullName { get; set; }
        // public bool IsPasswordHealthy { get; set; }
        public int Unit_ID { get; set; }


        public int UserTypeId { get; set; }
        public int ParentId { get; set; }


        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Updated By")]
        public int UpdatedBy { get; set; }

        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }
    }
}
