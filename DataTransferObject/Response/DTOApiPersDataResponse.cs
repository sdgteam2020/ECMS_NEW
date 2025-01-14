using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOApiPersDataResponse1 : DTOAPIBaseResponse
    {
        public string Pers_Army_No { get; set; }=string.Empty;
        public string Pers_name { get; set; } = string.Empty;
        public string Pers_Rank { get; set; } = string.Empty;
        public string Pers_Father_Name { get; set; } = string.Empty;
        public string Pers_birth_dt { get; set; } = string.Empty;
        public string Pers_enrol_dt { get; set; } = string.Empty;
        public string Pers_District { get; set; } = string.Empty;
        public string Pers_State { get; set; } = string.Empty;
        public string Pers_Regt { get; set; } = string.Empty;
        public string Pers_Height { get; set; } = string.Empty;
        public string Pers_UID { get; set; } = string.Empty;
        public string Pers_Blood_Gp { get; set; } = string.Empty;
        public string Pers_House_no { get; set; } = string.Empty;
        public string Pers_Moh_st { get; set; } = string.Empty;
        public string Pers_Village { get; set; } = string.Empty;
        public string Pers_Tehsil { get; set; } = string.Empty;
        public string Pers_Post_office { get; set; } = string.Empty;
        public string Pers_Police_stn { get; set; } = string.Empty;
        public string Pers_Pin_code { get; set; } = string.Empty;
        public string Pers_Iden_mark_1 { get; set; } = string.Empty;
        public string Pers_Iden_mark_2 { get; set; } = string.Empty;
        public string Pers_Gender { get; set; } = string.Empty;
    }
    public class DTOApiPersDataResponse : DTOAPIBaseResponse
    {
        public string pers_Army_No { get; set; } = string.Empty;
        public string pers_name { get; set; } = string.Empty;
        public string pers_Rank { get; set; } = string.Empty;
        public string pers_Father_Name { get; set; } = string.Empty;
        public string pers_birth_dt { get; set; } = string.Empty;
        public string pers_enrol_dt { get; set; } = string.Empty;
        public string pers_District { get; set; } = string.Empty;
        public string pers_State { get; set; } = string.Empty;
        public string pers_Regt { get; set; } = string.Empty;
        public string pers_Height { get; set; } = string.Empty;
        public string pers_UID { get; set; } = string.Empty;
        public string pers_Blood_Gp { get; set; } = string.Empty;
        public string pers_House_no { get; set; } = string.Empty;
        public string pers_Moh_st { get; set; } = string.Empty;
        public string pers_Village { get; set; } = string.Empty;
        public string pers_Tehsil { get; set; } = string.Empty;
        public string pers_Post_office { get; set; } = string.Empty;
        public string pers_Police_stn { get; set; } = string.Empty;
        public string pers_Pin_code { get; set; } = string.Empty;
        public string pers_Iden_mark_1 { get; set; } = string.Empty;
        public string pers_Iden_mark_2 { get; set; } = string.Empty;
        public string pers_Gender { get; set; } = string.Empty;


    }
    public class ApiPersDataResponseData
    {
        public DTOApiPersDataResponse afsac { get; set; }
    }
}
