using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MEncryptionSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key
        public required byte[] KeyBytes { get; set; }  // 256-bit key (32 bytes)
        public required byte[] IVBytes { get; set; }  // 128-bit IV (16 bytes)

        [Unicode(false)]
        public string PublicKey { get; set; } = string.Empty;

        [Unicode(false)]
        public string PrivateKey { get; set; } = string.Empty;
    }
}
