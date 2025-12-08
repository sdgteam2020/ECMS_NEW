namespace DataTransferObject.Response
{
    public class DTOBeforePostingOutCheckedInputDataResponse
    {
        public int BasicDetailId { get; set; }
        public int UnitId { get; set; }
        public byte StatusId { get; set; }
        public int? MaxTrnFwdId { get; set; }
        public int? ToAspNetUsersId { get; set; }
        public int? ToUserID { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
