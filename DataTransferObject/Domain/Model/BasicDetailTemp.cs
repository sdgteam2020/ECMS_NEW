using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Model
{
    public class BasicDetailTemp:Common
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BasicDetailTempId { get; set; }

        [StringLength(36)]
        [Column(TypeName = "nvarchar(36)")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        [Index("IX_BasicDetails_ServiceNo", IsClustered = false, IsUnique = true, Order = 1)]
        public string ServiceNo { get; set; } = string.Empty;

        public DateTime DOB { get; set; }

        public DateTime DateOfCommissioning { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string PermanentAddress { get; set; } = string.Empty;

        [NotMapped]
        public string? EncryptedId { get; set; }
    }
}
