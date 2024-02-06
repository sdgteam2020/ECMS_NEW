using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class MApiDataOffrs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ApplyForId { get; set; }
        public string? Pers_Army_No { get; set; }
        public string? Pers_name { get; set; }
        public string? Pers_Rank { get; set; }
        public string? Pers_Father_Name { get; set; }
        public string? Pers_birth_dt { get; set; }
        public string? Pers_enrol_dt { get; set; }
        public string? Pers_District { get; set; }
        public string? Pers_State { get; set; }
        public string? Pers_Regt { get; set; }
        public string? Pers_Height { get; set; }
        public string? Pers_UID { get; set; }
        public string? Pers_Blood_Gp { get; set; }
        public string? Pers_House_no { get; set; }
        public string? Pers_Moh_st { get; set; }
        public string? Pers_Village { get; set; }
        public string? Pers_Tehsil { get; set; }
        public string? Pers_Post_office { get; set; }
        public string? Pers_Police_stn { get; set; }
        public string? Pers_Pin_code { get; set; }
        public string? Pers_Iden_mark_1 { get; set; }
        public string? Pers_Iden_mark_2 { get; set; }
        public string? Pers_Gender { get; set; }
    }
}
