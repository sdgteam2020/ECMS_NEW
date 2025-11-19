namespace DataTransferObject.Requests
{
    public class DTOMasterRequest
    {
        public string tableName { get; set; } = string.Empty;
        public int? id { get; set; }
        public string? ParentId { get; set; }
    }
}
