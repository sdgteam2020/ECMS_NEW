using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataTransferObject.Domain.Model
{
    public class MApiLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ClientPW { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string accessKey { get; set; } = string.Empty;

    }
}
