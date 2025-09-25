namespace DataTransferObject.Response
{
    public class DTORequestSubDashboardCountResponse
    {
        public int ToDraftedOffrs { get; set; }
        public int ToDraftedJCO { get; set; }
        public int ToSubmittedOffrs { get; set; }
        public int ToSubmittedJCO { get; set; }
        public int ToClosedOffrs { get; set; }
        public int ToClosedJCO { get; set; }
        public int ToCompletedOffrs { get; set; }
        public int ToCompletedJCO { get; set; }
        public int ToRejectedOffrs { get; set; }
        public int ToRejectedJCO { get; set; }

    }
}
