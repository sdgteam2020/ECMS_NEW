using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOApiPersDataResponse1 : DTOAPIBaseResponse
    {
        public string Pers_Army_No { get; set; } = string.Empty;
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
    public class ApiPersDataResponseData1
    {
        public DTOApiPersDataResponse1 AFSAC { get; set; }
    }
    public class DTOApiPersDataResponse : DTOAPIBaseResponse
    {
        public string Pers_birth_dt { get; set; } = string.Empty;
        public DTOApiPersAddressResponse Pers_Address { get; set; } = new DTOApiPersAddressResponse(); // Ensures it's never null
        public string Pers_Army_No { get; set; } = string.Empty;
        public string Pers_enrol_dt { get; set; } = string.Empty;
        public string Pers_name { get; set; } = string.Empty;
        //public string pers_Rank { get; set; }
        // public string pers_Father_Name { get; set; }




        //public string pers_Regt { get; set; }
        //public string pers_Height { get; set; }
        //public string pers_UID { get; set; }
        //public string pers_Blood_Gp { get; set; }


        //public string pers_Iden_mark_1 { get; set; }
        //public string pers_Iden_mark_2 { get; set; }
        //public string pers_Gender { get; set; }


    }
    public class DTOApiPersDataFinalResponse : DTOAPIBaseResponse
    {
        public string Pers_Army_No { get; set; } = string.Empty;
        public string Pers_name { get; set; } = string.Empty;
        public string Pers_enrol_dt { get; set; } = string.Empty;
        public string Pers_birth_dt { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOE { get; set; }
        public string? Pers_District { get; set; }
        public string? Pers_State { get; set; }
        public string? Pers_House_no { get; set; }
        public string? Pers_Moh_st { get; set; }
        public string? Pers_Village { get; set; }
        public string? Pers_Tehsil { get; set; }
        public string? Pers_Post_office { get; set; }
        public string? Pers_Police_stn { get; set; }
        public string? Pers_Pin_code { get; set; }
    }
    public class DTOApiPersAddressResponse
    {
        public string? Pers_District { get; set; }
        public string? Pers_State { get; set; }
        public string? Pers_House_no { get; set; }
        public string? Pers_Moh_st { get; set; }
        public string? Pers_Village { get; set; }
        public string? Pers_Tehsil { get; set; }
        public string? Pers_Post_office { get; set; }
        public string? Pers_Police_stn { get; set; }
        public string? Pers_Pin_code { get; set; }
    }

    public class ApiPersDataResponseData
    {
        public DTOApiPersDataResponse afsac { get; set; }
    }
}
