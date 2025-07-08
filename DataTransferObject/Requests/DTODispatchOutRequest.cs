using DataTransferObject.Domain.Identitytable;
using DataTransferObject.Domain.Master;
using DataTransferObject.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTODispatchOutRequest
    {
        public int DispatchCardId { get; set; }

        public byte Step { get; set; } // 1 For AFSAC AND 2 FOR Regiment / Record

        public byte ApplyForId { get; set; }

        public byte? RegId { get; set; }

        public byte? RecordOfficeId { get; set; }
        public DateTime OutDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime DispatchDate { get; set; }

        public byte DispatchModeId { get; set; }

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string RefOfDispatch { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string LotNo { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "VARCHAR(50)")]
        public string NameOfCourierIncharge { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string UploadFilePath { get; set; } = string.Empty;

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? FromRemark { get; set; }

        [StringLength(100)]
        [Column(TypeName = "VARCHAR(100)")]
        public string? ToRemark { get; set; }

        public int FromUnitId { get; set; }

        public int ToUnitId { get; set; }

        public int ToUserId { get; set; }
        
        public int FromUserId { get; set; }

        public int FromAspNetUsersId { get; set; }
        public int ToAspNetUsersId { get; set; }
        public bool IsComplete { get; set; } = false;
    }
}
