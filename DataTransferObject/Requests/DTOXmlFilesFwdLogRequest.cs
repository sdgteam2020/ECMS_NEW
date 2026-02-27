using System.ComponentModel.DataAnnotations;

namespace DataTransferObject.Requests
{
    public class DTOXmlFilesFwdLogRequest
    {
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Id is number.")]
        public int Id { get; set; }

        [RegularExpression(@"^[\d]+$", ErrorMessage = "RequestId is number.")]
        public int RequestId { get; set; }

        [RegularExpression(@"^[A-Za-z0-9+/=\s]+$", ErrorMessage = "Invalid string.")]
        public string XmlFiles { get; set; } = string.Empty;
        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Updatedby is number.")]
        public int Updatedby { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsActive { get; set; }


    }
    public class DTOXmlFilesForUpdate
    {
        public int Id { get; set; }
      
        public object jsonfile { get; set; }



    }
}
