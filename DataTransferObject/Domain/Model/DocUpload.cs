using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class DocUpload: Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocUploadId { get; set; }
        public string DocName { get; set; } = string.Empty;
        public string DocPath { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
