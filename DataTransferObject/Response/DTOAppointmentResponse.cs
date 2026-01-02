namespace DataTransferObject.Response
{
    public class DTOAppointmentResponse
    {
        public int TotalFilteredRecords { get; set; }
        public int ApptId { get; set; }
        public string AppointmentName { get; set; }=string.Empty;
        public string? AppointmentAbbreviation { get; set; }
        public bool Approved { get; set; }

    }
}
