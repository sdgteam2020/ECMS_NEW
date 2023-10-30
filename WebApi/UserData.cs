namespace WebApi
{
    public class UserData
    {
        public string Name { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;   
        public string AadhaarNo { get; set; } = string.Empty;
        public decimal Ht { get; set; } 
    }
    public class ApiData
    {
        public string Name { get; set; } = string.Empty;
        public string ServiceNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public DateTime DateOfCommissioning { get; set; }
        public string PermanentAddress { get; set; } = string.Empty;
    }
}
