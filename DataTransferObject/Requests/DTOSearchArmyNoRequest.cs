using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Requests
{
    public class DTOSearchArmyNoRequest
    {

        [RegularExpression(@"^[\w]+$", ErrorMessageResourceType = typeof(ErrorMessages), ErrorMessageResourceName = "SpecialChars")]
        public string ArmyNo { get; set; } = string.Empty;

        [RegularExpression(@"^[\d]+$", ErrorMessage = "TypeId is number.")]
        [Range(typeof(byte), "50", "52", ErrorMessage = "Invalid TypeId Input.")]
        public byte TypeId { get; set; }
        public int AspNetUsersId { get; set; }
        public bool Claim { get; set; } = false;
        public int MapUnitId { get; set; }
    }
}
