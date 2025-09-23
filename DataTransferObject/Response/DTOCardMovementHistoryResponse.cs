namespace DataTransferObject.Response
{
    public class DTOCardMovementHistoryResponse
    {
        public string StepName { get; set; } = string.Empty;
        public string ReportedBy {  get; set; } = string.Empty;
        public string Remark {  get; set; } = string.Empty;
        public DateTime ReportedOn {  get; set; }
    }
}
