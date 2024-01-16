using DataTransferObject.Domain.Master;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOUserProfileResponse
    {
        public int MapId { get; set; }
        public int UserId { get; set; }
      
        public string ArmyNo { get; set; }
       
        public string Name { get; set; }
        public int ApptId { get; set; }
        public string AppointmentName { get; set; }
        //public int FormationId { get; set; }
        //public string FormationName { get; set; }
        public int UnitId { get; set; }
        public Boolean IntOffr { get; set; }
        public Boolean IsIO { get; set; }
        public Boolean IsCO { get; set; }
        public string SusNo { get; set; }
        //public int RequestId { get; set; }
        public string UnitName { get; set; }
        public string Rank { get; set;}
        public int RankId { get; set; }

        public string DomainId { get; set; }
        public string RoleName { get; set; }
        public string MappedDate { get; set; }
        public string MappedBy { get; set; }
        //public int IOUserId { get; set; }
        //public string IOArmyNo { get; set; }
        //public string IOName { get; set; }
        //public string IOSusNo { get; set; }
        //public int UnitIdIo { get; set; }
        //public string UnitIo { get; set; }
        //public int GSOUserId { get; set; }
        //public string GSOArmyNo { get; set; }
        //public string GSOName { get; set; }
        //public string GSOSusNo { get; set; }
        //public int UnitIdGSO { get; set; }
        //public string UnitGSO { get; set; }
    }
}
