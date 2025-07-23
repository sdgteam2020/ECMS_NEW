using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Master
{
    public class MEncryptionSetting
    {
        [Key]
        public int Id { get; set; }  // Primary Key

        [Column("KeyValue")]
        public required byte[] KeyValue { get; set; }  // 256-bit key (32 bytes)

        [Column("IVValue")]
        public required byte[] IVValue { get; set; }  // 128-bit IV (16 bytes)
    }
}
