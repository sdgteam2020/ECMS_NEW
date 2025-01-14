using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MApiData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? applyForId { get; set; }
        public string? pers_Army_No { get; set; }
        public string? pers_name { get; set; }
        public string? pers_Rank { get; set; }
        public string? pers_Father_Name { get; set; }
        public string? pers_birth_dt { get; set; }
        public string? pers_enrol_dt { get; set; }
        public string? pers_District { get; set; }
        public string? pers_State { get; set; }
        public string? pers_Regt { get; set; }
        public string? pers_Height { get; set; }
        public string? pers_UID { get; set; }
        public string? pers_Blood_Gp { get; set; }
        public string? pers_House_no { get; set; }
        public string? pers_Moh_st { get; set; }
        public string? pers_Village { get; set; }
        public string? pers_Tehsil { get; set; }
        public string? pers_Post_office { get; set; }
        public string? pers_Police_stn { get; set; }
        public string? pers_Pin_code { get; set; }
        public string? pers_Iden_mark_1 { get; set; }
        public string? pers_Iden_mark_2 { get; set; }
        public string? pers_Gender { get; set; }
        [NotMapped]
        public Boolean Status { get; set; }
        [NotMapped]
        public string? Message { get; set; }

    }
}
