using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response.User
{
   
    public class UserM 
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}