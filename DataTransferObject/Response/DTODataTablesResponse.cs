namespace DataTransferObject.Response
{
    public class DTODataTablesResponse<T>
    {   
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
        public bool Result { get; set; } 
        public string Message { get; set; }=string.Empty;
    }
    public class DTODataTablesWithSelectedIdsResponse<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<int>? selectedIds { get; set; }
        public List<T> data { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
