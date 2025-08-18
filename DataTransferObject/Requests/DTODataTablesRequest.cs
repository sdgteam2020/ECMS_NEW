using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string sortColumn { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string sortDirection { get; set; } = string.Empty;

        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only Alphabets allowed.")]
        public string Choice { get; set; } = string.Empty;

        //public DataTablesSearch Search { get; set; }
        //public List<DataTablesOrder> Order { get; set; }
        //public List<DataTablesColumn> Columns { get; set; }
    }
    public class DataTablesSearch
    {
        public string Value { get; set; } = string.Empty;
        public bool Regex { get; set; }
    }

    public class DataTablesOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; } = string.Empty;
    }

    public class DataTablesColumn
    {
        public string Data { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTablesSearch Search { get; set; }
    }
    public class DTODataTablesRequestForMapUnit : DTODataTablesRequest
    {

    }
    public class DTODataTablesRequestForMapUnitChange : DTODataTablesRequest
    {
        public int UnitMapId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
    public class DTODataTablesRequestForFaultyCard : DTODataTablesRequest
    {
        public int UnitMapId { get; set; }
        public bool Claim { get; set; }
    }
    public class DTODataTablesRequestForReport : DTODataTablesRequest
    {
        public int? TableId { get; set; }
        public int? UnitType { get; set; }
        public byte? ComdId { get; set; }
        public byte? CorpsId { get; set; }
        public byte? DivId { get; set; }
        public byte? BdeId { get; set; }
        public byte? FmnBranchID { get; set; }
        public byte? PsoId { get; set; }
        public byte? SubDteId { get; set; }
        public int? UnitMapId { get; set; }
        public string? MonthYear { get; set; }
    }
    public class DTODataTablesRequestFor_BasicDetails_Index : DTODataTablesRequest
    {
        public int UserId { get; set; }
        public int stepcount { get; set; }
        public int TypeId { get; set; }
        public int applyForId { get; set; }
        public string JCOOR { get; set; } = string.Empty;
    }
    public class DTODataTablesRequestForCardDispatch : DTODataTablesRequest
    {
        public byte ClaimValue { get; set; }
        public int TDMId { get; set; }
        public int UnitId { get; set; }
     
    }
    public class DTODataTablesRequestForCardDispatchDialog : DTODataTablesRequest
    {
        public int DispatchCardId { get; set; }
    }
    public class DTODataTablesRequestForCardStatusList:DTODataTablesRequest
    {
        public string? SearchField { get; set; }

        [RegularExpression("^[a-zA-Z0-9_/ ]*$", ErrorMessage = "Only Alphabets,Numbers,underscores and slash are allowed.")]
        public string? SearchText { get; set; }
        public bool AllChecked { get; set; } = false;
        public bool SearchTextChanged { get; set; } = false;
    }
}