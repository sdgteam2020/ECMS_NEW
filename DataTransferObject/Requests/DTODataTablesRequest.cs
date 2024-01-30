using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string searchValue { get; set; } = string.Empty;
        public string SortColumn { get; set; } = string.Empty;
        public string SortColumnDirection { get; set; } = string.Empty;

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
}
