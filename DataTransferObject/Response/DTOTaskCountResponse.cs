namespace DataTransferObject.Response
{
    public  class DTOTaskCountResponse
    {
        public int TotDispatchCards { get; set; }
        public int TotDistCards { get; set; }
        public int TotDestCards { get; set; }
        public int TotHotlistCards { get; set; }
        public int TotMisprintedCard { get; set; }
        public int TotUnitChangeRequest { get; set; }
        public int TotLostCards { get; set; }
    }
}
