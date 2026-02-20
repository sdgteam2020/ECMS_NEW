using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Draw { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Start { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int Length { get; set; }

        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string? searchValue { get; set; }

        [RegularExpression(@"^[a-zA-Z_]*$", ErrorMessage = "Only alphabets and underscores are allowed.")]
        public string? sortColumn { get; set; } = null; // Make nullable

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string? sortDirection { get; set; } = null; // Make nullable


        [RegularExpression("^[a-zA-Z: ]*$", ErrorMessage = "Only Alphabets and colon are allowed.")]
        public string Choice { get; set; } = string.Empty;

    }
    public class DTODataTablesRequestForMapUnit : DTODataTablesRequest
    {

    }
    public class DTODataTablesRequestForMapUnitChange : DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitMapId { get; set; }
        
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public string RoleName { get; set; } = string.Empty;
    }
    public class DTODataTablesRequestForFaultyCard : DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitMapId { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool Claim { get; set; }
    }
    public class DTODataTablesRequestForReport : DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? TableId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? UnitType { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? ComdId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? CorpsId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? DivId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? BdeId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? FmnBranchID { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? PsoId { get; set; }
        
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? SubDteId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? UnitMapId { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string? MonthYear { get; set; }
    }
    public class DTODataTablesRequestFor_BasicDetails_Index : DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UserId { get; set; }
        
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int stepcount { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int TypeId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int applyForId { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string JCOOR { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool AllChecked { get; set; } = false;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool SearchTextChanged { get; set; } = false;
    }
    public class DTODataTablesRequestForCardDispatch : DTODataTablesRequest
    {
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public byte ClaimValue { get; set; }
        
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int TDMId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitId { get; set; }
     
    }
    public class DTODataTablesRequestForCardDispatchDialog : DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int DispatchCardId { get; set; }
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool AllChecked { get; set; } = false;
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool SearchTextChanged { get; set; } = false;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte ClaimValue { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int TDMId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "StepId is number.")]
        public byte StepId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RegId is number.")]
        public byte? RegId { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RecordOfficeId is number.")]
        public byte? RecordOfficeId { get; set; }
    }
    public class DTODataTablesRequestForCardStatusList:DTODataTablesRequest
    {
        [RegularExpression("^[a-zA-Z0-9_ ]*$", ErrorMessage = "Only Alphabets and Numbers are allowed.")]
        public string? SearchField { get; set; }

        [RegularExpression("^[a-zA-Z0-9_/ ]*$", ErrorMessage = "Only Alphabets,Numbers,underscores and slash are allowed.")]
        public string? SearchText { get; set; }

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool AllChecked { get; set; } = false;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool SearchTextChanged { get; set; } = false;

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int TDMId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int UnitId { get; set; }
    }
    public class DTODataTablesRequestForNotification : DTODataTablesRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "SentAspNetUsersId is number.")]
        public int ReciverAspNetUsersId { get; set; }
    }
    public class DTODataTablesRequestForReportCard : DTODataTablesRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? ApplyForId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? UnitType { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? ComdId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? CorpsId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? DivId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? BdeId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? FmnBranchID { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? PsoId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public byte? SubDteId { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "Numbers allowed.")]
        public int? UnitMapId { get; set; }
    }
    public class DTODataTablesRequestForCommanCheckAll : DTODataTablesRequest
    {
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool AllChecked { get; set; } = false;
        
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public bool SearchTextChanged { get; set; } = false;
    }
    public class DTODataTableRequestForAppCloseList: DTODataTablesRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "apply is number.")]
        public int apply { get; set; }
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "UnitMapId is number.")]
        public int UnitMapId { get; set; }
    }
}