namespace DataTransferObject.Requests
{
    public class DTOCSVExportRequest
    {
        public required int[] Ids { get; set; }
        public bool IdsTypeRequestIdOrTrnFwdId { get; set; } = false;
    }
}
